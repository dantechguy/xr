using UnityEngine;

public static class Utils
{
    public static Bounds GetBounds(GameObject obj)
    {
        var prefabMeshes = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[prefabMeshes.Length];
        for (int j = 0; j < prefabMeshes.Length; j++)
        {
            combineInstances[j].mesh = prefabMeshes[j].sharedMesh;
            combineInstances[j].transform = prefabMeshes[j].transform.localToWorldMatrix;
        }
   
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);
        return combinedMesh.bounds;
    } 
}