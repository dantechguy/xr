using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Burst.Intrinsics;
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
    [SerializeField] private MeshFilter scannedMesh;
    [SerializeField] private float itemMeshSize;
    [SerializeField] private ScanMeshSelector scanMeshSelector;
    public ComputeShader meshVertexProcessingComputeShader;
    public ComputeShader intersectMeshesComputeShader;

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

        // if ar mesh manager is still enabled, disable it and preprocess the meshes
        if (arMeshManager.enabled)
        {
            arMeshManager.enabled = false;

            scannedMesh.mesh = CombineMeshes(arMeshManager.meshes);

            MakeVerticesUnique(scannedMesh);
            print(scannedMesh.mesh.vertices.Length);
            print(scannedMesh.mesh.triangles.Length);
            RemoveMeshOutsideOfSelector(scannedMesh);
            print(scannedMesh.mesh.vertices.Length);
            print(scannedMesh.mesh.triangles.Length);

            scanMeshSelector.HideSelectorBox(true);
        }

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

            // Generate texture
            Matrix4x4 viewMatrix = mainCamera.worldToCameraMatrix;

            if (scannedMesh.mesh.uv == null || scannedMesh.mesh.uv.Length == 0)
            {
                Vector2[] uvsInit = new Vector2[scannedMesh.mesh.vertices.Length];
                for (int i = 0; i < uvsInit.Length; i++)
                {
                    uvsInit[i] = new Vector2(-1, -1);
                }
                scannedMesh.mesh.SetUVs(0, uvsInit);
            }

            Vector2[] uvs = scannedMesh.mesh.uv;

            Matrix4x4 worldToScreenMatrix = eventArgs.projectionMatrix.Value * mainCamera.worldToCameraMatrix;
            float[] worldToScreenMatrixFlat = new float[16];
            for (int i = 0; i < 16; i++)
                worldToScreenMatrixFlat[i] = worldToScreenMatrix[i % 4, i / 4];

            NativeArray<Vector3> cameraForwardArray = new NativeArray<Vector3>(1, Allocator.Temp);
            cameraForwardArray[0] = mainCamera.transform.forward;

            int kernelIndex = meshVertexProcessingComputeShader.FindKernel("CSMain");

            // Create buffers
            ComputeBuffer verticesBuffer = new ComputeBuffer(scannedMesh.mesh.vertices.Length, sizeof(float) * 3);
            ComputeBuffer normalsBuffer = new ComputeBuffer(scannedMesh.mesh.normals.Length, sizeof(float) * 3);
            ComputeBuffer uvsBuffer = new ComputeBuffer(scannedMesh.mesh.uv.Length, sizeof(float) * 2);
            ComputeBuffer worldToScreenMatrixBuffer = new ComputeBuffer(1, sizeof(float) * 16);
            ComputeBuffer cameraForwardBuffer = new ComputeBuffer(1, sizeof(float) * 3);

            // Set buffers
            verticesBuffer.SetData(scannedMesh.mesh.vertices);
            normalsBuffer.SetData(scannedMesh.mesh.normals);
            uvsBuffer.SetData(scannedMesh.mesh.uv);
            worldToScreenMatrixBuffer.SetData(worldToScreenMatrixFlat);
            cameraForwardBuffer.SetData(cameraForwardArray);

            // Set compute shader parameters
            meshVertexProcessingComputeShader.SetBuffer(kernelIndex, "vertices", verticesBuffer);
            meshVertexProcessingComputeShader.SetBuffer(kernelIndex, "normals", normalsBuffer);
            meshVertexProcessingComputeShader.SetBuffer(kernelIndex, "uvs", uvsBuffer);
            meshVertexProcessingComputeShader.SetMatrix("worldToScreenMatrix", worldToScreenMatrix);
            meshVertexProcessingComputeShader.SetVector("cameraForward", -mainCamera.transform.forward);
            meshVertexProcessingComputeShader.SetInt("TOTAL_TEXTURES", totalTextures);
            meshVertexProcessingComputeShader.SetInt("TEXTURE_INDEX", textureIndex);

            // Dispatch compute shader
            meshVertexProcessingComputeShader.Dispatch(kernelIndex, scannedMesh.mesh.vertices.Length / 64 / 3, 1, 1);

            uvsBuffer.GetData(uvs);



            scannedMesh.mesh.SetUVs(0, uvs);

            // Apply texture to material
            scannedMesh.GetComponent<MeshRenderer>().material = materialWithTexture;

            textureIndex = (textureIndex + 1) % totalTextures; // Loop through all textures
        }
    }

    void RemoveMeshOutsideOfSelector(MeshFilter meshFilter)
    {

        // int kernelIndex = intersectMeshesComputeShader.FindKernel("CSMain");

        // Vector4[] selectorPoints = new Vector4[4];
        // for (int i = 0; i < 4; i++)
        // {
        //     selectorPoints[i] = new Vector4(scanMeshSelector.cornerPoints[i].x, scanMeshSelector.cornerPoints[i].y, scanMeshSelector.cornerPoints[i].z);
        // }


        // // Create buffers
        // ComputeBuffer verticesBuffer = new ComputeBuffer(meshFilter.mesh.vertices.Length, sizeof(float) * 3);
        // ComputeBuffer trianglesInsideSelectorMaskBuffer = new ComputeBuffer(meshFilter.mesh.vertices.Length, sizeof(int));

        // // Set buffers
        // verticesBuffer.SetData(meshFilter.mesh.vertices);

        // // Set compute shader parameters
        // intersectMeshesComputeShader.SetBuffer(kernelIndex, "vertices", verticesBuffer);
        // intersectMeshesComputeShader.SetBuffer(kernelIndex, "trianglesInsideSelectorMask", trianglesInsideSelectorMaskBuffer);
        // intersectMeshesComputeShader.SetVectorArray("selectorPoints", selectorPoints);
        // intersectMeshesComputeShader.SetFloat("selectorHeight", scanMeshSelector.selectorHeight);
        // intersectMeshesComputeShader.SetInt("totalVertices", meshFilter.mesh.vertices.Length);

        // // Dispatch compute shader
        // intersectMeshesComputeShader.Dispatch(kernelIndex, meshFilter.mesh.vertices.Length / 64 / 3 + 1, 1, 1);

        // int[] trianglesInsideSelectorMask = new int[meshFilter.mesh.vertices.Length];
        // trianglesInsideSelectorMaskBuffer.GetData(trianglesInsideSelectorMask);


        // Mesh mesh = meshFilter.mesh;

        // int trianglesInsideSelectorCount = 0;
        // for (int i = 0; i < mesh.triangles.Length; i++)
        // {
        //     if (trianglesInsideSelectorMask[i] == 1)
        //         trianglesInsideSelectorCount++;
        // }
        // print(trianglesInsideSelectorCount);
        // trianglesInsideSelectorCount = trianglesInsideSelectorCount - trianglesInsideSelectorCount % 3;

        // int[] newTriangles = new int[trianglesInsideSelectorCount];

        // int j = 0;
        // for (int i = 0; i < trianglesInsideSelectorCount; i++)
        // {
        //     if (trianglesInsideSelectorMask[i] == 1)
        //     {
        //         newTriangles[j] = mesh.triangles[i];
        //         j++;
        //     }
        // }

        // mesh.triangles = newTriangles;

        List<int> newTriangles = new List<int>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            if (scanMeshSelector.IsPointInsideSelector(vertices[triangles[i]]) &&
                scanMeshSelector.IsPointInsideSelector(vertices[triangles[i + 1]]) &&
                scanMeshSelector.IsPointInsideSelector(vertices[triangles[i + 2]]))
            {
                newTriangles.Add(triangles[i]);
                newTriangles.Add(triangles[i + 1]);
                newTriangles.Add(triangles[i + 2]);
            }
        }

        // This leaves a bunch of redundant info in the vertices, but we remove that in the MakeVerticesUnique function
        mesh.triangles = newTriangles.ToArray();

        print(mesh.triangles.Length);
        print(newTriangles.ToArray().Length);
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

    public void SaveCarToFile(string filename)
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

        // Rotate vertices
        Vector3[] vertices = scannedMesh.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = scanMeshSelector.GetRotation() * vertices[i];
        }
        scannedMesh.mesh.vertices = vertices;
        scannedMesh.mesh.RecalculateNormals();
        scannedMesh.mesh.RecalculateBounds();
        scannedMesh.mesh.RecalculateTangents();

        // Save to file
        MeshAndTexture meshAndTexture = new MeshAndTexture();
        meshAndTexture.vertices = scannedMesh.mesh.vertices;
        meshAndTexture.triangles = scannedMesh.mesh.triangles;
        meshAndTexture.normals = scannedMesh.mesh.normals;
        meshAndTexture.uv = scannedMesh.mesh.uv;
        meshAndTexture.texture = atlasTexture;

        string json = JsonUtility.ToJson(meshAndTexture);
        File.WriteAllText(filename, json);
    }

    public GameObject OpenCarFile(string filename)
    {
        if (File.Exists(filename))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string jsonData = File.ReadAllText(filename);
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


            GameObject car = carBase;

            GameObject body = car.GetNamedChild("Body");
            body.GetComponent<MeshFilter>().mesh = mesh;
            var bounds = GetBounds(body);

            var colliderBounds = GetBounds(car.GetNamedChild("Collider"));
            float scaleFactor = Mathf.Min(colliderBounds.size.x / bounds.size.x, colliderBounds.size.y / bounds.size.y,
                colliderBounds.size.z / bounds.size.z);
            body.transform.localScale *= scaleFactor;

            var bodyBounds = GetBounds(body);
            Vector3 center = bodyBounds.center;
            body.transform.position = new Vector3(-center.x, -bodyBounds.min.y, -center.z);

            var meshRenderer = body.GetComponent<MeshRenderer>();
            meshRenderer.material = materialWithTexture;

            car.transform.position = new Vector3(0, 2, 0);

            return car;
        }
        else
        {
            throw new Exception("file does not exist");
        }
    }

    public void InstantiateCarFile(string filename)
    {
        GameObject car = OpenCarFile(filename);
        Instantiate(car);
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