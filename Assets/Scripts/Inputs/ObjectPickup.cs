using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _pickupDistance = 3f;
    [SerializeField] private float _holdDistance = 2f;
    [SerializeField] private float _moveSpeed = 10f;

    private InputSystem_Actions _inputActions;
    private Rigidbody _heldObject;

    private void OnEnable()
    {
        _inputActions ??= new InputSystem_Actions();
        _inputActions.Enable();

        _inputActions.Player.Interact.performed -= OnInteract;
        _inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _inputActions.Player.Interact.performed -= OnInteract;
        _inputActions.Disable();
    }

    private void Update()
    {
        if (_heldObject == null)
            return;

        Vector3 targetPosition =
            _playerCamera.transform.position +
            _playerCamera.transform.forward * _holdDistance;

        Vector3 direction = targetPosition - _heldObject.position;

        _heldObject.linearVelocity = direction * _moveSpeed;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (_heldObject == null)
        {
            TryPickup();
        }
        else
        {
            DropObject();
        }
    }

    private void TryPickup()
    {
        Ray ray = new Ray(
            _playerCamera.transform.position,
            _playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _pickupDistance))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                _heldObject = rb;
                _heldObject.useGravity = false;
                _heldObject.angularVelocity = Vector3.zero;
            }
        }
    }

    private void DropObject()
    {
        _heldObject.useGravity = true;
        _heldObject = null;
    }
}