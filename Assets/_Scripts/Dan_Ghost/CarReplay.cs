using UnityEngine;
using System;

public class CarReplay : MonoBehaviour
{

    public ObjectReplay objectReplay;

    private void Start()
    {
        objectReplay = GetComponent<ObjectReplay>();
    }

    public void RaceStart()
    {
        objectReplay.StartTracking();
        objectReplay.StartReplaying();
    }

    public void RaceCancel()
    {
        objectReplay.StopTracking();
        objectReplay.StopReplaying();
    }

    public void RaceFinish()
    {
        objectReplay.StopTracking();
        objectReplay.StopReplaying();

        if (objectReplay.trackingTime < objectReplay.replayTime || objectReplay.replayTime == TimeSpan.Zero)
        {
            objectReplay.SaveTracking();
        }
    }

    public void ResetGhost()
    {
        objectReplay.StopTracking();
        objectReplay.StopReplaying();
        objectReplay.ClearReplay();
    }
}