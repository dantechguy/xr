using System;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GeneralSettings generalSettings;

    [FormerlySerializedAs("trackManager_")] [SerializeField]
    private TrackManager trackManager;

    [SerializeField] private Timer timer;
    [SerializeField] private CarReplay carReplay;

    private List<Waypoint> wayPoints_ = new List<Waypoint>();
    private int nextWaypoint_;
    private int totalLaps_;
    private int completedLaps_;

    public static Action<int, int> onLapCompleted;

    public void Enable()
    {
        totalLaps_ = generalSettings.laps;
        completedLaps_ = 0;
        onLapCompleted?.Invoke(completedLaps_, totalLaps_);

        wayPoints_ = trackManager.GetWayPoints();

        if (wayPoints_.Count == 0)
            XLogger.LogWarning(Category.GameManager, "No waypoints found");
        else
            SetNextWayPoint(0);

        foreach (var waypoint in wayPoints_)
            waypoint.Init(this);

    }

    public void StartTimer(PrometeoCarController _car)
    {
        timer.StartTimer();
        
        carReplay.objectReplay.trackedObject = _car.transform;
        carReplay.RaceStart();
    }

    public void Disable()
    {
        foreach (var waypoint in wayPoints_)
            waypoint.SetToNotCompleted();

        timer.ResetTimer();
        
        carReplay.RaceCancel();
    }

    public void SetNextWayPoint(int _index)
    {
        if (_index > wayPoints_.Count - 1)
        {
            LapFinished();
            return;
        }

        nextWaypoint_ = _index;
        for (int i = 0; i < _index; i++)
            wayPoints_[i].SetToCompleted();

        wayPoints_[nextWaypoint_].SetToNextWaypoint();

        for (int i = nextWaypoint_ + 1; i < wayPoints_.Count; i++)
            wayPoints_[i].SetToNotCompleted();
    }

    private void LapFinished()
    {
        XLogger.Log(Category.GameManager, "Lap Finished");
        completedLaps_++;
        if (completedLaps_ <= totalLaps_)
            onLapCompleted?.Invoke(completedLaps_, totalLaps_);
        if (completedLaps_ >= totalLaps_)
        {
            XLogger.Log(Category.GameManager, "Race Finished");
            timer.StopTimer();
            carReplay.RaceFinish();
            return;
        }

        SetNextWayPoint(0);
    }

    public int GetNextWaypointIndex()
    {
        return nextWaypoint_;
    }
}