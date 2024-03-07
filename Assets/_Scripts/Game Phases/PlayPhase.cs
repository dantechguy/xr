using System;
using System.Collections;
using System.Collections.Generic;
using Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private GameObject playUICanvas;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GeneralSettings generalSettings;
    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private Image countDownBackground;
    [SerializeField] private float countDownInterval = 1.0f;
    [SerializeField] private Color countDownRed;
    [SerializeField] private Color countDownGreen;
    
    [Header("New Car Controls")] [SerializeField]
    private TouchAccelerator touchAcceleratorObject;

    [SerializeField] private TouchSteeringWheel touchSteeringWheelObject;

    [Header("Sounds")] [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<AudioClip> countSounds;
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

        Transform countDown = countDownText.transform.parent;
        if (generalSettings.countDown)
        {
            countDown.gameObject.SetActive(true);
            countDownText.text = "3";
            countDownBackground.color = countDownRed;
            audioSource.clip = countSounds[0];
            audioSource.Play();
            yield return new WaitForSeconds(countDownInterval);
            countDownText.text = "2";
            yield return new WaitForSeconds(countDownInterval);
            countDownText.text = "1";
            yield return new WaitForSeconds(countDownInterval);
            countDownText.text = "";
            countDownBackground.color = countDownGreen;
        }
        else
        {
            countDown.gameObject.SetActive(false);
        }

        SetUpCarControls();

        gameManager.StartTimer(car_);

        if (generalSettings.countDown)
        {
            yield return new WaitForSeconds(countDownInterval * 0.6f);
            countDown.gameObject.SetActive(false);
        }
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