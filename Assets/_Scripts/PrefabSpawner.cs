using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] private SpawnSettings spawnSettings;

    public void SpawnObject(ARRaycastHit _hit)
    {
        XLogger.Log(Category.AR, $"Raycast hit type: {_hit.hitType}");
        if (_hit.trackable is not ARPlane plane)
        {
            XLogger.LogWarning(Category.AR, "Hit trackable is not a plane");
            return;
        }

        if (plane.alignment != PlaneAlignment.HorizontalUp)
        {
            XLogger.LogWarning(Category.AR, "Plane alignment is not horizontal up");
            return;
        }
            
        Pose hitPose = _hit.pose;
        XLogger.Log(Category.AR, $"Hit pose: {hitPose.position}");
        
        // TODO: link with UI
        GameObject spawnPrefab = spawnSettings.GetActivePrefab();
        GameObject spawnedObject = Instantiate(spawnPrefab, hitPose.position, hitPose.rotation, transform);
        spawnedObject.transform.localScale = spawnSettings.globalScale * Vector3.one;
    }

    public void ApplyGlobalScale()
    {
        foreach (Transform child in transform)
        {
            if (!child.CompareTag("Spawned")) continue;
            child.localScale = spawnSettings.globalScale * Vector3.one;
        }
    }
}