using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour
{
    [SerializeField]
    private List<Button> _levels = new();

    private void Start()
    {
        SaveChanged(ProgressData.Instance.CurrentLevel);
    }
    private void OnEnable()
    {
        
        ProgressData.OnCurrentLevelChanged -= SaveChanged;
        ProgressData.OnCurrentLevelChanged += SaveChanged;
    }
    private void OnDisable()
    {
        ProgressData.OnCurrentLevelChanged -= SaveChanged;
    }
    private void SaveChanged(int currentLevel)
    {
        if (_levels.Count == 0) return;

        int counter = 0;
        foreach (Button level in _levels)
        {
            counter++;
            level.interactable = currentLevel >= counter;
        }
    }
}
