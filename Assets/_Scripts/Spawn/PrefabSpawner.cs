using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabSpawner : AbstractHitConsumer
{
    [SerializeField] private SpawnSettings spawnSettings;
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private SpawnedObjectAudioPlayer audioPlayer;
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

        GameObject spawnPrefab = spawnSettings.GetActivePrefab();
        GameObject spawnedObject = Instantiate(spawnPrefab, hitPose.position, hitPose.rotation);

        // apply global scale
        var transformable = spawnedObject.GetComponent<ARSpawnedTransformable>();
        transformable.ApplyGlobalScale(spawnSettings.globalScale);

        // attache AR anchor
        ARAnchor anchor = anchorManager.AttachAnchor(plane, _hit.pose);
        anchor.destroyOnRemoval = false;
        spawnedObject.transform.SetParent(anchor.transform);
        
        audioPlayer.PlaySpawnAudio(_hit.pose.position, spawnedObject);

        // immediately select the spawned object
        if (spawnSettings.selectRightAfterSpawn && spawnedObject.TryGetComponent(out ARSpawnedSelectable selectable))
        {
            selectable.OnSelect();
            GamePhaseManger.instance.SwitchPhase(GamePhaseManger.GamePhase.Select);
        }

        trackManager.GenerateTrack();
    }

    public void ApplyGlobalScale()
    {
        foreach (ARAnchor child in anchorManager.trackables)
        {
            var transformable = child.GetComponentInChildren<ARSpawnedTransformable>();
            transformable.ApplyGlobalScale(spawnSettings.globalScale);
        }
    }
}