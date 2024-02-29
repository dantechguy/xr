using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private TrackGenerator trackGenerator;
    [SerializeField] private SpawnSettings spawnSettings;
    private List<Waypoint> wayPoints_ = new List<Waypoint>();

    public List<Waypoint> GetWayPoints()
    {
        return wayPoints_;
    }

    public event Action onTrackGenerated;

    public void GenerateTrack()
    {
        wayPoints_.Clear();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Waypoint waypoint))
                wayPoints_.Add(waypoint);
        }

        if (wayPoints_.Count < 2)
            return;

        wayPoints_.Sort((_a, _b) => _a.position.CompareTo(_b.position));
        for (int i = 0; i < wayPoints_.Count; i++)
            wayPoints_[i].SetOrderInTrack(i);

        var transforms = wayPoints_.ConvertAll(_waypoint => _waypoint.transform);

        trackGenerator.GenerateTrack(transforms, spawnSettings.trackScale, spawnSettings.isClosed);

        onTrackGenerated?.Invoke();
    }
}