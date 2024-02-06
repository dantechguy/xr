using Logging;
using UnityEngine;

public class PlayPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private GameObject playUICanvas;

    [SerializeField] private GameObject throttleButton;
    [SerializeField] private GameObject reverseButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject brakeButton;


    private CustomCarController car_;

    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Play phase enabled");
        playUICanvas.SetActive(true);

        var cars = FindObjectsOfType<CustomCarController>();
        if (cars.Length == 0)
        {
            XLogger.LogWarning(Category.GamePhase, "No cars found");
            return;
        }

        car_ = cars[0];
        car_.enabled = true;
        for (var i = 1; i < cars.Length; i++)
            cars[i].enabled = false;

        car_.SetUpTouchControls(throttleButton, reverseButton, rightButton, leftButton, brakeButton);
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Play phase disabled");
        playUICanvas.SetActive(false);
        if (car_ != null)
            car_.enabled = false;
    }
}