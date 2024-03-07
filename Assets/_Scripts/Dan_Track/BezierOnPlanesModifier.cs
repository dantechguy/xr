using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public static class BezierOnPlanesModifier
{
    // TODO: return void
    public static void ModifyBezierToStopClippingThroughARPlanes(BezierPath bezierPath, List<ARPlane> planes, float distance, int maxIterations = 30)
    {
        List<ARPlane> planesToCheck = GetOnlyPlanesFacingUpward(planes);
        
        for (int i=0; i<maxIterations; i++)
        {
            List<Tuple<int, float, Vector3, ARPlane>> pointsOnBezierPathBelowAPlaneWithinDistance = GetPointsOnBezierBelowPlanesWithinDistance(bezierPath, planesToCheck, distance);

            if (pointsOnBezierPathBelowAPlaneWithinDistance.Count == 0) break;
            
            InsertAnchorIntoBezierToMovePointUpToPlane(bezierPath, pointsOnBezierPathBelowAPlaneWithinDistance.First());
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
                spacing: distance
                // Optimisation relies on that all bezier curve handles are horizontal
                // shouldTraverseSegment: segmentPoints => !plane.infinitePlane.SameSide(segmentPoints[0], segmentPoints[3])
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

    private static void InsertAnchorIntoBezierToMovePointUpToPlane(BezierPath bezierPath, Tuple<int, float, Vector3, ARPlane> point)
    {
        int segmentIndex = point.Item1;
        float t = point.Item2;
        Vector3 pointOnBezier = point.Item3;
        ARPlane plane = point.Item4;

        Vector3 pointOnPlane = plane.infinitePlane.ClosestPointOnPlane(pointOnBezier) + Vector3.up * 0.1f;

        bezierPath.SplitSegment(pointOnPlane, segmentIndex, t);
        int newAnchorIndex = (segmentIndex + 1) * 3;
        MakeControlHandlesHorizontal(bezierPath, newAnchorIndex);
        
    }

    private static void MakeControlHandlesHorizontal(BezierPath bezierPath, int anchorIndex)
    {
        Vector3 anchor = bezierPath.GetPoint(anchorIndex);
        Vector3 handle1 = bezierPath.GetPoint(anchorIndex - 1);
        Vector3 handle2 = bezierPath.GetPoint(anchorIndex + 1);
        
        bezierPath.MovePoint(anchorIndex-1, new Vector3(handle1.x, anchor.y, handle1.z));
        bezierPath.MovePoint(anchorIndex+1, new Vector3(handle2.x, anchor.y, handle2.z));
    }
    
    private static bool IsPointBelowPlaneWithinDistance(ARPlane plane, Vector3 point, float distance)
    {
        float distancePointIsBelowPlane = -plane.infinitePlane.GetDistanceToPoint(point);
        Vector2 pointProjectedOntoPlane = VectorMaths.ProjectPointToPlaneSpace(point, plane);

        return (0 < distancePointIsBelowPlane && distancePointIsBelowPlane < distance)
               && (VectorMaths.IsPointInPolygon(polygon: plane.boundary.ToArray(), point: pointProjectedOntoPlane));
    }
}