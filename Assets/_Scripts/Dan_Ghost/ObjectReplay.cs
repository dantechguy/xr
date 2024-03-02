using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections.Generic;

// Your code goes here


public class ObjectReplay : MonoBehaviour
{
    public Transform trackedObject;
    public GameObject replayPrefab;
    public bool keepReplayObjectWhenFinished = false;
    private GameObject replayObject;

    private bool isTracking = false;
    private Stopwatch trackingStopwatch = new Stopwatch();
    private List<Tuple<TimeSpan, SavedTransform>> trackingTransforms = new List<Tuple<TimeSpan, SavedTransform>>();


    private bool isReplaying = false;
    private int replayIndex = 0;
    private Stopwatch replayStopwatch = new Stopwatch();
    private List<Tuple<TimeSpan, SavedTransform>> replayTransforms = new List<Tuple<TimeSpan, SavedTransform>>();



    // Update is called once per frame
    void Update()
    {
        if (isTracking)
        {
            trackingTransforms.Add(new Tuple<TimeSpan, SavedTransform>(
                trackingStopwatch.Elapsed,
                SavedTransform.FromTransform(trackedObject.transform)
            ));
        }

        if (isReplaying)
        {
            while (replayIndex < replayTransforms.Count-1 && replayTransforms[replayIndex].Item1 <= replayStopwatch.Elapsed)
            {
                replayIndex++;
            }

                

            replayTransforms[replayIndex].Item2.ApplyToTransform(replayObject.transform);

            if (replayIndex == replayTransforms.Count-1 && !keepReplayObjectWhenFinished)
            {
                StopReplaying();
            }
        }
    }

    public void StartTracking()
    {
        isTracking = true;
        trackingStopwatch.Restart();
        trackingTransforms = new List<Tuple<TimeSpan, SavedTransform>>();
    }

    public void StopTracking()
    {
        isTracking = false;
        trackingStopwatch.Stop();
    }

    public void SaveTracking()
    {
        replayTransforms = new List<Tuple<TimeSpan, SavedTransform>>(trackingTransforms);
    }

    public TimeSpan trackingTime
    {
        get
        {
            if (trackingTransforms.Count == 0) return TimeSpan.Zero;
            return trackingTransforms[trackingTransforms.Count - 1].Item1;
        }
    }

    public void StartReplaying()
    {
        if (replayTransforms.Count == 0) return;

        StopReplaying();
        isReplaying = true;
        replayIndex = 0;
        replayStopwatch.Restart();
        // TODO: Check this doesn't flicker between creation and being moved to first position on next update call
        replayObject = Instantiate(replayPrefab);
    }

    public void StopReplaying()
    {
        isReplaying = false;
        replayStopwatch.Stop();
        Destroy(replayObject);
    }

    public void ClearReplay()
    {
        StopReplaying();
        replayTransforms = new List<Tuple<TimeSpan, SavedTransform>>();
    }

    public TimeSpan replayTime
    {
        get
        {
            if (replayTransforms.Count == 0) return TimeSpan.Zero;
            return replayTransforms[replayTransforms.Count - 1].Item1;
        }
    }
}
