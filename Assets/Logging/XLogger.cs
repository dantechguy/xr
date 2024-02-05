using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Logging
{
    public enum Category
    {
        Others,
        Numeric,
        Audio,
        Animation,
        Physics,
        Scene,
        Achievement,
        UI,
        DebugConsole,
        Settings,
        AR,
        Spawn,
        Input,
        GamePhase
    }

    public interface ILogOutput
    {
        void Log(object _message, Category _category = Category.Others);
        void LogWarning(object _message, Category _category = Category.Others);
        void LogError(object _message, Category _category = Category.Others);
    }

    public class DebugLogOutput : ILogOutput
    {
        public void Log(object _message, Category _category = Category.Others)
        {
            Debug.Log(XLogger.FormatMessageWithCategory(XLogger.InfoColor, _category, _message));
        }

        public void LogWarning(object _message, Category _category = Category.Others)
        {
            Debug.LogWarning(XLogger.FormatMessageWithCategory(XLogger.WarningColor, _category, _message));
        }

        public void LogError(object _message, Category _category = Category.Others)
        {
            Debug.LogError(XLogger.FormatMessageWithCategory(XLogger.ErrorColor, _category, _message));
        }
    }

    public static class XLogger
    {
        private static List<ILogOutput> logOutputs_ = new() { new DebugLogOutput() };

        public static bool isLoggingEnabled { get; set; } = true;

        public const string InfoColor = nameof(Color.white);
        public const string WarningColor = nameof(Color.yellow);
        public const string ErrorColor = nameof(Color.red);
        
        public static void AddLogOutput(ILogOutput _logOutput)
        {
            logOutputs_.Add(_logOutput);
        }

        // [Conditional("DEBUG")]
        public static void Log(object _message)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.Log(_message);
            }
        }

        // [Conditional("DEBUG")]
        public static void Log(Category _category, object _message)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.Log(_message, _category);
            }
        }

        // [Conditional("DEBUG")]
        public static void LogWarning(object _message)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.LogWarning(_message);
            }
        }

        // [Conditional("DEBUG")]
        public static void LogWarning(Category _category, object _message)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.LogWarning(_message, _category);
            }
        }

        // [Conditional("DEBUG")]
        public static void LogError(object _message)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.LogError(_message);
            }
        }

        // [Conditional("DEBUG")]
        public static void LogError(Category _category, object _message)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.LogError(_message, _category);
            }
        }

        // [Conditional("DEBUG")]
        public static void LogException(Exception _exception)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.LogError(_exception.Message);
            }
        }

        // [Conditional("DEBUG")]
        public static void LogException(Category _category, Exception _exception)
        {
            if (!isLoggingEnabled) return;
            foreach (ILogOutput logOutput in logOutputs_)
            {
                logOutput.LogError(_exception.Message, _category);
            }
        }

        public static string FormatMessage(string _color, object _message)
        {
            return $"<color={_color}>{_message}</color>";
        }

        public static string FormatMessageWithCategory(string _color, Category _category, object _message)
        {
            return $"<color={_color}><b>[{_category.ToString()}]</b> {_message}</color>";
        }
    }
}