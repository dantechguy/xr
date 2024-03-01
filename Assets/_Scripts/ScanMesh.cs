using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ScanMesh : MonoBehaviour
{
    [SerializeField] private ARMeshManager arMeshManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] private Transform cameraOffset;
    [SerializeField] private GameObject carBase;
    [SerializeField] private float itemMeshSize;
    public ComputeShader computeShader;

    private int textureIndex = 0; // index of uv channel we are currently using
    private const int totalTextures = 6;
    Texture2D atlasTexture;


    bool shouldExtractTexturedMesh = false;


    public void ExtractTexturedMesh()
    {
        shouldExtractTexturedMesh = true;
    }

    void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!shouldExtractTexturedMesh || eventArgs.projectionMatrix == null)
            return;

        arMeshManager.enabled = false;

        shouldExtractTexturedMesh = false;
        if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cameraImage))
        {
#if UNITY_EDITOR
            XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cameraImage.width, cameraImage.height),
                outputDimensions = new Vector2Int(cameraImage.width, cameraImage.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.None
            };
#else
            XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cameraImage.width, cameraImage.height),
                outputDimensions = new Vector2Int(cameraImage.width, cameraImage.height),
                outputFormat = TextureFormat.RGBA32,
                transformation = XRCpuImage.Transformation.MirrorY
            };
