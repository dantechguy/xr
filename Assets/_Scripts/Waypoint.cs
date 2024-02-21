using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int position;

    private int orderInTrack;
    
    public void SetOrderInTrack(int _orderInTrack)
    {
        orderInTrack = _orderInTrack;
    }
    
    public int GetOrderInTrack()
    {
        return orderInTrack;
    }
}
