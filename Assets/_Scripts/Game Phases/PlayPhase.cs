using System;
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

    [Header("New Car Controls")] [SerializeField]
    private TouchAccelerator touchAcceleratorObject;

    [SerializeField] private TouchSteeringWheel touchSteeringWheelObject;
    //
    // [Header("Old Car Controls")] [SerializeField]
    // private GameObject throttleButton;
    //
    // [SerializeField] private GameObject reverseButton;
    // [SerializeField] private GameObject rightButton;
    // [SerializeField] private GameObject leftButton;
    // [SerializeField] private GameObject brakeButton;


    private PrometeoCarController car_;
    private Outline carOutline_;
    private CustomCarController car_old_;
    private Camera camera_;

    public void Enable()
    {
        StartCoroutine(CoEnable());

        if (camera_ == null)
            camera_ = Camera.main;

        // SetUpCarControlsOld();
    }

    private IEnumerator CoEnable()
    {
        XLogger.Log(Category.GamePhase, "Play phase enabled");
        playUICanvas.SetActive(true);
        gameManager.enabled = true;
        gameManager.Enable();

        Transform parent = countDownText.transform.parent;
        if (generalSettings.countDown)
        {
            parent.gameObject.SetActive(true);
            countDownText.text = "3";
            yield return new WaitForSeconds(1);
            countDownText.text = "2";
            yield return new WaitForSeconds(1);
            countDownText.text = "1";
            yield return new WaitForSeconds(1);
        }

        parent.gameObject.SetActive(false);

        SetUpCarControls();

        gameManager.StartTimer(car_);
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
        carOutline_ = car_.GetComponent<Outline>();
        car_.EnableControl(true);
        car_.enabled = true;
        for (var i = 1; i < cars.Length; i++)
        {
            cars[i].EnableControl(false);
            car_.enabled = false;
        }

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
        {
            car_.EnableControl(false);
            car_.enabled = false;
            carOutline_.OutlineMode = Outline.Mode.OutlineAll;
            carOutline_.enabled = false;
            car_ = null;
            carOutline_ = null;
        }

        StopAllCoroutines();
    }

    private void Update()
    {
        if (car_ == null || carOutline_ == null) return;

        // turn on outline of car if car is invisible/blocked
        Vector3 direction = car_.transform.position - camera_.transform.position;
        if (Physics.Raycast(camera_.transform.position, direction, out RaycastHit hit, 100))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                carOutline_.OutlineMode = Outline.Mode.OutlineHidden;
                carOutline_.enabled = true;
                return;
            }
        }

        carOutline_.OutlineMode = Outline.Mode.OutlineAll;
        carOutline_.enabled = false;
    }

    // private void SetUpCarControlsOld()
    // {
    //     var cars = FindObjectsOfType<CustomCarController>();
    //     if (cars.Length == 0)
    //     {
    //         XLogger.LogWarning(Category.GamePhase, "No cars found");
    //         return;
    //     }
    //
    //     car_old_ = cars[0];
    //     car_old_.enabled = true;
    //     for (var i = 1; i < cars.Length; i++)
    //         cars[i].enabled = false;
    //
    //     car_old_.SetUpTouchControls(throttleButton, reverseButton, rightButton, leftButton, brakeButton);
    // }
}