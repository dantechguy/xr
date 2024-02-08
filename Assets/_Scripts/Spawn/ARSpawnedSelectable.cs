﻿using System;
using Logging;
using UnityEngine;

/// <summary>
/// Every prefab that can be selected in the selection phase must have this component
/// </summary>
public class ARSpawnedSelectable : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private SelectionInfo selectionInfo;

    private void Start()
    {
        if (selectionInfo == null)
            XLogger.LogError(Category.Select, "Selection info not set");
        
        if (selectionIndicator == null)
            XLogger.LogError(Category.Select, "Selection indicator not set");

        selectionIndicator.SetActive(false);
    }

    public void OnSelect()
    {
        selectionIndicator.SetActive(true);
        selectionInfo.SetSelected(this);
    }

    public void OnDeselect()
    {
        selectionIndicator.SetActive(false);
    }
}