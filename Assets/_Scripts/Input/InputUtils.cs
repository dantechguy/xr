using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputUtils
{
    public static bool IsPositionOverUI(Vector2 _screenPos)
    {
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = _screenPos
        };
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return raycastResults.Count > 0;
    }   
}