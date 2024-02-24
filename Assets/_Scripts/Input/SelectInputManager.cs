using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SelectInputManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private SelectedTransformer selectedTransformer;

    private ARInputActions actions_;
    private InputAction tapAction_;
    private Camera cam_;

    private void OnEnable()
    {
        if (actions_ == null)
            actions_ = new ARInputActions();
        tapAction_ = actions_.TouchscreenGestures.Press;
        tapAction_.performed += OnTap;
        actions_.TouchscreenGestures.Enable();
        
        if (cam_ == null)
            cam_ = Camera.main;
    }

    private void OnDisable()
    {
        tapAction_.performed -= OnTap;
        actions_.TouchscreenGestures.Disable();
    }

    private void OnTap(InputAction.CallbackContext _context)
    {
        var screenPos = actions_.TouchscreenGestures.TapStartPosition.ReadValue<Vector2>();
        XLogger.Log(Category.Input, $"Tap position: {screenPos}");
        
        if (InputUtils.IsPositionOverUI(screenPos))
        {
            XLogger.Log(Category.Input, "Pointer over UI");
            return;
        }
        
        // select other selectables 
        Ray ray = cam_.ScreenPointToRay(screenPos);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            var selectable = hitObject.collider.GetComponentInParent<ARSpawnedSelectable>();
            if (selectable != null)
            {
                XLogger.Log(Category.Input, "Hit selectable spawned object");
                selectable.OnSelect();
                return;
            }
        }

        var hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            XLogger.Log(Category.Select, $"Move to new position");
            selectedTransformer.MoveToHit(hits[0]);
        }
    }
}