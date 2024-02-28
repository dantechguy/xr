using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LightEstimationUI : MonoBehaviour
{
    [SerializeField] private LightEstimator lightEstimator;
    [Header("UI")] [SerializeField] private TextMeshProUGUI brightnessText;
    [SerializeField] private TextMeshProUGUI tempText;
    [SerializeField] private TextMeshProUGUI correctionText;
    [SerializeField] private TextMeshProUGUI directionText;
    [SerializeField] private TextMeshProUGUI colorText;
    [SerializeField] private TextMeshProUGUI intensityText;

    private const string Placeholder = "N/A";

    public void ToggleLightEstimator()
    {
        lightEstimator.gameObject.SetActive(!lightEstimator.gameObject.activeSelf);
    }

    private void Update()
    {
        if (!lightEstimator.gameObject.activeSelf) return;
        
        brightnessText.text =
            $"Brightness: {(lightEstimator.brightness.HasValue ? lightEstimator.brightness.Value.ToString() : Placeholder)}";
        tempText.text =
            $"Temperature: {(lightEstimator.colorTemperature.HasValue ? lightEstimator.colorTemperature.Value.ToString() : Placeholder)}";
        correctionText.text =
            $"Correction: {(lightEstimator.colorCorrection.HasValue ? lightEstimator.colorCorrection.Value.ToString() : Placeholder)}";
        directionText.text =
            $"Direction: {(lightEstimator.mainLightDirection.HasValue ? lightEstimator.mainLightDirection.Value.ToString() : Placeholder)}";
        colorText.text =
            $"Color: {(lightEstimator.mainLightColor.HasValue ? lightEstimator.mainLightColor.Value.ToString() : Placeholder)}";
        colorText.color = lightEstimator.mainLightColor.HasValue ? lightEstimator.mainLightColor.Value : Color.white;
        intensityText.text =
            $"Intensity: {(lightEstimator.mainLightIntensityLumens.HasValue ? lightEstimator.mainLightIntensityLumens.Value.ToString() : Placeholder)}";
    }
}