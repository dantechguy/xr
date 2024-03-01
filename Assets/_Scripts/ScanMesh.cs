using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ScanMesh : MonoBehaviour
{
    [SerializeField] private ARMeshManager arMeshManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera textureCamera;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] private Transform cameraOffset;

    private int textureIndex = 0; // index of uv channel we are currently using
    private const int totalTextures = 4;
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
            // CombineInstance[] combineInstances = new CombineInstance[meshes.Count];
            // for (int j = 0; j < meshes.Count; j++)
            // {
            //     combineInstances[j].mesh = meshes[j].sharedMesh;
            //     combineInstances[j].transform = meshes[j].transform.localToWorldMatrix;
            // }
            // Mesh combinedMesh = new Mesh();
            // combinedMesh.CombineMeshes(combineInstances, true, true);
            // combinedMesh.RecalculateNormals();

            // Generate texture
            Matrix4x4 viewMatrix = mainCamera.worldToCameraMatrix;
            foreach (MeshFilter meshFilter in meshes)
            {
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

                for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
                {
                    if (!(uvs[i].x == -1f && uvs[i].y == -1f))
                    {
                        print("continue");
                        continue;
                    }

                    Matrix4x4 combinedMatrix = eventArgs.projectionMatrix.Value * mainCamera.worldToCameraMatrix * cameraOffset.localToWorldMatrix;
                    Vector3 screenPoint = combinedMatrix.MultiplyPoint(meshFilter.mesh.vertices[i]);

                    float u = (screenPoint.x + 1) / 2;
                    float v = (screenPoint.y + 1) / 2;
                    uvs[i].x = u;
                    uvs[i].y = v;

                    if (screenPoint.z < 0 || uvs[i].x < 0 || uvs[i].x > 1 || uvs[i].y < 0 || uvs[i].y > 1 || Vector3.Dot(meshFilter.mesh.normals[i], mainCamera.transform.forward) > 0)
                        uvs[i] = new Vector2(-1, -1);
                    else
                        uvs[i].x = (uvs[i].x / totalTextures) + ((float)textureIndex / totalTextures); // adjust for texture tiling 

                    print(uvs[i]);
                }
                meshFilter.mesh.SetUVs(0, uvs);

                // Apply texture to material
                meshFilter.GetComponent<MeshRenderer>().material = materialWithTexture;
            }
            textureIndex = (textureIndex + 1) % totalTextures; // Loop through all textures


            // // Apply the textured mesh to a new GameObject
            // GameObject texturedObject = new GameObject("TexturedMesh");
            // texturedObject.AddComponent<MeshFilter>().sharedMesh = combinedMesh;
            // MeshRenderer meshRenderer = texturedObject.AddComponent<MeshRenderer>();

            // texturedObject.transform.parent = transform;

            // meshRenderer.material = materialWithTexture;
        }
    }
}
