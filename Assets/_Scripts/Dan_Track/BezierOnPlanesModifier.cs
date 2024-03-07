using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public static class BezierOnPlanesModifier
{
    public static void ModifyBezierToStopClippingThroughARPlanes(BezierPath bezierPath, List<ARPlane> planes, float distance)
    {
        List<ARPlane> planesToCheck = GetOnlyPlanesFacingUpward(planes);

        for (int i=0; i<30; i++)
        {

            List<Tuple<int, float, Vector3, ARPlane>> pointsOnBezierPathBelowAPlaneWithinDistance = GetPointsOnBezierBelowPlanesWithinDistance(bezierPath, planesToCheck, distance);

            if (pointsOnBezierPathBelowAPlaneWithinDistance.Count == 0) break;

            InsertAnchorsIntoBezierToMovePointsUpToPlane(bezierPath, pointsOnBezierPathBelowAPlaneWithinDistance);
        }
    }

    private static List<ARPlane> GetOnlyPlanesFacingUpward(List<ARPlane> planes)
    {
        return planes.Where(plane => plane.alignment == PlaneAlignment.HorizontalUp).ToList();
    }

    private static List<Tuple<int, float, Vector3, ARPlane>> GetPointsOnBezierBelowPlanesWithinDistance(BezierPath bezierPath, List<ARPlane> planes, float distance)
    {
        List<Tuple<int, float, Vector3, ARPlane>> resultPoints = new List<Tuple<int, float, Vector3, ARPlane>>();

        foreach (ARPlane plane in planes)
        {
            IEnumerable<Tuple<int, float, Vector3>> pointsOnBezier = BezierEnumerator.PointsAlongBezierPath(
                bezierPath: bezierPath, 
                spacing: distance, 
                // Optimisation relies on that all bezier curve handles are horizontal
                shouldTraverseSegment: segmentPoints => !plane.infinitePlane.SameSide(segmentPoints[0], segmentPoints[3])
            );

            resultPoints.AddRange(
                pointsOnBezier.Select(
                    // Add [ARPlane]
                    point => new Tuple<int, float, Vector3, ARPlane>(point.Item1, point.Item2, point.Item3, plane)
                ).Where(
                    // Only choose points which are underneath the plane within [distance]
                    point => IsPointBelowPlaneWithinDistance(point.Item4, point.Item3, distance)
                )
            );
        }

        return resultPoints;
    }

    private static void InsertAnchorsIntoBezierToMovePointsUpToPlane(BezierPath bezierPath, List<Tuple<int, float, Vector3, ARPlane>> points)
    {
        // Iterate through points
        // Move point to plane (i.e. move up to match y-coord)
        // split segment on bezier curve at this point, and move point to new point position
        
        foreach (Tuple<int, float, Vector3, ARPlane> point in points)
        {
            int segmentIndex = point.Item1;
            float t = point.Item2;
            Vector3 pointOnBezier = point.Item3;
            ARPlane plane = point.Item4;

            Vector3 pointOnPlane = plane.infinitePlane.ClosestPointOnPlane(pointOnBezier);

            bezierPath.SplitSegment(pointOnPlane, segmentIndex, t);
        }
    }

    private static bool IsPointBelowPlaneWithinDistance(ARPlane plane, Vector3 point, float distance)
    {
        float distancePointIsBelowPlane = -plane.infinitePlane.GetDistanceToPoint(point);
        Vector2 pointProjectedOntoPlane = VectorMaths.ProjectPointToPlaneSpace(point, plane);

        return (0 < distancePointIsBelowPlane && distancePointIsBelowPlane < distance)
               && (VectorMaths.IsPointInPolygon(polygon: plane.boundary.ToArray(), point: pointProjectedOntoPlane));
    }

}