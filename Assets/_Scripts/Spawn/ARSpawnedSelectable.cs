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

    private Outline outline_;
    private bool selected_;

    private void Awake()
    {
        outline_ = GetComponent<Outline>();
        if (outline_ == null)
            XLogger.LogError(Category.Select, "Outline not found");
    }

    private void Start()
    {
        if (selectionInfo == null)
            XLogger.LogError(Category.Select, "Selection info not set");
        
        if (selectionIndicator == null)
            XLogger.LogError(Category.Select, "Selection indicator not set");

        selectionIndicator.SetActive(selected_);
        ToggleOutline(selected_);
    }

    public void OnSelect()
    {
        selected_ = true;
        selectionIndicator.SetActive(true);
        ToggleOutline(true);
        selectionInfo.SetSelected(this);
    }

    public void OnDeselect()
    {
        selected_ = false;
        selectionIndicator.SetActive(false);
        ToggleOutline(false);
    }

    public void ToggleOutline(bool _active)
    {
        outline_.enabled = _active;
    }
}