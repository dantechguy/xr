using System;
using System.Collections;
using System.Collections.Generic;
using Logging;
using TMPro;
using UnityEngine;

public class ToggleTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private string activeText;
    [SerializeField] private string inactiveText;
    [SerializeField] private bool defaultActive;

    private TextMeshProUGUI text_;

    private void Start()
    {
        text_ = GetComponentInChildren<TextMeshProUGUI>();
        if (target == null)
        {
            XLogger.LogWarning("Target not set for ToggleTarget");
        }
        target.SetActive(defaultActive);
        if (text_ != null)
            text_.text = defaultActive ? activeText : inactiveText;
    }

    public void ToggleActive()
    {
        target.SetActive(!target.activeSelf);
        if (text_ != null)
            text_.text = target.activeSelf ? activeText : inactiveText;
    }
    
    
}
