using Logging;
using UnityEngine;

public class PlayPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private GameObject playUICanvas;
    [SerializeField] private GameManager gameManager;

    [Header("New Car Controls")]
    [SerializeField] private TouchAccelerator touchAcceleratorObject;
    [SerializeField] private TouchSteeringWheel touchSteeringWheelObject;

    [Header("Old Car Controls")]
    [SerializeField]
    private GameObject throttleButton;

    [SerializeField] private GameObject reverseButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject brakeButton;


    private PrometeoCarController car_;
    private CustomCarController car_old_;

    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Play phase enabled");

        playUICanvas.SetActive(true);
        gameManager.enabled = true;
        gameManager.Enable();

        SetUpCarControls();
        // SetUpCarControlsOld();
    }

    private void SetUpCarControls()
    {
        var cars = FindObjectsOfType<PrometeoCarController>();
        if (cars.Length == 0)
        {
            XLogger.LogWarning(Category.GamePhase, "No cars found");
            return;
        }

        car_ = cars[0];
        car_.EnableControl(true);
        car_.enabled = true;
        for (var i = 1; i < cars.Length; i++)
            cars[i].EnableControl(false);

        car_.touchAcceleratorObject = touchAcceleratorObject;
        car_.touchSteeringWheelObject = touchSteeringWheelObject;
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Play phase disabled");

        playUICanvas.SetActive(false);
        gameManager.Disable();
        gameManager.enabled = false;

        if (car_ != null)
            car_.EnableControl(false);
    }

    private void SetUpCarControlsOld()
    {
        var cars = FindObjectsOfType<CustomCarController>();
        if (cars.Length == 0)
        {
            XLogger.LogWarning(Category.GamePhase, "No cars found");
            return;
        }

        car_old_ = cars[0];
        car_old_.enabled = true;
        for (var i = 1; i < cars.Length; i++)
            cars[i].enabled = false;

        car_old_.SetUpTouchControls(throttleButton, reverseButton, rightButton, leftButton, brakeButton);
    }
}