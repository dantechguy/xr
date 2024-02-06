﻿using System.Collections.Generic;
using Logging;
using UnityEngine;

[CreateAssetMenu(menuName = "Create SpawnSettings", fileName = "SpawnSettings", order = 0)]
public class SpawnSettings : ScriptableObject
{
    public float globalScale = 1.0f;
    public List<GameObject> prefabs;
    public int activePrefabIndex;

    public GameObject GetActivePrefab()
    {
        if (activePrefabIndex < 0 || activePrefabIndex >= prefabs.Count)
        {
            XLogger.LogError(Category.Spawn, "Active prefab index is out of range");
            return null;
        }
        return prefabs[activePrefabIndex];
    }

}