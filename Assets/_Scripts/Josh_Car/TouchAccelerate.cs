using UnityEngine;
using UnityEngine.EventSystems;

public class TouchAccelerator : MonoBehaviour
{
    public Transform gasPedal;
    public Transform brakePedal;
    public RectTransform container;
    public Canvas canvas;

    float throttle = 0;
    bool isDragging = false;
    int touchId = -1;


    public void Update()
    {
        // Loop through all the touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            // Get the touch object at index i
            Touch touch = Input.GetTouch(i);

            // Check the phase of the touch
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        if (!isDragging && RectTransformUtility.RectangleContainsScreenPoint(container, touch.position))
                        {
                            isDragging = true;
                            float yPosPct = touch.position.y / (container.rect.height * canvas.scaleFactor);
                            yPosPct = Mathf.Clamp(yPosPct, -1, 1);
                            throttle = 2 * yPosPct - 1;
                            RotatePedals(throttle);
                            touchId = i;
                        }
                        break;
                    }
                case TouchPhase.Moved:
                    {
                        if (isDragging && touchId == i)
                        {
                            float yPosPct = touch.position.y / (container.rect.height * canvas.scaleFactor);
                            yPosPct = Mathf.Clamp(yPosPct, -1, 1);
                            throttle = 2 * yPosPct - 1;
                            RotatePedals(throttle);
                        }
                        break;
                    }
                case TouchPhase.Ended:
                    if (isDragging && touchId == i)
                    {
                        isDragging = false;
                        touchId = -1;
                    }
                    break;
                case TouchPhase.Canceled:
                    if (isDragging && touchId == i)
                    {
                        isDragging = false;
                        touchId = -1;
                    }
                    break;
            }
        }

        // Apply spring force when not dragging
        if (!isDragging)
        {
            throttle = Mathf.MoveTowards(throttle, 0f, 0.2f);
            RotatePedals(throttle);
        }
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
