using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;

public class DebugConsoleOutput : MonoBehaviour, ILogOutput
{
    private DebugConsoleManager debugConsoleManager_;
    
    void Start()
    {
        debugConsoleManager_ = GetComponent<DebugConsoleManager>();
        if (debugConsoleManager_ == null)
        {
            Debug.LogError("DebugConsoleOutput requires a DebugConsoleManager component.");
        }
    }

    public void Log(object _message, Category _category = Category.Others)
    {
        debugConsoleManager_.AddText(
            XLogger.FormatMessageWithCategory(XLogger.InfoColor, _category, _message));
    }

    public void LogWarning(object _message, Category _category = Category.Others)
    {
        debugConsoleManager_.AddText(
            XLogger.FormatMessageWithCategory(XLogger.WarningColor, _category, _message));
    }

    public void LogError(object _message, Category _category = Category.Others)
    {
        debugConsoleManager_.AddText(
            XLogger.FormatMessageWithCategory(XLogger.ErrorColor, _category, _message));
    }
}
