using NaughtyAttributes;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class ObjectPickup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera _playerCamera;
    public Camera PlayerCamera => _playerCamera = _playerCamera != null ? _playerCamera : Camera.main;
    [SerializeField] private float _pickupDistance = 3f;
    [SerializeField] private float _holdDistance = 2f;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _spring = 120f;
    [SerializeField] private float _damping = 20f;
    [SerializeField] private float _maxForce = 500f;
    [SerializeField] private float _angularSpring = 200f;
    [SerializeField] private float _angularDamping = 30f;
    [SerializeField] private float _maxTorque = 300f;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private LayerMask _raycastLayerMask;

    private InputSystem_Actions _inputActions;
    [SerializeField, ReadOnly]
    private PickableObject _pickableObject;
    private Rigidbody _heldObject => _pickableObject?.Rigidbody ?? null;
    public bool IsHoldingObject => _pickableObject != null;
    private Quaternion _pickupRotationOffset = Quaternion.identity;

    private void OnValidate()
    {
        _playerCamera = _playerCamera != null ? _playerCamera : Camera.main;
    }
    private void OnEnable()
    {
        _inputActions ??= new InputSystem_Actions();
        _inputActions.Enable();

        _inputActions.Player.PickObject.performed -= OnInteract;
        _inputActions.Player.PickObject.performed += OnInteract;
    }
    private void OnDisable()
    {
        _inputActions.Player.PickObject.performed -= OnInteract;
        _inputActions.Disable();
    }

    private void FixedUpdate()
    {
        if (!IsHoldingObject) return;

        Vector3 delta = _holdPoint.position - _heldObject.position;
        Vector3 force = delta * _spring - _heldObject.linearVelocity * _damping;
        force = Vector3.ClampMagnitude(force, _maxForce);
        _heldObject.AddForce(force, ForceMode.Acceleration);

        Quaternion targetRotation;

        switch (_pickableObject.HoldRotation)
        {
            case HoldRotationMode.FaceCamera:
                targetRotation = _playerCamera.transform.rotation;
                break;

            case HoldRotationMode.Free:
            default:
                targetRotation = _playerCamera.transform.rotation * _pickupRotationOffset;
                break;
        }

        Quaternion currentRot = _heldObject.rotation;
        Quaternion deltaRot = targetRotation * Quaternion.Inverse(currentRot);

        deltaRot = Quaternion.RotateTowards(Quaternion.identity, deltaRot, 180f);

        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 0.01f)
        {
            float currentAngularSpring = _angularSpring;
            if (_pickableObject.HoldRotation == HoldRotationMode.Free)
                currentAngularSpring *= 0.65f;   // подбери под себя

            Vector3 torque = axis * (angle * Mathf.Deg2Rad * currentAngularSpring)
                            - _heldObject.angularVelocity * _angularDamping;

            torque = Vector3.ClampMagnitude(torque, _maxTorque);
            _heldObject.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsHoldingObject)
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
            PlayerCamera.transform.position,
            PlayerCamera.transform.forward);

        if (Physics.Raycast(
            ray: ray,
            hitInfo: out RaycastHit hit,
            maxDistance: _pickupDistance,
            layerMask: _raycastLayerMask))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null && rb.TryGetComponent<PickableObject>(out var pO) && pO.TryPick())
            {
                _pickableObject = pO;
                //_heldObject.useGravity = false;
                _heldObject.angularVelocity = Vector3.zero;
                _heldObject.linearVelocity = Vector3.zero;
                _pickupRotationOffset = Quaternion.Inverse(_playerCamera.transform.rotation) * _heldObject.rotation;
            }
        }
    }
    private void DropObject()
    {
        if (!_pickableObject.TryDrop()) return;

        //_heldObject.useGravity = true;
        _pickableObject = null;
    }
}
