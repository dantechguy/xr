#nullable enable
using System;
using PathCreation;
using PathCreation.Utility;
using System.Collections.Generic;
using UnityEngine;

public static class BezierEnumerator
{
    public static IEnumerable<Tuple<int, float, Vector3>> PointsAlongBezierPath(BezierPath bezierPath, float spacing, Func<Vector3[], bool>? shouldTraverseSegment = null)
    {
        for (int segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
        {
            Vector3[] segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);
            if (shouldTraverseSegment != null && !shouldTraverseSegment(segmentPoints)) continue;

            float segmentLength = CubicBezierUtility.EstimateCurveLength(segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3]);
            float step = spacing / segmentLength;

            for (float t=0; t<1; t+=step)
            {
                Vector3 pointOnBezier = CubicBezierUtility.EvaluateCurve(segmentPoints, t);
                yield return new Tuple<int, float, Vector3>(segmentIndex, t, pointOnBezier);
            }
        }
    }
}