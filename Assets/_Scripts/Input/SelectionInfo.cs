using System;
using JetBrains.Annotations;
using Logging;
using UnityEngine;

/// <summary>
/// Acts as the global state for selection 
/// </summary>
[CreateAssetMenu(menuName = "Create SelectionInfo", fileName = "SelectionInfo", order = 0)]
public class SelectionInfo : ScriptableObject
{
    private ARSpawnedSelectable selected_;
    private bool justSelected_;
    
    public event Action onSelected;

    public void SetSelected(ARSpawnedSelectable _selected)
    {
        selected_ = _selected;
        justSelected_ = true;
        onSelected?.Invoke();
    }
    
    public bool IsJustSelected()
    {
        if (!justSelected_) return justSelected_;
        
        justSelected_ = false;
        return true;
    }
    
    public void ClearSelected()
    {
        if (selected_ == null) return;
        selected_.OnDeselect();
        selected_ = null;
    }
    
    [CanBeNull]
    public ARSpawnedSelectable GetSelected()
    {
        if (selected_ == null)
        {
            XLogger.LogWarning(Category.Select, "No selected object");
        }
        return selected_;
    }
}