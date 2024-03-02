using Logging;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaneSelectPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private PlaneSelectionInfo selectionInfo;
    [SerializeField] private GameObject selectUICanvas;
    
    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Plane select phase enabled");
        selectUICanvas.SetActive(true);
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Plane select phase disabled");
        selectUICanvas.SetActive(false);
        selectionInfo.ClearSelected();
    }
}