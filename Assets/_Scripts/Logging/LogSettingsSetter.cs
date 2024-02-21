using System;
using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;

public class LogSettingsSetter : MonoBehaviour
{
    [SerializeField] private LogSettings logSettings;

    private DebugConsoleOutput debugConsoleOutput_;
    
    private void Start()
    {
        XLogger.isLoggingEnabled = logSettings.enable;
        
        debugConsoleOutput_ = FindObjectOfType<DebugConsoleOutput>();
        if (debugConsoleOutput_ == null)
        {
            XLogger.LogWarning(Category.DebugConsole, "DebugConsoleOutput is not found!");
            return;
        }

        if (logSettings.useConsoleOutput)
            XLogger.AddLogOutput(debugConsoleOutput_);
    }
}