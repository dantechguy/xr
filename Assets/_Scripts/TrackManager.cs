using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private TrackGenerator trackGenerator;
    [SerializeField] private SpawnSettings spawnSettings;
    
    public event Action onTrackGenerated;
    
    public void GenerateTrack()
    {
        var wayPoints = new List<Waypoint>();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Waypoint waypoint))
                wayPoints.Add(waypoint);
        }
        
        wayPoints.Sort((_a, _b) => _a.position.CompareTo(_b.position));
        for (int i = 0; i < wayPoints.Count; i++)
            wayPoints[i].SetOrderInTrack(i);
        
        var transforms = wayPoints.ConvertAll(_waypoint => _waypoint.transform);
        
        trackGenerator.GenerateTrack(transforms, spawnSettings.trackScale, false);
        
        onTrackGenerated?.Invoke();
    }
}
