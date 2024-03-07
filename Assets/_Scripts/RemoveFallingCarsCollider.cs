using Logging;
using UnityEngine;

public class RemoveFallingCarsCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider _other)
    {
        XLogger.Log(Category.GamePhase, $"Detected { _other.name } falling off");
        var car = _other.GetComponentInParent<PrometeoCarController>();
        if (car != null)
        {
            XLogger.Log(Category.GamePhase, "Car fell off the plane, destroyed");
            Destroy(car.gameObject);
        }
    }
}