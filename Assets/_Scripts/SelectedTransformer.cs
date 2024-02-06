using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(GameObjectMover))]
public class SelectedTransformer : MonoBehaviour
{
    [SerializeField] private SelectionInfo selectionInfo;
    
    private GamePhaseManger phaseManger_;

    private GameObjectMover mover_;

    private void Start()
    {
        mover_ = GetComponent<GameObjectMover>();
        phaseManger_ = FindObjectOfType<GamePhaseManger>();
        if (phaseManger_ == null)
        {
            XLogger.LogWarning(Category.GamePhase, "Game phase manager not found");
        }
    }

    public void MoveToHit(ARRaycastHit _hit)
    {
        if (selectionInfo.IsJustSelected()) return; // prevent the selection tap from moving the object
        mover_.MoveObject(selectionInfo.GetSelected()?.gameObject, _hit);
    }

    public void Delete()
    {
        Destroy(selectionInfo.GetSelected()?.gameObject);
        selectionInfo.ClearSelected();
        phaseManger_.SwitchPhase(GamePhaseManger.GamePhase.Spawn);
    }
}