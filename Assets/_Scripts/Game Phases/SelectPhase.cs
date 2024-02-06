using Logging;
using UnityEngine;

public class SelectPhase : MonoBehaviour, GamePhaseManger.IGamePhase
{
    [SerializeField] private SelectionInfo selectionInfo_;
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
        selectionInfo_.ClearSelected();
    }
}