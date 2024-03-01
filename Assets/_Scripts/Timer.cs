using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float time_;
    private bool isRunning_ = false;

    public void ResetTimer()
    {
        time_ = 0;
        timerText.text = GetTimeString();
        isRunning_ = false;
    }

    public void StartTimer()
    {
        ResetTimer();
        isRunning_ = true;
    }

    private void Update()
    {
        if (!isRunning_) return;
        time_ += Time.deltaTime;
        timerText.text = GetTimeString();
    }

    private String GetTimeString()
    {
        var minutes = (int)time_ / 60;
        var seconds = (int)time_ % 60;
        var centiseconds = (int)(time_ * 100) % 100;
        return $"{minutes:0}:{seconds:00}:{centiseconds:00}";
    }

    public void StopTimer()
    {
        isRunning_ = false;
    }
}