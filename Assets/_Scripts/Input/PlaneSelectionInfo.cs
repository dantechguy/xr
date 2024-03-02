using System;
using JetBrains.Annotations;
using Logging;
using UnityEngine;

/// <summary>
/// Acts as the global state for selection 
/// </summary>
[CreateAssetMenu(menuName = "PlaneSelectionInfo", fileName = "PlaneSelectionInfo", order = 0)]
public class PlaneSelectionInfo : ScriptableObject
{
    private ARPlaneSelectable selected_;
    private bool justSelected_;

    public event Action onPlaneSelected;
    public event Action onPlaneDeselected;

    public void SetSelected(ARPlaneSelectable _selected)
    {
        // if the same object is selected, do nothing
        if (selected_ == _selected) return;

        ClearSelected();
        selected_ = _selected;
        justSelected_ = true;
        onPlaneSelected?.Invoke();
    }

    public void ClearSelected()
    {
        if (selected_ == null) return;
        selected_.OnDeselect();
        selected_ = null;
        onPlaneDeselected?.Invoke();
    }

    [CanBeNull]
    public ARPlaneSelectable GetSelected()
    {
        if (selected_ == null)
        {
            XLogger.LogWarning(Category.Select, "No selected plane");
        }

        return selected_;
    }
}