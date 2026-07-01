using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlateButton : MonoBehaviour
{
    #region Refs
    [SerializeField, Foldout("Refs")]
    private PlateButtonZone _zone;
    [SerializeField, Foldout("Refs")]
    private Animator _animator;
    private readonly int _buttonIsActivatedStateId = Animator.StringToHash("IsActivated");
    [SerializeField, Foldout("Refs")]
    private List<Collider> _ignoreColliders = new();
    public List<Collider> IgnoreColliders => _ignoreColliders;
    #endregion

    #region Conditions
    [SerializeField, Foldout("Conditions")]
    private bool _hasMinMassLimit;
    [SerializeField, Min(0), Foldout("Conditions"), ShowIf("_hasMinMassLimit")]
    private float _minMassLimit = 1;
    [SerializeField, Foldout("Conditions")]
    private bool _hasMaxMassLimit;
    [SerializeField, Min(0), Foldout("Conditions"), ShowIf("_hasMaxMassLimit")]
    private float _maxMassLimit = 100;
    [SerializeField, Foldout("Conditions"), ReadOnly]
    private bool _isActivated;
    public bool IsActivated => _isActivated;

    [SerializeField, Foldout("Conditions")]
    public UnityEvent OnActivatedUnityEvent;
    [SerializeField, Foldout("Conditions")]
    public UnityEvent OnInactivatedUnityEvent;
    [SerializeField, Foldout("Conditions")]
    public UnityEvent OnActivatedChangedUnityEvent;
    [SerializeField, Foldout("Conditions")]
    public UnityEvent<bool> OnActivatedChangedUnityDEvent;
    #endregion

    private void OnValidate()
    {
        if (_zone == null)
            _zone = GetComponentInChildren<PlateButtonZone>() ?? FindAnyObjectByType<PlateButtonZone>();
    }

    public void RecalculateButtonTriggers()
    {
        float mass = 0;
        _zone.TriggeredBodies.ForEach(b => mass += b.mass);

        bool activated = mass > 0;
        if (_hasMinMassLimit && mass < _minMassLimit) activated = false;
        if (_hasMaxMassLimit && mass > _maxMassLimit) activated = false;

        _isActivated = activated;
        _animator.SetBool(_buttonIsActivatedStateId, _isActivated);

        if (_isActivated)
            OnActivatedUnityEvent?.Invoke();
        else
            OnInactivatedUnityEvent?.Invoke();
        OnActivatedChangedUnityEvent?.Invoke();
        OnActivatedChangedUnityDEvent?.Invoke(_isActivated);
    }
}
