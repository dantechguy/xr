using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ARPlaneSelectable : MonoBehaviour
{
    [SerializeField] private PlaneSelectionInfo selectionInfo;
    [SerializeField] private Material selectedMaterial;

    private Material defaultMaterial_;
    private MeshRenderer meshRenderer_;

    private void Start()
    {
        meshRenderer_ = GetComponent<MeshRenderer>();
        defaultMaterial_ = meshRenderer_.material;
    }

    public void OnSelect()
    {
        meshRenderer_.material = selectedMaterial;
        selectionInfo.SetSelected(this);
    }

    public void OnDeselect()
    {
        meshRenderer_.material = defaultMaterial_;
    }
}