using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static string CurrentSceneName => SceneManager.GetActiveScene().name;

    public void Load(string sceneName) => SceneManager.LoadScene(sceneName);
    public async void LoadAsync(string sceneName) => await SceneManager.LoadSceneAsync(sceneName);
    public void Reload() => Load(CurrentSceneName);
    public async void ReloadAsync() => await SceneManager.LoadSceneAsync(CurrentSceneName);
    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }
}
