using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneTransparencyApplier : MonoBehaviour
{
    [SerializeField] private GeneralSettings generalSettings;

    private ARPlaneManager planeManager_;

    private void Start()
    {
        planeManager_ = GetComponent<ARPlaneManager>();
    }

    // the setting the transparency for new planes once doesn't seem enough, it always reverts back 
    // ugly but has to set it every frame
    private void Update()
    {
        foreach (ARPlane plane in planeManager_.trackables)
            generalSettings.ApplyTransparencyToPlane(plane);
    }
}