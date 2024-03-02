using UnityEngine;
using UnityEngine.XR.ARFoundation;

[CreateAssetMenu(fileName = "GeneralSettings", menuName = "Settings/GeneralSettings", order = 1)]
public class GeneralSettings : ScriptableObject
{
    public float planeTransparency;
    public bool countDown;
    public int laps;

    public void ApplyTransparencyToPlane(ARPlane _plane)
    {
        var meshRenderer = _plane.GetComponent<MeshRenderer>();
        Material material = meshRenderer.material;
        Color color = material.color;
        color.a = planeTransparency;
        material.color = color;
            
        var lineRenderer = _plane.GetComponent<LineRenderer>();
        Material lineMaterial = lineRenderer.material;
        Color lineColor = lineMaterial.color;
        lineColor.a = planeTransparency;
        lineMaterial.color = lineColor;
    }
}