using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class TrackGenerator : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;


    [Header("Material settings")]
    public Material roadMaterial;
    public Material undersideMaterial;

    private void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    // private void Update()
    // {
    //     List<Transform> checkpoints = new List<Transform> {
    //         GameObject.Find("c1").transform,
    //         GameObject.Find("c2").transform,
    //         GameObject.Find("c3").transform,
    //         GameObject.Find("c4").transform,
    //     };
    //     GenerateTrack(checkpoints, trackScale: 0.5f, isClosed: false);
    // }

    public void GenerateTrack(List<Transform> checkpoints, float trackScale, bool isClosed, float trackWidth = 1f, float trackSmoothing = 1f)
    {
        if (checkpoints.Count >= 2)
        {
            float width = trackWidth * trackScale;
            float thickness = trackScale / 10;
            float smoothing = 1.5f * trackSmoothing * trackScale;

            List<Vector3> points = checkpoints.ConvertAll(c => c.position);
            BezierPath bezierPath = new BezierPath(points: points, isClosed: isClosed, space: PathSpace.xyz);
            bezierPath.AutoControlLength = smoothing;
            bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
            print(bezierPath.NumPoints);

            //Set handles to be parallel to gate
            for (int i = 0; i < checkpoints.Count; i++)
            {
                // The library uses a stupid way of editing control handels
                int handle1Index, handle2Index;

                if (i == 0)
                {
                    handle1Index = -1;
                    handle2Index = 1;
                }
                else if (i == checkpoints.Count - 1)
                {
                    handle1Index = i * 3 - 1;
                    handle2Index = -1;
                }
                else
                {
                    handle1Index = i * 3 - 1;
                    handle2Index = i * 3 + 1;
                }

                if (handle1Index != -1)
                {
                    Vector3 handle1 = checkpoints[i].position + checkpoints[i].forward * smoothing;
                    bezierPath.MovePoint(handle1Index, handle1);
                }

                if (handle2Index != -1)
                {
                    Vector3 handle2 = checkpoints[i].position - checkpoints[i].forward * smoothing;
                    bezierPath.MovePoint(handle2Index, handle2);
                }

                bezierPath.SetAnchorNormalAngle(i, 0);
            }

            VertexPath vertexPath = new VertexPath(bezierPath: bezierPath, transform: transform, maxAngleError: 0.3f, minVertexDst: 0f);

            float textureTiling = (float)Math.Round(vertexPath.length / 5 / trackScale);

            CreateTrackMeshInteral(vertexPath, thickness: thickness, flattenSurface: true, roadWidth: width);
            AssignMaterials(textureTiling: textureTiling);
        }
        else
        {
            mesh.Clear();
        }

        meshCollider.enabled = true;
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }


    private void CreateTrackMeshInteral(VertexPath path, float thickness, bool flattenSurface, float roadWidth)
    {
        Vector3[] verts = new Vector3[path.NumPoints * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
        int[] roadTriangles = new int[numTris * 3];
        int[] underRoadTriangles = new int[numTris * 3];
        int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 8  9
        // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
        int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

        bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
            Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i)).normalized;



            // Find position to left and right of current path vertex
            Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
            Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

            // Add top of road vertices
            verts[vertIndex + 0] = vertSideA;
            verts[vertIndex + 1] = vertSideB;
            // Add bottom of road vertices
            verts[vertIndex + 2] = vertSideA - localUp * thickness;
            verts[vertIndex + 3] = vertSideB - localUp * thickness;

            // Duplicate vertices to get flat shading for sides of road
            verts[vertIndex + 4] = verts[vertIndex + 0];
            verts[vertIndex + 5] = verts[vertIndex + 1];
            verts[vertIndex + 6] = verts[vertIndex + 2];
            verts[vertIndex + 7] = verts[vertIndex + 3];

            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
            uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Bottom of road normals
            normals[vertIndex + 2] = -localUp;
            normals[vertIndex + 3] = -localUp;
            // Sides of road normals
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Set triangle indices
            if (i < path.NumPoints - 1 || path.isClosedLoop)
            {
                for (int j = 0; j < triangleMap.Length; j++)
                {
                    roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                    underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++)
                {
                    sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                }

            }

            vertIndex += 8;
            triIndex += 6;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles(roadTriangles, 0);
        mesh.SetTriangles(underRoadTriangles, 1);
        mesh.SetTriangles(sideOfRoadTriangles, 2);
        mesh.RecalculateBounds();
    }

    void AssignMaterials(float textureTiling)
    {
        if (roadMaterial != null && undersideMaterial != null)
        {
            meshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
            meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
        }
    }

    public void ClearTrack()
    {
        meshCollider.enabled = false;
        mesh.Clear();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}