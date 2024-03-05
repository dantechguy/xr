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


    // Pinch gesture variables
    private float initialDistance;

    // Twist gesture variables
    private Vector2 initialTouch0Position;
    private Vector2 initialTouch1Position;
    private float initialObjectRotation;
    private float initialObjectScale;

    private bool didHaveTwoFingers = false;

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

    void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (InputUtils.IsPositionOverUI(Input.GetTouch(i).position))
            {
                XLogger.Log(Category.Input, "Pointer over UI");
                return;
            }
        }

        if (Input.touchCount == 0)
        {
            didHaveTwoFingers = false;
        }
        else if (Input.touchCount == 1 && !didHaveTwoFingers)
        {
            Touch touch0 = Input.GetTouch(0);

            if (touch0.phase == UnityEngine.TouchPhase.Moved || (touch0.phase == UnityEngine.TouchPhase.Ended && !didHaveTwoFingers))
            {

                var hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch0.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    XLogger.Log(Category.Select, $"Move to new position");
                    selectedTransformer.MoveToHit(hits[0]);
                }
            }
        }
        // Check if there are two touches for pinch and twist gestures
        else if (Input.touchCount == 2)
        {
            didHaveTwoFingers = true;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Pinch gesture
            if (touch0.phase == UnityEngine.TouchPhase.Began || touch1.phase == UnityEngine.TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touch0.position, touch1.position);
                initialObjectScale = selectedTransformer.GetScale();
            }
            else if (touch0.phase == UnityEngine.TouchPhase.Moved || touch1.phase == UnityEngine.TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);
                float scaleFactor = currentDistance / initialDistance;
                selectedTransformer.ApplyLocalScale(initialObjectScale * scaleFactor);
            }

            // Twist gesture
            if (touch0.phase == UnityEngine.TouchPhase.Began || touch1.phase == UnityEngine.TouchPhase.Began)
            {
                initialTouch0Position = touch0.position;
                initialTouch1Position = touch1.position;
                initialObjectRotation = selectedTransformer.GetRotation();
            }

            if ((touch0.phase == UnityEngine.TouchPhase.Moved || touch1.phase == UnityEngine.TouchPhase.Moved) &&
                     (touch0.phase != UnityEngine.TouchPhase.Began || touch1.phase != UnityEngine.TouchPhase.Began))
            {
                Vector2 currentTouch0Position = touch0.position;
                Vector2 currentTouch1Position = touch1.position;
                float initialAngle = Mathf.Atan2(initialTouch1Position.y - initialTouch0Position.y,
                                                  initialTouch1Position.x - initialTouch0Position.x) * Mathf.Rad2Deg;
                float currentAngle = Mathf.Atan2(currentTouch1Position.y - currentTouch0Position.y,
                                                  currentTouch1Position.x - currentTouch0Position.x) * Mathf.Rad2Deg;
                print(initialAngle);
                print(currentAngle);
                float angleOffset = currentAngle - initialAngle - initialObjectRotation;
                selectedTransformer.ApplyRotation(-angleOffset);
            }
        }
    }

    private void OnTap(InputAction.CallbackContext _context)
    {
#if UNITY_EDITOR
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
#endif
    }
}