using System;
using Logging;
using UnityEngine;

/// <summary>
/// Every prefab that can be selected in the selection phase must have this component
/// </summary>
public class ARSpawnedSelectable : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private SelectionInfo selectionInfo;

    private bool selected_;

    private void Start()
    {
        if (selectionInfo == null)
            XLogger.LogError(Category.Select, "Selection info not set");
        
        if (selectionIndicator == null)
            XLogger.LogError(Category.Select, "Selection indicator not set");

        selectionIndicator.SetActive(selected_);
    }

    public void OnSelect()
    {
        selected_ = true;
        selectionIndicator.SetActive(true);
        selectionInfo.SetSelected(this);
    }

    public void OnDeselect()
    {
        selected_ = false;
        selectionIndicator.SetActive(false);
    }
}