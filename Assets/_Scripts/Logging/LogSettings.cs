using UnityEngine;

[CreateAssetMenu(fileName = "LogSettings", menuName = "LogSettings")]
public class LogSettings : ScriptableObject
{
    public bool enable;
    public bool useConsoleOutput;
}