#endif
            Texture2D texture = new Texture2D(cameraImage.width, cameraImage.height, TextureFormat.RGBA32, false);

            // Convert camera image to texture
            byte[] pixelBuffer = new byte[texture.GetRawTextureData().Length];
            GCHandle handle = GCHandle.Alloc(pixelBuffer, GCHandleType.Pinned);
            try
            {
                cameraImage.Convert(conversionParams, handle.AddrOfPinnedObject(), pixelBuffer.Length);
                texture.LoadRawTextureData(pixelBuffer);
                texture.Apply();
            }
            finally
            {
                // Make sure to release the GCHandle to avoid memory leaks
                if (handle.IsAllocated)
                    handle.Free();

                cameraImage.Dispose();
            }

            rawImage.texture = texture;
            rawImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);

            Material materialWithTexture = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (atlasTexture == null)
            {
                atlasTexture = new Texture2D(cameraImage.width * totalTextures, cameraImage.height, TextureFormat.RGBA32, false);
            }

            // Calculate the position in the atlas for the current texture
            int xOffset = (textureIndex % totalTextures) * cameraImage.width;

            // Copy pixels from individual texture to the atlas
            atlasTexture.SetPixels(xOffset, 0, cameraImage.width, cameraImage.height, texture.GetPixels());
            atlasTexture.Apply();

            materialWithTexture.mainTexture = atlasTexture;

            // Get the mesh from ArMeshManager
            var meshes = arMeshManager.meshes;

            // Generate texture
            Matrix4x4 viewMatrix = mainCamera.worldToCameraMatrix;
            foreach (MeshFilter meshFilter in meshes)
            {
                if (meshFilter.mesh.vertices.Length != meshFilter.mesh.triangles.Length)
                    MakeVerticesUnique(meshFilter);

                if (meshFilter.mesh.uv == null || meshFilter.mesh.uv.Length == 0)
                {
                    Vector2[] uvsInit = new Vector2[meshFilter.mesh.vertices.Length];
                    for (int i = 0; i < uvsInit.Length; i++)
                    {
                        uvsInit[i] = new Vector2(-1, -1);
                    }
                    meshFilter.mesh.SetUVs(0, uvsInit);
                }

                Vector2[] uvs = meshFilter.mesh.uv;

                Matrix4x4 worldToScreenMatrix = eventArgs.projectionMatrix.Value * mainCamera.worldToCameraMatrix * cameraOffset.localToWorldMatrix;
                float[] worldToScreenMatrixFlat = new float[16];
                for (int i = 0; i < 16; i++)
                    worldToScreenMatrixFlat[i] = worldToScreenMatrix[i % 4, i / 4];

                NativeArray<Vector3> cameraForwardArray = new NativeArray<Vector3>(1, Allocator.Temp);
                cameraForwardArray[0] = mainCamera.transform.forward;

                int kernelIndex = computeShader.FindKernel("CSMain");

                // Create buffers
                ComputeBuffer verticesBuffer = new ComputeBuffer(meshFilter.mesh.vertices.Length, sizeof(float) * 3);
                ComputeBuffer normalsBuffer = new ComputeBuffer(meshFilter.mesh.normals.Length, sizeof(float) * 3);
                ComputeBuffer uvsBuffer = new ComputeBuffer(meshFilter.mesh.uv.Length, sizeof(float) * 2);
                ComputeBuffer worldToScreenMatrixBuffer = new ComputeBuffer(1, sizeof(float) * 16);
                ComputeBuffer cameraForwardBuffer = new ComputeBuffer(1, sizeof(float) * 3);

                // Set buffers
                verticesBuffer.SetData(meshFilter.mesh.vertices);
                normalsBuffer.SetData(meshFilter.mesh.normals);
                uvsBuffer.SetData(meshFilter.mesh.uv);
                worldToScreenMatrixBuffer.SetData(worldToScreenMatrixFlat);
                cameraForwardBuffer.SetData(cameraForwardArray);

                // Set compute shader parameters
                computeShader.SetBuffer(kernelIndex, "vertices", verticesBuffer);
                computeShader.SetBuffer(kernelIndex, "normals", normalsBuffer);
                computeShader.SetBuffer(kernelIndex, "uvs", uvsBuffer);
                computeShader.SetMatrix("worldToScreenMatrix", worldToScreenMatrix);
                computeShader.SetVector("cameraForward", -mainCamera.transform.forward);
                computeShader.SetInt("TOTAL_TEXTURES", totalTextures);
                computeShader.SetInt("TEXTURE_INDEX", textureIndex);

                // Dispatch compute shader
                computeShader.Dispatch(kernelIndex, meshFilter.mesh.vertices.Length / 64 / 3, 1, 1);

                uvsBuffer.GetData(uvs);



                meshFilter.mesh.SetUVs(0, uvs);

                // Apply texture to material
                meshFilter.GetComponent<MeshRenderer>().material = materialWithTexture;
            }
            textureIndex = (textureIndex + 1) % totalTextures; // Loop through all textures
        }
    }

    void MakeVerticesUnique(MeshFilter meshFilter)
    {
        Mesh mesh = meshFilter.mesh;
        int[] oldTriangles = mesh.triangles;
        int[] newTriangles = new int[oldTriangles.Length];
        Vector3[] oldVertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[oldTriangles.Length];

        for (int i = 0; i < oldTriangles.Length; i++)
        {
            newVertices[i] = new Vector3(oldVertices[oldTriangles[i]].x, oldVertices[oldTriangles[i]].y, oldVertices[oldTriangles[i]].z);
            newTriangles[i] = i;
        }

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    Mesh CombineMeshes(IList<MeshFilter> meshes)
    {
        CombineInstance[] combineInstances = new CombineInstance[meshes.Count];
        for (int j = 0; j < meshes.Count; j++)
        {
            combineInstances[j].mesh = meshes[j].sharedMesh;
            combineInstances[j].transform = meshes[j].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances, true, true);

        return combinedMesh;
    }

    Bounds GetBounds(GameObject obj)
    {
        var prefabMeshes = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[prefabMeshes.Length];
        for (int j = 0; j < prefabMeshes.Length; j++)
        {
            combineInstances[j].mesh = prefabMeshes[j].sharedMesh;
            combineInstances[j].transform = prefabMeshes[j].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances, true, true);
        return combinedMesh.bounds;
    }

    public void CreateCar()
    {
        // GameObject car = Instantiate(carBase);

        // GameObject body = car.GetNamedChild("Body");
        // body.GetComponent<MeshFilter>().mesh = CombineMeshes(arMeshManager.meshes);
        // var bounds = GetBounds(body);

        // var colliderBounds = GetBounds(car.GetNamedChild("Collider"));
        // float scaleFactor = Mathf.Min(colliderBounds.size.x / bounds.size.x, colliderBounds.size.y / bounds.size.y,
        //     colliderBounds.size.z / bounds.size.z);
        // body.transform.localScale *= scaleFactor;

        // body.transform.position = new Vector3(0, -GetBounds(body).min.y, 0);

        // var meshRenderer = body.GetComponent<MeshRenderer>();
        // meshRenderer.material = arMeshManager.meshes[0].GetComponent<MeshRenderer>().material;

        // Save to file
        Mesh combinedMeshes = CombineMeshes(arMeshManager.meshes);
        MeshAndTexture meshAndTexture = new MeshAndTexture();
        meshAndTexture.vertices = combinedMeshes.vertices;
        meshAndTexture.triangles = combinedMeshes.triangles;
        meshAndTexture.normals = combinedMeshes.normals;
        meshAndTexture.uv = combinedMeshes.uv;
        meshAndTexture.texture = atlasTexture;

        string json = JsonUtility.ToJson(meshAndTexture);
        File.WriteAllText("testobj.json", json);
    }

    public void OpenCarFile()
    {
        if (File.Exists("testobj.json"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string jsonData = File.ReadAllText("testobj.json");
            MeshAndTexture meshAndTexture = JsonUtility.FromJson<MeshAndTexture>(jsonData);

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = meshAndTexture.vertices;
            mesh.triangles = meshAndTexture.triangles;
            mesh.normals = meshAndTexture.normals;
            mesh.uv = meshAndTexture.uv;

            Texture2D texture = meshAndTexture.texture;
            Material materialWithTexture = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            materialWithTexture.mainTexture = texture;


            GameObject car = Instantiate(carBase);

            GameObject body = car.GetNamedChild("Body");
            body.GetComponent<MeshFilter>().mesh = mesh;
            var bounds = GetBounds(body);

            var colliderBounds = GetBounds(car.GetNamedChild("Collider"));
            float scaleFactor = Mathf.Min(colliderBounds.size.x / bounds.size.x, colliderBounds.size.y / bounds.size.y,
                colliderBounds.size.z / bounds.size.z);
            body.transform.localScale *= scaleFactor;

            body.transform.position = new Vector3(0, -GetBounds(body).min.y, 0);

            var meshRenderer = body.GetComponent<MeshRenderer>();
            meshRenderer.material = materialWithTexture;
        }
    }
}



[Serializable]
public class MeshAndTexture
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uv;
    public Texture2D texture;
}