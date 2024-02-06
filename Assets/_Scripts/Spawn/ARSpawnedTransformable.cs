using UnityEngine;

public class ARSpawnedTransformable : MonoBehaviour
{
    private Vector3 localScale_ = Vector3.one;
    private float globalScale_ = 1.0f;

    public void ApplyGlobalScale(float _globalScale)
    {
        globalScale_ = _globalScale;
        transform.localScale = _globalScale * localScale_;
    }

    public void ApplyLocalScale(float _localScale)
    {
        localScale_ = _localScale * Vector3.one;
        transform.localScale = globalScale_ * localScale_;
    }

    public float GetLocalScale()
    {
        return localScale_.x;
    }

    public void ApplyRotation(float _angle)
    {
        transform.rotation = Quaternion.Euler(0, _angle, 0);
    }

    public float GetRotationAngle()
    {
        return transform.rotation.eulerAngles.y;
    }
}