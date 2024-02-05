using Logging;
using UnityEngine;

public class SelectPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private GameObject selectUICanvas;
    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Select phase enabled");
        selectUICanvas.SetActive(true);
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Select phase disabled");
        selectUICanvas.SetActive(false);
    }
}