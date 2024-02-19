using UnityEngine;
using UnityEngine.EventSystems;

public class TouchAccelerator : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Transform gasPedal;
    public Transform brakePedal;
    public RectTransform container;
    public Canvas canvas;

    float throttle = 0;
    bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        float yPosPct = eventData.position.y / (container.rect.height * canvas.scaleFactor);
        yPosPct = Mathf.Clamp(yPosPct, -1, 1);
        throttle = 2 * yPosPct - 1;
        RotatePedals(throttle);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            float yPosPct = eventData.position.y / (container.rect.height * canvas.scaleFactor);
            yPosPct = Mathf.Clamp(yPosPct, -1, 1);
            throttle = 2 * yPosPct - 1;
            RotatePedals(throttle);
        }
    }

    public void Update()
    {
        // Apply spring force when not dragging
        if (!isDragging)
        {
            throttle = Mathf.MoveTowards(throttle, 0f, 0.02f);
            RotatePedals(throttle);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void RotatePedals(float throttle)
    {
        gasPedal.rotation = Quaternion.Euler(Mathf.Max(throttle * 60, 0), 20, 0);
        brakePedal.rotation = Quaternion.Euler(Mathf.Max(-throttle * 60, 0), 20, 0);
    }

    public float GetThrottle()
    {
        return throttle;
    }
}
