using System.Collections.Generic;
using Logging;
using UnityEngine;

[CreateAssetMenu(menuName = "Create SpawnSettings", fileName = "SpawnSettings", order = 0)]
public class SpawnSettings : ScriptableObject
{
    public float globalScale = 1.0f;
    public List<GameObject> prefabs;
    public GameObject customCar;
    public int activePrefabIndex;
    public bool selectRightAfterSpawn;
    [Header("Tracks")]
    public float trackScale;
    public bool isClosed;


    public GameObject GetActivePrefab()
    {
        if (activePrefabIndex < 0 || activePrefabIndex > prefabs.Count)
        {
            XLogger.LogError(Category.Spawn, "Active prefab index is out of range");
            return null;
        }
        if (activePrefabIndex == prefabs.Count)
            return customCar;
        
        return prefabs[activePrefabIndex];
    }

}