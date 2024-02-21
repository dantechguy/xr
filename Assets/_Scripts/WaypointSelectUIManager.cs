using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaypointSelectUIManager : MonoBehaviour
{
    [SerializeField] private GameObject waypointSelectUIPanel;
    [SerializeField] private TextMeshProUGUI orderInTrackText;
    [SerializeField] private SelectionInfo selectionInfo;
    [SerializeField] private TrackManager trackManager;

    private void Start()
    {
        waypointSelectUIPanel.SetActive(false);
    }

    private void OnEnable()
    {
        selectionInfo.onSelected += OnSelected;
        selectionInfo.onDeselected += OnDeSelected;
        trackManager.onTrackGenerated += OnTrackGenerated;
    }

    private void OnDisable()
    {
        selectionInfo.onSelected -= OnSelected;
        selectionInfo.onDeselected -= OnDeSelected;
        trackManager.onTrackGenerated -= OnTrackGenerated;
    }

    private void OnTrackGenerated()
    {
        ARSpawnedSelectable selected = selectionInfo.GetSelected();
        
        if (selected != null && selected.TryGetComponent(out Waypoint waypoint))
            SetOrderText(waypoint);
    }

    private void OnDeSelected()
    {
        waypointSelectUIPanel.SetActive(false);
    }

    private void OnSelected()
    {
        if (selectionInfo.GetSelected()!.TryGetComponent(out Waypoint waypoint))
        {
            waypointSelectUIPanel.SetActive(true);
            SetOrderText(waypoint);
        }
        else
        {
            waypointSelectUIPanel.SetActive(false);
        }
    }

    private void SetOrderText(Waypoint _waypoint)
    {
        int orderInTrack = _waypoint.GetOrderInTrack();
        orderInTrackText.text = $"Waypoint-" + orderInTrack
                                             + (orderInTrack == 0 ? "-(START)" : "")
                                             + (orderInTrack == selectionInfo.GetSelected()!.transform.parent
                                                 .childCount - 1
                                                 ? "-(END)"
                                                 : "");
    }
}