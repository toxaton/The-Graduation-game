using NaughtyAttributes;
using UnityEngine;

public class GatesController : MonoBehaviour
{
    [SerializeField]
    private Animator _gatesAnimator;
    private readonly int _gateOpenTriggerId = Animator.StringToHash("open");
    private readonly int _gateCloseTriggerId = Animator.StringToHash("close");

    private void OnValidate()
    {
        if (_gatesAnimator == null && TryGetComponent<Animator>(out var anim))
            _gatesAnimator = anim;
    }

    [Button("Close", EButtonEnableMode.Playmode)]
    public void CloseGates()
    {
        _gatesAnimator.SetTrigger(_gateCloseTriggerId);
    }
    [Button("Open", EButtonEnableMode.Playmode)]
    public void OpenGates()
    {
        _gatesAnimator.SetTrigger(_gateOpenTriggerId);
    }
}
