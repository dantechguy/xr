using UnityEngine;
using UnityEngine.XR.ARFoundation;

public static class VectorMaths
{
    public static bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
    {
        bool isInside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }
    
    public static Vector2 ProjectPointToPlaneSpace(Vector3 point, ARPlane plane)
    {
        Vector3 localPoint = plane.transform.InverseTransformPoint(point);
    
        Vector2 planePoint = new Vector2(localPoint.x, localPoint.z);
    
        return planePoint;
    }
}