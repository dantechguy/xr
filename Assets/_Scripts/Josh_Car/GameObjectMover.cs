using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Logging;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameObjectMover : AbstractHitConsumer
{
    [SerializeField] private GameObject gameObjectToSpawn;

    public override void OnHit(ARRaycastHit _hit)
    {
        MoveObject(_hit);
    }

    public void MoveObject([CanBeNull] GameObject _gameObject, ARRaycastHit _hit)
    {
        if (_gameObject == null)
        {
            XLogger.LogWarning(Category.AR, "No game object to move");
            return;
        }
        gameObjectToSpawn = _gameObject;
        MoveObject(_hit);
    }
    
    public new void MoveObject(ARRaycastHit _hit)
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

        gameObjectToSpawn.transform.position = hitPose.position;
        gameObjectToSpawn.transform.rotation = hitPose.rotation;
    }
}