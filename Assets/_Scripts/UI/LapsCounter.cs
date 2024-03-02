using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LapsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private Image icon;
    [SerializeField] private Image border;
    [Header("Colors")]
    [SerializeField] private Color lastLapColor;
    [SerializeField] private Color finishedColor;
    [SerializeField] private Color defaultColor;

    private void OnEnable()
    {
        GameManager.onLapCompleted += UpdateLapsCounter;
    }

    private void OnDisable()
    {
        GameManager.onLapCompleted -= UpdateLapsCounter;
    }

    private void UpdateLapsCounter(int _laps, int _totalLaps)
    {
        var remain = _totalLaps - _laps;
        counterText.text = $"{remain}  left";
        Color color = remain switch
        {
            1 => lastLapColor,
            0 => finishedColor,
            _ => defaultColor
        };
        counterText.color = color;
        icon.color = color;
        border.color = color;
    }
}