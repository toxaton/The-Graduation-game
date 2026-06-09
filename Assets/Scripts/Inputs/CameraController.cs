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
        MenuCaller.OnMenuCalledWithStatus -= ToggleCursorLock;
        MenuCaller.OnMenuCalledWithStatus += ToggleCursorLock;

        LockCursor();
    }
    private void OnDisable()
    {
        MenuCaller.OnMenuCalledWithStatus -= ToggleCursorLock;
        _inputActions.Disable();

        UnlockCursor();
    }

    private void Start()
    {
        LockCursor();
        // Инициализируем текущий вертикальный угол
        _currentVerticalAngle = Camera.transform.localEulerAngles.x;
        // Нормализуем угол (если он больше 180, переводим в отрицательный)
        if (_currentVerticalAngle > 180f)
            _currentVerticalAngle -= 360f;
    }

   private void FixedUpdate()
    {
        if (MenuCaller.S_MenuStatus) return;

        Vector2 look = _inputActions.Player.Look.ReadValue<Vector2>();
        float lookH = look.x;
        float lookV = look.y;

        transform.Rotate(Vector3.up, lookH * _horizontalRotationModifier * Time.fixedDeltaTime);

        float verticalInput = lookV * _verticalRotationModifier * Time.fixedDeltaTime;
        _currentVerticalAngle -= verticalInput; // Вычитаем, т.к. вниз - отрицательное вращение
        _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, _minVerticalAngle, _maxVerticalAngle);

        // Применяем ограниченный вертикальный поворот
        Camera.transform.localRotation = Quaternion.Euler(_currentVerticalAngle, 0f, 0f);
    }
    private void LockCursor()
    {
        // Скрываем курсор и блокируем его в центре экрана
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        // Показываем курсор и разблокируем его
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ToggleCursorLock(bool status)
    {
        if (status)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }
}
