using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class TrackGenerator : MonoBehaviour
{
    public PathCreator pathCreator;
    public RoadMeshCreator roadMeshCreator;
    public List<Transform> checkpoints;
    public float trackScale;
    public float trackWidth;
    public bool isClosed;

    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> points = checkpoints.ConvertAll(c => c.position);
        BezierPath bezierPath = new BezierPath(points: points, isClosed: isClosed, space: PathSpace.xyz);
        pathCreator.bezierPath = bezierPath;

        roadMeshCreator.roadWidth = trackWidth;
        roadMeshCreator.thickness = trackScale * 0.1f;

    }
}
