using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnInputManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private AbstractHitConsumer spawner;
    [SerializeField] private SelectionInfo selectionInfo;

    private ARInputActions actions_;
    private InputAction tapAction_;
    private GamePhaseManger phaseManger_;

    private void Start()
    {
        phaseManger_ = FindObjectOfType<GamePhaseManger>();
    }

    private void OnEnable()
    {
        if (actions_ == null)
            actions_ = new ARInputActions();
        tapAction_ = actions_.TouchscreenGestures.Press;
        tapAction_.performed += OnTap;
        actions_.TouchscreenGestures.Enable();
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

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            var selectable = hitObject.collider.GetComponentInParent<ARSpawnedSelectable>();
            if (selectable != null)
            {
                XLogger.Log(Category.Input, "Hit selectable spawned object");
                selectable.OnSelect();
                phaseManger_.SwitchPhase(GamePhaseManger.GamePhase.Select);
                return;
            }
        }

        var hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {
            spawner.OnHit(hits[0]);
        }
    }

}