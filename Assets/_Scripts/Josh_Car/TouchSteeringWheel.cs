using UnityEngine;
using UnityEngine.EventSystems;
using Logging;


public class TouchSteeringWheel : MonoBehaviour
{
    // Reference to the steering wheel RectTransform
    public RectTransform wheel;

    // Variables to track input
    private bool isDragging = false;
    private float startAngle;
    private float currentRotation;
    int touchId = -1;


    // Update is called once per frame
    void Update()
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
                        if (!isDragging && RectTransformUtility.RectangleContainsScreenPoint(wheel, touch.position))
                        {
                            isDragging = true;
                            float deltaX = (wheel.position.x - touch.position.x) / (wheel.rect.width / 2);
                            float angle = deltaX * 90;
                            startAngle = 0;// = angle - currentRotation;
                            SetSteeringAngle(angle);
                            currentRotation = angle;
                            touchId = i;
                        }
                        break;
                    }
                case TouchPhase.Moved:
                    {
                        if (isDragging && touchId == i)
                        {
                            float deltaX = (wheel.position.x - touch.position.x) / (wheel.rect.width / 2);
                            float angle = deltaX * 90;
                            SetSteeringAngle(angle);
                            currentRotation = angle;
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
            currentRotation = Mathf.MoveTowards(currentRotation, 0f, 10f);
            SetSteeringAngle(currentRotation);
        }

        if (Input.touchCount == 0)
        {
            isDragging = false;
            touchId = -1;
        }
    }

    private void SetSteeringAngle(float angle)
    {
        wheel.rotation = Quaternion.Euler(0, 0, angle);

    }

    public float GetSteering()
    {
        return -currentRotation / 90f;
    }
}
