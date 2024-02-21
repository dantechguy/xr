using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointPositionSlider : MonoBehaviour
{
    [SerializeField] private SelectionInfo selectionInfo;
    
    private Slider slider_;

    private void OnEnable()
    {
        selectionInfo.onSelected += OnSelected;
    }

    private void OnDisable()
    {
        selectionInfo.onSelected -= OnSelected;
    }

    private void OnSelected()
    {
        if (selectionInfo.GetSelected()!.TryGetComponent(out Waypoint waypoint))
        {
            slider_.value = waypoint.position;
        }
    }
    private void Start()
    {
        slider_ = GetComponent<Slider>();
    }

    public void UpdatePosition()
    {
        if (selectionInfo.GetSelected()!.TryGetComponent(out Waypoint waypoint))
        {
            waypoint.position = (int)slider_.value;
        }
    }
}
