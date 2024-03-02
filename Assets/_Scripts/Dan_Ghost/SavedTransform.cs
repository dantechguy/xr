using UnityEngine;

public class SavedTransform
{
    private Vector3 _position;
    private Quaternion _rotation;
    private Vector3 _scale;

    public Vector3 position
    {
        get
        {
            return _position;
        }
    }

    public Quaternion rotation
    {
        get
        {
            return _rotation;
        }
    }

    public Vector3 scale
    {
        get
        {
            return _scale;
        }
    }

    public SavedTransform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        _position = position;
        _rotation = rotation;
        _scale = scale;
    }

    public static SavedTransform FromTransform(Transform transform)
    {
        return new SavedTransform(transform.position, transform.rotation, transform.localScale);
    }

    public void ApplyToTransform(Transform transform)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        transform.localScale = _scale;
    }

    public override string ToString()
    {
        return "SavedTransform(" + _position + ", " + _rotation + ", " + _scale + ")";
    }
}