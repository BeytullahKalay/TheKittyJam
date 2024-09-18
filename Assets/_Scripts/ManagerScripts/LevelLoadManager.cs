using DG.Tweening;
using GameAnalyticsSDK;
using UnityEngine;


[DefaultExecutionOrder(-1)]
public class LevelLoadManager : MonoBehaviour
{
    [SerializeField] private LevelHolder levelHolder;

    private GameObject _loadedLevel;

    private void Awake()
    {
        CheckPlayerPref();
    }

    private void OnEnable()
    {
        EventManager.OnLevelLoadPrepare += ResetDoTween;
        EventManager.OnLevelLoad += LoadLevel;
        EventManager.OnLevelLoad += FireStartLevelAnalytics;

        EventManager.OnGameWinPrepare += IncreaseLevelIndex;
        EventManager.OnGameWinPrepare += ResetDoTween;

        EventManager.OnGameLosePrepare += ResetDoTween;
        EventManager.OnGameLose += FireFailLevelAnalytics;
    }

    private void OnDisable()
    {
        EventManager.OnLevelLoadPrepare -= ResetDoTween;
        EventManager.OnLevelLoad -= LoadLevel;
        EventManager.OnLevelLoad -= FireStartLevelAnalytics;

        EventManager.OnGameWinPrepare -= IncreaseLevelIndex;
        EventManager.OnGameWinPrepare -= ResetDoTween;

        EventManager.OnGameLosePrepare -= ResetDoTween;
        EventManager.OnGameLose -= FireFailLevelAnalytics;
    }

    private void Start()
    {
        EventManager.LevelLoadExecute?.Invoke();
    }

    private void CheckPlayerPref()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefNames.Level))
            PlayerPrefs.SetInt(PlayerPrefNames.Level, 0);
    }

    private void LoadLevel()
    {
        var levelIndex = PlayerPrefs.GetInt(PlayerPrefNames.Level);

        if (levelIndex >= levelHolder.Levels.Count - 1)
            levelIndex = Random.Range(0, levelHolder.Levels.Count);

        if (_loadedLevel != null)
            Destroy(_loadedLevel);

        _loadedLevel = Instantiate(levelHolder.Levels[levelIndex], Vector3.zero, Quaternion.identity);
    }

    private static void ResetDoTween()
    {
        DOTween.Clear();
        DOTween.Init();
    }

    private void IncreaseLevelIndex()
    {
        var currentLevelIndex = PlayerPrefs.GetInt(PlayerPrefNames.Level);
        var nextLevel = currentLevelIndex + 1;
        PlayerPrefs.SetInt(PlayerPrefNames.Level, nextLevel);
    }

    private void FireStartLevelAnalytics()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level " + PlayerPrefs.GetInt(PlayerPrefNames.Level));
    }

    private void FireFailLevelAnalytics()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level " + PlayerPrefs.GetInt(PlayerPrefNames.Level));
    }
}
