using UnityEngine;
using UnityEngine.EventSystems;
using Logging;


public class TouchSteeringWheel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Reference to the steering wheel RectTransform
    public RectTransform wheel;

    // Variables to track input
    private bool isDragging = false;
    private float startAngle;
    private float currentRotation;


    // Update is called once per frame
    void Update()
    {
        // Apply spring force when not dragging
        if (!isDragging)
        {
            currentRotation = Mathf.MoveTowards(currentRotation, 0f, 1f);
            SetSteeringAngle(currentRotation);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        Vector2 direction = eventData.position - new Vector2(wheel.position.x, wheel.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        startAngle = angle - currentRotation;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Apply steering input if the user is dragging
        if (isDragging)
        {
            Vector2 direction = eventData.position - new Vector2(wheel.position.x, wheel.position.y);
            float angle = Mathf.Clamp(-Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - startAngle, -180f, 180f);
            // Apply rotation to your vehicle controller or steering wheel visualization
            // For example:
            SetSteeringAngle(angle);
            currentRotation = angle;
        }
    }

    private void SetSteeringAngle(float angle)
    {
        wheel.rotation = Quaternion.Euler(0, 0, angle);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public float GetSteering()
    {
        return -currentRotation / 250f;
    }
}
