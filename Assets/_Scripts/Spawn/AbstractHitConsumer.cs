using UnityEngine;
using UnityEngine.XR.ARFoundation;

public abstract class AbstractHitConsumer : MonoBehaviour
{

    public abstract void OnHit(ARRaycastHit _hit);

}
