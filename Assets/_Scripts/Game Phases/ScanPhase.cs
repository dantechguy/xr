using Logging;
using UnityEngine;
using UnityEngine.Serialization;

public class ScanPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private GameObject scanUICanvas;
    [SerializeField] private ScanMeshSelector scanMeshSelector;

    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Scan phase enabled");
        scanUICanvas.SetActive(true);
        scanMeshSelector.enabled = true;
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Scan phase disabled");
        scanUICanvas.SetActive(false);
        scanMeshSelector.enabled = false;
    }
}