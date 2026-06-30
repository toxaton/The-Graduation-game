using NaughtyAttributes;
using UnityEngine;

public class FeetTrigger : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public bool IsGrounded { get; private set; } = false;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField, ReadOnly] private int _groundContacts = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer) return;

        if (((1 << other.gameObject.layer) & groundMask.value) != 0)
        {
            _groundContacts++;
            IsGrounded = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer) return;
        IsGrounded = _groundContacts > 0;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer) return;

        if (((1 << other.gameObject.layer) & groundMask.value) != 0)
        {
            _groundContacts = Mathf.Max(0, _groundContacts - 1);
            IsGrounded = _groundContacts > 0;
        }
    }

    public void ResetGrounded()
    {
        IsGrounded = false;
        _groundContacts = 0;
    }
}