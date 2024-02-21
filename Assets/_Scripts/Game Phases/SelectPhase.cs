using Logging;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [FormerlySerializedAs("selectionInfo_")] [SerializeField] private SelectionInfo selectionInfo;
    [SerializeField] private GameObject selectUICanvas;
    [SerializeField] private SelectInputManager selectInputManager;
    
    public void Enable()
    {
        XLogger.Log(Category.GamePhase, "Select phase enabled");
        selectUICanvas.SetActive(true);
        selectInputManager.enabled = true;
    }

    public void Disable()
    {
        XLogger.Log(Category.GamePhase, "Select phase disabled");
        selectUICanvas.SetActive(false);
        selectInputManager.enabled = false;
        selectionInfo.ClearSelected();
    }
}