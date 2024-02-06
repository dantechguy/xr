using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RotationSlider : MonoBehaviour
{
    [SerializeField] private SelectedTransformer transformer_;
    private Slider slider_;

    private void OnEnable()
    {
        transformer_.GetSelectionInfo().onSelected += OnSelected;
    }

    private void OnDisable()
    {
        transformer_.GetSelectionInfo().onSelected -= OnSelected;
    }

    private void OnSelected()
    {
        ARSpawnedSelectable selected = transformer_.GetSelectionInfo().GetSelected();
        if (selected == null) return;
        slider_.value = selected.GetComponent<ARSpawnedTransformable>().GetRotationAngle();
    }

    void Start()
    {
        slider_ = GetComponent<Slider>();
    }

    public void ApplyRotation()
    {
        transformer_.ApplyRotation(slider_.value);
    }
}