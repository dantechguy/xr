using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabSpawner : AbstractHitConsumer
{
    [SerializeField] private SpawnSettings spawnSettings;

    public override void OnHit(ARRaycastHit _hit)
    {
        SpawnObject(_hit);
    }

    public void SpawnObject(ARRaycastHit _hit)
    {
        XLogger.Log(Category.Spawn, $"Raycast hit type: {_hit.hitType}");
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
        XLogger.Log(Category.Spawn, $"Hit pose: {hitPose.position}");

        // TODO: link with UI
        GameObject spawnPrefab = spawnSettings.GetActivePrefab();
        GameObject spawnedObject = Instantiate(spawnPrefab, hitPose.position, hitPose.rotation, transform);
        
        var transformable = spawnedObject.GetComponent<ARSpawnedTransformable>();
        transformable.ApplyGlobalScale(spawnSettings.globalScale);
    }

    public void ApplyGlobalScale()
    {
        foreach (Transform child in transform)
        {
            var transformable = child.GetComponent<ARSpawnedTransformable>();
            transformable.ApplyGlobalScale(spawnSettings.globalScale);
        }
    }
}