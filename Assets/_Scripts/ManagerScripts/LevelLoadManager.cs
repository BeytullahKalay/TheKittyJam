using DG.Tweening;
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
        EventManager.OnLevelLoadPreapre += ResetDoTween;
        EventManager.OnLevelLoad += LoadLevel;
        EventManager.OnGameWinPrepare += IncreaseLevelIndex;
        EventManager.OnGameWin += CallLevelLoadExecution;
        EventManager.OnGameLose += CallLevelLoadExecution;
    }

    private void OnDisable()
    {
        EventManager.OnLevelLoadPreapre -= ResetDoTween;
        EventManager.OnLevelLoad -= LoadLevel;
        EventManager.OnGameWinPrepare -= IncreaseLevelIndex;
        EventManager.OnGameWin -= CallLevelLoadExecution;
        EventManager.OnGameLose -= CallLevelLoadExecution;
    }

    private void Start()
    {
        EventManager.LevelLoadExecute?.Invoke();
    }

    private void CheckPlayerPref()
    {
        if (PlayerPrefs.HasKey(PlayerPrefNames.Level))
            PlayerPrefs.SetInt(PlayerPrefNames.Level, 0);
    }

    private void LoadLevel()
    {
        var levelIndex = PlayerPrefs.GetInt(PlayerPrefNames.Level);

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

        if (currentLevelIndex >= levelHolder.Levels.Count - 1)
        {
            var newLevelIndex = Random.Range(0, levelHolder.Levels.Count);
            PlayerPrefs.SetInt(PlayerPrefNames.Level, newLevelIndex);
        }
        else
        {
            PlayerPrefs.SetInt(PlayerPrefNames.Level, currentLevelIndex + 1);
        }
    }

    private void CallLevelLoadExecution()
    {
        EventManager.LevelLoadExecute?.Invoke();
    }
}
