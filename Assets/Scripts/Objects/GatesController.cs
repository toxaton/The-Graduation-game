using NaughtyAttributes;
using UnityEngine;

public class GatesController : MonoBehaviour
{
    [SerializeField]
    private Animator _gatesAnimator;
    private readonly int _gateIsOpenedId = Animator.StringToHash("IsOpened");

    private void OnValidate()
    {
        if (_gatesAnimator == null && TryGetComponent<Animator>(out var anim))
            _gatesAnimator = anim;
    }

    [Button("Close", EButtonEnableMode.Playmode)]
    public void CloseGates()
    {
        _gatesAnimator.SetBool(_gateIsOpenedId, false);
    }
    [Button("Open", EButtonEnableMode.Playmode)]
    public void OpenGates()
    {
        _gatesAnimator.SetBool(_gateIsOpenedId, true);
    }
}
