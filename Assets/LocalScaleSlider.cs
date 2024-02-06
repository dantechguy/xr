using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LocalScaleSlider : MonoBehaviour
{
    [SerializeField] private SelectedTransformer transformer_;
    private Slider slider_;

    private void OnEnable()
    {
        transformer_.GetSelectionInfo().onSelected += OnSelected;
    }

    private void OnSelected()
    {
        slider_.value = transformer_.GetSelectionInfo().GetSelected()?.GetComponent<ARSpawnedTransformable>()
            ?.GetLocalScale() ?? 1;
    }

    void Start()
    {
        slider_ = GetComponent<Slider>();
    }

    public void ApplyLocalScale()
    {
        transformer_.ApplyLocalScale(slider_.value);
    }

}