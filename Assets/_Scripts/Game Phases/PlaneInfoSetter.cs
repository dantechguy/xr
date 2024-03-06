using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

public class PlaneInfoSetter : MonoBehaviour
{
    [SerializeField] private PlaneSelectionInfo selectionInfo;
    
    [SerializeField] private TextMeshProUGUI orientation;
    [FormerlySerializedAs("isGround")] [SerializeField] private TextMeshProUGUI isGroundText;

    private void OnEnable()
    {
        selectionInfo.onGroundColliderSet += UpdateInfo;
        UpdateInfo();
    }
    
    private void OnDisable()
    {
        selectionInfo.onGroundColliderSet -= UpdateInfo;
    }

    private void UpdateInfo()
    {
        ARPlaneSelectable selected = selectionInfo.GetSelected();
        if (selected != null)
        {
            var plane = selected.GetComponent<ARPlane>();
            orientation.text = plane.alignment.ToString();
            
            var isGround = selectionInfo.IsGround(selected);
            isGroundText.text = isGround ? "Is Ground" : "Not Ground";
        }
        else
        {
            orientation.text = "No plane selected";
            isGroundText.text = "No plane selected";
        }
    }
}