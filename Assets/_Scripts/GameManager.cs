using System.Collections.Generic;
using Logging;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TrackManager trackManager_;
    
    private List<Waypoint> wayPoints_ = new List<Waypoint>();
    private int nextWaypoint_;

    public void Enable()
    {
        wayPoints_ = trackManager_.GetWayPoints();
        
        if (wayPoints_.Count == 0)
            XLogger.LogWarning(Category.GameManager, "No waypoints found");
        else
            SetNextWayPoint(0);

        foreach (var waypoint in wayPoints_)
            waypoint.Init(this);
    }

    public void Disable()
    {
        foreach (var waypoint in wayPoints_)
            waypoint.SetToNotCompleted();
    }

    public void SetNextWayPoint(int _index)
    {
        if (_index > wayPoints_.Count - 1)
        {
            XLogger.Log(Category.GameManager, "Lap Finished");
            // for now, just restart the lap
            SetNextWayPoint(0);
            return;
        }
        
        nextWaypoint_ = _index;
        for (int i = 0; i < _index; i++)
            wayPoints_[i].SetToCompleted();

        wayPoints_[nextWaypoint_].SetToNextWaypoint();
        
        for (int i = nextWaypoint_+1; i < wayPoints_.Count; i++)
            wayPoints_[i].SetToNotCompleted();

    }

    public int GetNextWaypointIndex()
    {
        return nextWaypoint_;
    }
}