using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private TrackGenerator trackGenerator;
    [SerializeField] private SpawnSettings spawnSettings;
    [SerializeField] private ARAnchorManager anchorManager;
    private List<Waypoint> wayPoints_ = new List<Waypoint>();

    public List<Waypoint> GetWayPoints()
    {
        return wayPoints_;
    }

    public event Action onTrackGenerated;

    public void GenerateTrack()
    {
        wayPoints_.Clear();
        foreach (ARAnchor child in anchorManager.trackables)
        {
            var waypoint = child.GetComponentInChildren<Waypoint>();
            if (waypoint != null && waypoint.enabled)
                wayPoints_.Add(waypoint);
        }

        if (wayPoints_.Count < 2)
        {
            trackGenerator.ClearTrack();
            return;
        }

        wayPoints_.Sort((_a, _b) => _a.position.CompareTo(_b.position));
        for (int i = 0; i < wayPoints_.Count; i++)
            wayPoints_[i].SetOrderInTrack(i);

        var transforms = wayPoints_.ConvertAll(_waypoint => _waypoint.transform);

        trackGenerator.GenerateTrack(transforms, spawnSettings.trackScale, spawnSettings.isClosed);

        onTrackGenerated?.Invoke();
    }
}