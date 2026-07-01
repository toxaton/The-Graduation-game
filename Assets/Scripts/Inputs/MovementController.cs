using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private readonly int _moveSpeedAnimatorPropId = Animator.StringToHash("MoveSpeed");
    [SerializeField]
    private float _moveSpeed = 1f;
    [SerializeField, Range(0f, 1000f)]
    private float _jumpForce = 100f;
    [SerializeField]
    private FeetTrigger _feet;
    private DateTime _lastJump;
    [SerializeField]
    private float _jumpDelaySec = 0.2f;
    private bool CanJump => _lastJump.AddSeconds(_jumpDelaySec) < DateTime.UtcNow;
    [SerializeField, Range(0f, 5f)]
    private float _sprintForce = 2f;
    [SerializeField]
    private bool _multiplyByMass = true;
    private InputSystem_Actions _inputActions;
    private Rigidbody _rb;

    [field: SerializeField, Range(0f, 5f), ReadOnly]
    public float SmoothSprintModifier { get; private set; }

    #region ChangeSpeedByPickedOpject
    [SerializeField]
    private ObjectPickup _objectPickup;
    [SerializeField, ShowIf("ChangeSpeedByPickedObject")]
    private bool _autoUpdateModifier = false;
    private bool ChangeSpeedByPickedObject => _objectPickup != null;
    [SerializeField, MinMaxSlider(minValue: 0, maxValue: 100), ShowIf("ChangeSpeedByPickedObject")]
    private Vector2 _pickedObjectMassMinMaxLimits = new Vector2(0, 100);
    [SerializeField, ShowIf("ChangeSpeedByPickedObject")]
    private Vector2 _pickedObjectMinMaxSpeedAffects = new Vector2(1, 0.5f);
    [SerializeField, ShowIf("ChangeSpeedByPickedObject"), ReadOnly]
    private float _pickedObjectSpeedModifier = 1f;
    public void UpdatePickedObjectSpeedModifier() => _pickedObjectSpeedModifier = GetPickedObjectSpeedModifier();
    private float GetPickedObjectSpeedModifier()
    {
        if (!ChangeSpeedByPickedObject || _objectPickup.HeldObject is null) return 1;
        float mass = _objectPickup.HeldObject.mass,
        minLim = _pickedObjectMassMinMaxLimits.x,       maxLim = _pickedObjectMassMinMaxLimits.y,
        minAff = _pickedObjectMinMaxSpeedAffects.x,     maxAff = _pickedObjectMinMaxSpeedAffects.y;
        float diapLim = maxLim - minAff, diapAff = maxAff - minAff;
        if (diapLim == 0 || diapAff == 0) return 1;
        if (mass <= minLim) return minAff;
        if (mass >= maxLim) return maxAff;
        float diapMass = mass - minLim,
            k = diapMass / diapLim;
        
        return minAff + (k * diapAff);
    }
    #endregion

    private Tween _sprintAnimation;

    private void Start()
    {
        SmoothSprintModifier = 1;
    }
    private void OnEnable()
    {
        _rb ??= GetComponent<Rigidbody>();
        _inputActions ??= new();
        _inputActions?.Enable();
        _inputActions.Player.Jump.performed -= OnJump;
        _inputActions.Player.Jump.performed += OnJump;

        _inputActions.Player.Sprint.started -= OnSprintStart;
        _inputActions.Player.Sprint.started += OnSprintStart;
        _inputActions.Player.Sprint.canceled -= OnSprintEnd;
        _inputActions.Player.Sprint.canceled += OnSprintEnd;
    }
    private void OnDisable()
    {
        _inputActions.Player.Jump.performed -= OnJump;

        _inputActions.Player.Sprint.started -= OnSprintStart;
        _inputActions.Player.Sprint.canceled -= OnSprintEnd;
        _inputActions.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!_feet.IsGrounded || !CanJump) return;
        _lastJump = DateTime.UtcNow;
        _rb.AddForce(new Vector3(0, _jumpForce * (_multiplyByMass ? _rb.mass : 1), 0));
    }
    
    private void OnSprintStart(InputAction.CallbackContext context)
    {
        AnimateSprite(_sprintForce, 1f, Ease.OutQuint);
    }
    private void OnSprintEnd(InputAction.CallbackContext context)
    {
        AnimateSprite(1, 1f, Ease.OutQuint);
    }
    private void FixedUpdate()
    {
        if (_autoUpdateModifier) UpdatePickedObjectSpeedModifier();

        Vector2 move = _inputActions.Player.Move.ReadValue<Vector2>();
        move *= _moveSpeed * Time.fixedDeltaTime * SmoothSprintModifier * _pickedObjectSpeedModifier;
        //transform.Translate(move.x, 0, move.y);
        Quaternion rot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
        _rb.linearVelocity = rot * new Vector3(move.x, _rb.linearVelocity.y, move.y);
        _animator?.SetFloat(_moveSpeedAnimatorPropId, Mathf.Abs(move.y));
    }

    private void AnimateSprite(float endValue, float duration, Ease ease = Ease.Linear)
    {
        _sprintAnimation?.Kill();
        _sprintAnimation = DOTween.To(
            () => this.SmoothSprintModifier,
            x => this.SmoothSprintModifier = x,
            endValue,
            duration)
            .SetEase(ease)
            .Play();
    }
}
