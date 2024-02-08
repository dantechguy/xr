using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TrackScaleSlider : MonoBehaviour
{
    [SerializeField] private SpawnSettings spawnSettings;

    private Slider slider_;

    private void Start()
    {
        slider_ = GetComponent<Slider>();
        slider_.value = spawnSettings.trackScale;
    }

    private void OnEnable()
    {
        if (slider_ != null)
            slider_.value = spawnSettings.trackScale;
    }

    public void SetTrackScale()
    {
        spawnSettings.trackScale = slider_.value;
    }
}
