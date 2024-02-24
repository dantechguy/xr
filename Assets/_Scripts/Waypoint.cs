using System;
using System.Collections;
using System.Collections.Generic;
using Logging;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int position;
    
    [Header("Materials")]
    [SerializeField] private MeshRenderer targetMesh;
    [SerializeField] private Material completedMaterial;
    [SerializeField] private Material uncompletedMaterial;
    [SerializeField] private Material nextWaypointMaterial;

    private int orderInTrack_;
    private GameManager gameManager_;

    public void Init(GameManager _gameManager)
    {
        gameManager_ = _gameManager;
    }
    
    public void SetOrderInTrack(int _orderInTrack)
    {
        orderInTrack_ = _orderInTrack;
    }
    
    public int GetOrderInTrack()
    {
        return orderInTrack_;
    }

    public void SetToCompleted()
    {
        targetMesh.material = completedMaterial;
    }

    public void SetToNextWaypoint()
    {
        targetMesh.material = nextWaypointMaterial;
    }

    public void SetToNotCompleted()
    {
        targetMesh.material = uncompletedMaterial;
    }

    public void OnCarPassThrough()
    {
        if (gameManager_ == null)
        {
            XLogger.LogWarning(Category.GameManager, "GameManager is null");
            return;
        }
        if (gameManager_.GetNextWaypointIndex() == orderInTrack_)
        {
            XLogger.Log(Category.GameManager, "Car passed through the correct waypoint");
            gameManager_.SetNextWayPoint(orderInTrack_+1);
        }
        else
        {
            XLogger.LogWarning(Category.GameManager, "Car passed through the wrong waypoint");
        }
    }
}