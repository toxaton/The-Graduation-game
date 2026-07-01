using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class PlateButtonZone : MonoBehaviour
{
    [SerializeField]
    private PlateButton _button;
    [SerializeField, ReadOnly]
    private List<Rigidbody> _triggeredBodies = new();
    public List<Rigidbody> TriggeredBodies => _triggeredBodies;

    private void OnValidate()
    {
        if (_button == null)
            _button = GetComponentInParent<PlateButton>() ?? FindAnyObjectByType<PlateButton>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_button.IgnoreColliders.Contains(other)) return;
        var rb = other.GetComponent<Rigidbody>();
        if (rb != null && !_triggeredBodies.Contains(rb))
        {
            _triggeredBodies.Add(rb);
            _button.RecalculateButtonTriggers();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_button.IgnoreColliders.Contains(other)) return;

        var rb = other.GetComponent<Rigidbody>();
        if (rb != null && _triggeredBodies.Contains(rb))
        {
            _triggeredBodies.Remove(rb);
            _button.RecalculateButtonTriggers();
        }
    }
}
