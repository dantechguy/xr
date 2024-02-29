using System;
using Logging;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RotationSlider : MonoBehaviour
{
    [FormerlySerializedAs("transformer_")][SerializeField] private SelectedTransformer transformer;
    private Slider slider_;
    private SelectionInfo selectionInfo_;

    private void OnEnable()
    {
        if (slider_ == null)
            slider_ = GetComponent<Slider>();

        if (selectionInfo_ == null)
            selectionInfo_ = transformer.GetSelectionInfo();

        selectionInfo_.onSelected += OnSelected;

        OnSelected();

        XLogger.Log(Category.UI, "RotationSlider enabled");
    }

    private void OnDisable()
    {
        selectionInfo_.onSelected -= OnSelected;

        XLogger.Log(Category.UI, "RotationSlider disabled");
    }

    private void OnSelected()
    {
        ARSpawnedSelectable selected = selectionInfo_.GetSelected();
        if (selected == null) return;
        var angle = selected.GetComponent<ARSpawnedTransformable>().GetRotationAngle();
        XLogger.Log(Category.UI, $"RotationSlider angle: {angle}");
        slider_.value = angle;
    }

    public void ApplyRotation()
    {
        transformer.ApplyRotation(slider_.value);
    }
}