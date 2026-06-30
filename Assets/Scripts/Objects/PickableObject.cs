using NaughtyAttributes;
using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody = _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>();
    [field: SerializeField] public bool CanBePicked { get; private set; } = true;
    [field: SerializeField, ReadOnly] public bool IsPicked { get; private set; }
    [field: SerializeField] public HoldRotationMode HoldRotation { get; private set; } = HoldRotationMode.Free;

    private void OnValidate()
    {
        _rigidbody = _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>();
    }

    public bool TryPick()
    {
        if (IsPicked || !CanBePicked) return false;

        IsPicked = true;
        return true;
    }
    public bool TryDrop()
    {
        if (!IsPicked) return false;

        IsPicked = false;
        return true;
    }
}

public enum HoldRotationMode
{
    Free,
    FaceCamera,
}
