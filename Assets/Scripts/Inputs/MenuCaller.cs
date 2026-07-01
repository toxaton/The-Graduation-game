using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuCaller : MonoBehaviour
{
    private static bool s_menuStatus;
    public static bool S_MenuStatus
    {
        get => s_menuStatus;
        set
        {
            s_menuStatus = value;
            Time.timeScale = value ? 0.0f : 1.0f;
            OnMenuCalled?.Invoke();
            OnMenuCalledWithStatus?.Invoke(S_MenuStatus);
            if (s_menuStatus)
                UnlockCursor();
            else
                LockCursor();
        }
    }

    private InputSystem_Actions _inputActions;

    public UnityEvent OnStartEvent;
    public UnityEvent OnMenuCalledEvent;
    public UnityEvent<bool> OnMenuCalledWithStatusEvent;
    public static Action OnMenuCalled;
    public static Action<bool> OnMenuCalledWithStatus;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        s_menuStatus = false;
        SceneManager.sceneLoaded -= OnSceneChange;
        SceneManager.sceneLoaded += OnSceneChange;
    }
    private static void OnSceneChange(Scene _s, LoadSceneMode _l)
    {
        S_MenuStatus = true;
    }

    private void Start()
    {
        OnStartEvent?.Invoke();
    }
    private void OnEnable()
    {
        _inputActions ??= new();
        _inputActions.Enable();
        _inputActions.UI.MenuCall.performed -= OnCall;
        _inputActions.UI.MenuCall.performed += OnCall;
    }
    private void OnDisable()
    {
        _inputActions.UI.MenuCall.performed -= OnCall;
        _inputActions.Disable();
    }

    public void ChangeStatus()
    {
        SetPause(!S_MenuStatus);
    }
    public void SetPause(bool paused)
    {
        S_MenuStatus = paused;
        OnMenuCalledEvent?.Invoke();
        OnMenuCalledWithStatusEvent?.Invoke(S_MenuStatus);
    }
    public static void LockCursor()
    {
        // Скрываем курсор и блокируем его в центре экрана
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public static void UnlockCursor()
    {
        // Показываем курсор и разблокируем его
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public static void ToggleCursorLock(bool status)
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
    private void OnCall(InputAction.CallbackContext context)
    {
        ChangeStatus();
    }
}
