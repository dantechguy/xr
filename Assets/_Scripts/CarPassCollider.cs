using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CarPassCollider : MonoBehaviour
{
    private Waypoint waypoint_;

    private void Awake()
    {
        waypoint_ = GetComponentInParent<Waypoint>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.CompareTag("Player"))
        {
            waypoint_.OnCarPassThrough();
        }
    }
}