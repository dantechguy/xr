using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SelectedPlaneTransformer : MonoBehaviour
{
    [SerializeField] private PlaneSelectionInfo selectionInfo;

    public void Delete()
    {
        ARPlaneSelectable selected = selectionInfo.GetSelected();
        if (selected != null)
        {
            selected.gameObject.SetActive(false);
        }
        selectionInfo.ClearSelected();
        GamePhaseManger.instance.SwitchPhase(GamePhaseManger.GamePhase.Spawn);
    }
}