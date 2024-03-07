using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CarPassCollider : MonoBehaviour
{
    private Waypoint waypoint_;

    private void Awake()
    {
        waypoint_ = GetComponentInParent<Waypoint>();
    }
    
    // Don't use because it doesn't give contact positions
    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.CompareTag("Player"))
        {
            waypoint_.OnCarPassThrough(_other.ClosestPointOnBounds(transform.position));
        }
    }

    // private void OnCollisionEnter(Collision _other)
    // {
    //     if (_other.gameObject.CompareTag("Player"))
    //     {
    //         waypoint_.OnCarPassThrough(_other.contacts[0].point);
    //     }
    // }
}