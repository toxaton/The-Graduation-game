using UnityEngine;
using NaughtyAttributes;
using System;

[CreateAssetMenu(fileName = "ProgressData", menuName = "SO/ProgressData")]
public class ProgressData : ScriptableObject
{
    [SerializeField, Range(1, 100)]
    private int _currentLevel = 1;
    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            _currentLevel = value;
            PlayerPrefs.SetInt(_currentLevelName, CurrentLevel);
            OnCurrentLevelChanged?.Invoke(CurrentLevel);
        }
    }
    private readonly string _currentLevelName = "CurrentLevel";
    public static Action<int> OnCurrentLevelChanged;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        Instance.LoadProgress();
    }

    [Button("Save")]
    public void SaveProgress()
    {
        PlayerPrefs.SetInt(_currentLevelName, CurrentLevel);
    }
    [Button("Load")]
    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey(_currentLevelName))
            _currentLevel = PlayerPrefs.GetInt(_currentLevelName);
    }
    [Button("Reset")]
    public void ResetProgress()
    {
        CurrentLevel = 1;
    }

    private static ProgressData s_instance;
    public static ProgressData Instance => s_instance = s_instance ?? Get();
    public static ProgressData Get() => Resources.Load<ProgressData>("Save/ProgressData");
}
