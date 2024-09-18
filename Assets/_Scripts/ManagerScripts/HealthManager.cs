using System;
using TMPro;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private GameSettingsDataSO gameSettings;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text remainingTimeText;
    public int CurrentLives { get; private set; }

    private int _maxLives;
    private float _timeToNextLife;
    private DateTime _lastTimeChecked;
    private float _remainingTimeForNextLife;

    private void Start()
    {
        InitializeLiveParameters();
        GetHealthDataFromPlayerPref();
        StartLiveRegeneration();
        AssingCurrentLivesToText();
        InitializeRemainingTimeTMPTextVisibility();
        UpdateRemainingTimeText(); // Remaining time textini hemen güncelle
    }

    private void Update()
    {
        if (CurrentLives < _maxLives)
        {
            UpdateRemainingTime();
        }
    }

    public void DecreaseLife()
    {
        CurrentLives--;
        PlayerPrefs.SetInt(PlayerPrefNames.Health, CurrentLives);
        AssingCurrentLivesToText();
        remainingTimeText.gameObject.SetActive(true);
    }

    public bool IsHaveLive()
    {
        return CurrentLives > 0;
    }

    private void StartLiveRegeneration()
    {
        TimeSpan timeSpan = DateTime.Now - _lastTimeChecked;
        int livesToAdd = Mathf.FloorToInt((float)timeSpan.TotalSeconds / _timeToNextLife);

        CurrentLives = Mathf.Min(CurrentLives + livesToAdd, _maxLives);

        float leftoverTime = (float)timeSpan.TotalSeconds % _timeToNextLife;
        _remainingTimeForNextLife = _timeToNextLife - leftoverTime;

        if (CurrentLives < _maxLives)
        {
            Invoke(nameof(AddLife), _remainingTimeForNextLife);
        }
    }

    private void UpdateRemainingTime()
    {
        _remainingTimeForNextLife -= Time.deltaTime;

        if (_remainingTimeForNextLife <= 0)
        {
            AddLife();
            _remainingTimeForNextLife = _timeToNextLife;
        }

        UpdateRemainingTimeText();
    }

    private void UpdateRemainingTimeText()
    {
        int minutes = Mathf.FloorToInt(_remainingTimeForNextLife / 60);
        int seconds = Mathf.FloorToInt(_remainingTimeForNextLife % 60);
        remainingTimeText.text = $"{minutes:D2}:{seconds:D2}";
    }

    private void AddLife()
    {
        if (CurrentLives < _maxLives)
        {
            CurrentLives++;
            AssingCurrentLivesToText();
            PlayerPrefs.SetInt(PlayerPrefNames.Health, CurrentLives);

            if (CurrentLives < _maxLives)
            {
                Invoke(nameof(AddLife), _timeToNextLife);
            }
        }
    }

    private void GetHealthDataFromPlayerPref()
    {
        CurrentLives = PlayerPrefs.GetInt(PlayerPrefNames.Health, _maxLives);
        _lastTimeChecked = DateTime.Parse(PlayerPrefs.GetString(PlayerPrefNames.LastHealthCheck, DateTime.Now.ToString()));
    }

    private void InitializeLiveParameters()
    {
        _maxLives = gameSettings.MaxLiveAmount;
        _timeToNextLife = gameSettings.LiveRegenarationTimeMinute * 60;
    }

    private void AssingCurrentLivesToText()
    {
        livesText.text = CurrentLives.ToString();
    }

    private void InitializeRemainingTimeTMPTextVisibility()
    {
        remainingTimeText.gameObject.SetActive(CurrentLives < _maxLives);
    }
}
