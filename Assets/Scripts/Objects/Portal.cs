using UnityEngine;

public class Portal : MonoBehaviour
{
    private SceneChanger _sceneChanger;
    private SceneChanger SceneChanger
    {
        get
        {
            if (_sceneChanger != null) return _sceneChanger;
            _sceneChanger = FindAnyObjectByType<SceneChanger>();
            if (_sceneChanger != null) return _sceneChanger;
            GameObject sC = new GameObject("SceneChanger", typeof(SceneChanger));
            _sceneChanger = sC.GetComponent<SceneChanger>() ?? sC.AddComponent<SceneChanger>();
            return _sceneChanger;
        }
    }

    [SerializeField]
    private int _nextLevelId;
    private bool _isFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        Finish();
    }
    public void Finish()
    {
        if (_isFinished) return;

        ProgressData.Instance.CurrentLevel = Mathf.Max(ProgressData.Instance.CurrentLevel, _nextLevelId);
        SceneChanger.LoadAsync("Levels");

        _isFinished = true;
    }
}
