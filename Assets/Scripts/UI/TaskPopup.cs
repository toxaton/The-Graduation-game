using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TaskPopup : MonoBehaviour
{
    private MenuCaller _menuCaller;
    private MenuCaller MenuCaller => _menuCaller = _menuCaller != null ? _menuCaller : FindAnyObjectByType<MenuCaller>();

    [SerializeField, ResizableTextArea, Foldout("Texts")]
    private string _taskText;
    [SerializeField, ResizableTextArea, Foldout("Texts")]
    private string _hintText;
    [SerializeField, Foldout("Components")]
    private TMP_Text _taskTMP;
    [SerializeField, Foldout("Components")]
    private TMP_Text _hintTMP;
    [SerializeField, Foldout("Components")]
    private GameObject _popupBody;
    [SerializeField, Foldout("Components")]
    private GameObject _hintBody;
    [SerializeField, Foldout("Components")]
    private GameObject _okButton;
    [SerializeField, Foldout("Components")]
    private GameObject _closeButton;

    private bool IsPopupShowing
    {
        get => _popupBody.activeSelf;
        set => _popupBody.SetActive(value);
    }
    private bool IsHintShowing
    {
        get => _hintBody.activeSelf;
        set => _hintBody.SetActive(value);
    }
    private bool FirstShow => _okButton.activeSelf;

    public void OnStart()
    {
        MenuCaller.SetPause(true);
        ShowPopup();
    }

    public void ShowPopup()
    {
        IsPopupShowing = true;
        _taskTMP.text = _taskText;
        _hintTMP.text = _hintText;
    }
    public void HidePopup()
    {
        IsPopupShowing = false;
        if (FirstShow)
        {
            _okButton.SetActive(false);
            _closeButton.SetActive(true);
        }
    }

    public void ShowHint() => SetHint(true);
    public void HideHint() => SetHint(false);
    public void SwitchHint() => SetHint(!IsHintShowing);
    private void SetHint(bool show)
    {
        IsHintShowing = show;
        _hintTMP.text = _hintText;
    }
}
