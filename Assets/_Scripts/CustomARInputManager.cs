using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CustomARInputManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private PrefabSpawner prefabSpawner;

    private ARInputActions actions_;
    private InputAction tapAction_;

    private void OnEnable()
    {
        actions_ = new ARInputActions();
        tapAction_ = actions_.TouchscreenGestures.Press;
        tapAction_.performed += OnTap;
        actions_.TouchscreenGestures.Enable();
    }

    private void OnTap(InputAction.CallbackContext _context)
    {
        var screenPos = actions_.TouchscreenGestures.TapStartPosition.ReadValue<Vector2>();
        XLogger.Log(Category.AR, $"Tap position: {screenPos}");
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            XLogger.Log(Category.AR, "Pointer over UI");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            if (hitObject.collider.CompareTag("Spawned"))
            {
                XLogger.Log(Category.AR, "Hit spawned object");
                Destroy(hitObject.collider.gameObject);
                return;
            }
        }

        var hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            prefabSpawner.SpawnObject(hits[0]);
        }
    }
}