using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ScanMeshSelector : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject pointMarkerPrefab;
    public GameObject selectorBox;
    private ARInputActions actions_;
    private InputAction tapAction_;
    private Camera cam_;

    public Vector3[] cornerPoints = new Vector3[4]; // Array to hold the corner points
    private int cornerPointIndex = 0;

    public float selectorHeight = 2f;

    private GameObject instantiatedSelectorBox;

    public void HideSelectorBox(bool hide)
    {
        instantiatedSelectorBox.SetActive(!hide);
    }

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

        if (InputUtils.IsPositionOverUI(screenPos))
        {
            return;
        }

        Ray ray = cam_.ScreenPointToRay(screenPos);
        RaycastHit hitObject;
        if (Physics.Raycast(ray, out hitObject))
        {
            var selectable = hitObject.collider.GetComponentInParent<ARSpawnedSelectable>();
            if (selectable != null)
            {
                selectable.OnSelect();
                GamePhaseManger.instance.SwitchPhase(GamePhaseManger.GamePhase.Select);
                return;
            }
        }

        var hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPos, hits, TrackableType.PlaneWithinPolygon))
        {

            if (hits[0].trackable is not ARPlane plane)
            {
                return;
            }

            if (plane.alignment != PlaneAlignment.HorizontalUp)
            {
                return;
            }

            Pose hitPose = hits[0].pose;

            GameObject spawnedObject = Instantiate(pointMarkerPrefab, hitPose.position, hitPose.rotation);

            if (cornerPointIndex == 4)
            {
                print(IsPointInsideSelector(hitPose.position));
                print(hitPose.position);
                return;
            }

            cornerPoints[cornerPointIndex] = hitPose.position;
            cornerPointIndex = (cornerPointIndex + 1);

            UpdateSelectorBox();
        }
    }

    private void UpdateSelectorBox()
    {
        if (cornerPoints.All(point => point != Vector3.zero))
        {
            // Calculate center
            Vector3 center = Vector3.zero;
            foreach (Vector3 point in cornerPoints)
            {
                center += point;
            }
            center /= cornerPoints.Length;
            center.y += selectorHeight * 0.5f;

            // Calculate rotation
            Quaternion rotation = Quaternion.LookRotation((cornerPoints[0] + cornerPoints[1]) * 0.5f - (cornerPoints[2] + cornerPoints[3]) * 0.5f);

            float width = 0.5f * (Vector3.Distance(cornerPoints[0], cornerPoints[1]) + Vector3.Distance(cornerPoints[2], cornerPoints[3]));
            float depth = 0.5f * (Vector3.Distance(cornerPoints[3], cornerPoints[0]) + Vector3.Distance(cornerPoints[1], cornerPoints[2]));

            // Instantiate the rectangular prism
            instantiatedSelectorBox = Instantiate(selectorBox, center, rotation);
            instantiatedSelectorBox.transform.localScale = new Vector3(width, selectorHeight, depth);
            instantiatedSelectorBox.SetActive(true);
        }
    }

    // This algorithm works by casting a ray from the point to be tested and counting how many times it intersects 
    // with the edges of the polygon. If the number of intersections is odd, the point is inside the polygon,
    // otherwise, it is outside
    public bool IsPointInsideSelector(Vector3 point)
    {

        // Check on the y plane
        float minY = float.PositiveInfinity;
        foreach (Vector3 cornerPoint in cornerPoints)
        {
            if (cornerPoint.y < minY)
                minY = cornerPoint.y;
        }
        float maxY = minY + selectorHeight;

        bool isInsideY = point.y > minY && point.y < maxY;

        if (!isInsideY)
            return false;

        // Check on the xz plane
        bool isInsideXZ = false;
        int j = 3;

        for (int i = 0; i < 4; i++)
        {
            if ((cornerPoints[i].z < point.z && cornerPoints[j].z >= point.z ||
                 cornerPoints[j].z < point.z && cornerPoints[i].z >= point.z) &&
                 (cornerPoints[i].x <= point.x || cornerPoints[j].x <= point.x))
            {
                if (cornerPoints[i].x + (point.z - cornerPoints[i].z) /
                    (cornerPoints[j].z - cornerPoints[i].z) * (cornerPoints[j].x - cornerPoints[i].x) < point.x)
                {
                    isInsideXZ = !isInsideXZ;
                }
            }
            j = i;
        }

        return isInsideXZ && isInsideY;
    }
}
