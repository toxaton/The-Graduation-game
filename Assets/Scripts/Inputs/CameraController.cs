using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    public Camera Camera => _camera == null ? Camera.main : _camera;
    [SerializeField]
    private float _horizontalRotationModifier = 10f;
    [SerializeField]
    private float _verticalRotationModifier = 10f;

    [Header("Vertical Rotation Limits")]
    [SerializeField]
    private float _minVerticalAngle = -80f;  
    [SerializeField]
    private float _maxVerticalAngle = 80f;   

    private InputSystem_Actions _inputActions;
    private float _currentVerticalAngle = 0f; 

    private void OnEnable()
    {
        _inputActions ??= new();
        _inputActions.Enable();
    }
    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Start()
    {
        // Инициализируем текущий вертикальный угол
        _currentVerticalAngle = Camera.transform.localEulerAngles.x;
        // Нормализуем угол (если он больше 180, переводим в отрицательный)
        if (_currentVerticalAngle > 180f)
            _currentVerticalAngle -= 360f;
    }

    private void LateUpdate()
    {
        if (MenuCaller.S_MenuStatus) return;

        Vector2 look = _inputActions.Player.Look.ReadValue<Vector2>();

        // Горизонтальный поворот — через Rigidbody (важно!)
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            float yaw = look.x * _horizontalRotationModifier * Time.fixedDeltaTime;
            Quaternion deltaRot = Quaternion.Euler(0, yaw, 0);
            rb.MoveRotation(rb.rotation * deltaRot);   // ← Вот это главное
        }
        else
        {
            // если нет rb — fallback
            transform.Rotate(Vector3.up, look.x * _horizontalRotationModifier * Time.fixedDeltaTime);
        }

        // Вертикальный — только камера
        float pitch = look.y * _verticalRotationModifier * Time.fixedDeltaTime;
        _currentVerticalAngle -= pitch;
        _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
        Camera.transform.localRotation = Quaternion.Euler(_currentVerticalAngle, 0f, 0f);
    }

    
}
