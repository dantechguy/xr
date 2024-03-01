using System.Collections;
using Logging;
using TMPro;
using UnityEngine;

public class PlayPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private GameObject playUICanvas;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GeneralSettings generalSettings;
    [SerializeField] private TextMeshProUGUI countDownText;

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
        StartCoroutine(CoEnable());
        
        // SetUpCarControlsOld();
    }

    private IEnumerator CoEnable()
    {
        XLogger.Log(Category.GamePhase, "Play phase enabled");
        playUICanvas.SetActive(true);

        Transform parent = countDownText.transform.parent;
        parent.gameObject.SetActive(true);
        countDownText.text = "3";
        yield return new WaitForSeconds(1);
        countDownText.text = "2";
        yield return new WaitForSeconds(1);
        countDownText.text = "1";
        yield return new WaitForSeconds(1);
        parent.gameObject.SetActive(false);

        gameManager.enabled = true;
        gameManager.Enable();

        SetUpCarControls();
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