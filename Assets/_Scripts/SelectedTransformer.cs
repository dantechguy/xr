using System;
using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(GameObjectMover))]
public class SelectedTransformer : MonoBehaviour
{
    [SerializeField] private SelectionInfo selectionInfo;
    [SerializeField] private TrackManager trackManager;

    private GameObjectMover mover_;

    private void Start()
    {
        mover_ = GetComponent<GameObjectMover>();
    }

    public void MoveToHit(ARRaycastHit _hit)
    {
        // if (selectionInfo.IsJustSelected()) return; // prevent the selection tap from moving the object
        mover_.MoveObject(selectionInfo.GetSelected()?.gameObject, _hit, false);
        trackManager.GenerateTrack();
    }

    public void Delete()
    {
        ARSpawnedSelectable selected = selectionInfo.GetSelected();
        if (selected != null)
        {
            // has to add this line because destroy take place at the end of the frame
            if (selected.TryGetComponent(out Waypoint waypoint))
                waypoint.enabled = false;
            Destroy(selected.GetComponentInParent<ARAnchor>().gameObject);
        }
        selectionInfo.ClearSelected();
        GamePhaseManger.instance.SwitchPhase(GamePhaseManger.GamePhase.Spawn);
        trackManager.GenerateTrack();
    }

    public void ApplyLocalScale(float _localScale)
    {
        selectionInfo.GetSelected()?.GetComponent<ARSpawnedTransformable>()?.ApplyLocalScale(_localScale);
    }

    public SelectionInfo GetSelectionInfo()
    {
        return selectionInfo;
    }

    public void ApplyRotation(float _angle)
    {
        selectionInfo.GetSelected()?.GetComponent<ARSpawnedTransformable>()?.ApplyRotation(_angle);
        trackManager.GenerateTrack();
    }

    public float GetRotation()
    {
        return selectionInfo.GetSelected()!.GetComponent<ARSpawnedTransformable>()!.GetRotationAngle();
    }

    public float GetScale()
    {
        return selectionInfo.GetSelected()!.GetComponent<ARSpawnedTransformable>()!.GetLocalScale();
    }
}