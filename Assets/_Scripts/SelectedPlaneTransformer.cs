using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SelectedPlaneTransformer : MonoBehaviour
{
    [SerializeField] private PlaneSelectionInfo selectionInfo;
    [SerializeField] private Transform groundCollider;

    private void Start()
    {
        groundCollider.gameObject.SetActive(false);
    }

    public void Delete()
    {
        ARPlaneSelectable selected = selectionInfo.GetSelected();
        if (selected != null)
        {
            selected.gameObject.SetActive(false);
        }
        selectionInfo.ClearSelected();
        GamePhaseManger.instance.SwitchPhase(GamePhaseManger.GamePhase.Spawn);
    }

    // set as the ground plane
    public void Extend()
    {
        ARPlaneSelectable selected = selectionInfo.GetSelected();
        selectionInfo.SetGroundCollider();
        if (selected != null)
        {
            groundCollider.gameObject.SetActive(true);
            groundCollider.position = selected.transform.position;
        }
    }
}