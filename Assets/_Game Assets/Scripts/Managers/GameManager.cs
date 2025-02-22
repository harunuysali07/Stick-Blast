﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Action OnGameStateChanged;

    private GameState _gameState;

    [ShowInInspector, ReadOnly]
    public GameState GameState
    {
        get => _gameState;
        private set
        {
            _gameState = value;
            OnGameStateChanged?.Invoke();
        }
    }

    [Space(10)] [Header("Singleton Container")] [ReadOnly, Required]
    public TouchManager touchManager;

    [ReadOnly, Required] public AudioManager audioManager;
    [ReadOnly, Required] public LevelManager levelManager;
    [ReadOnly, Required] public CameraManager cameraManager;
    [ReadOnly, Required] public ObjectPoolingManager objectPoolingManager;

    protected override void Init()
    {
        base.Init();

        GameState = GameState.Loading;

        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        HapticManager.SetHapticsActive(DataManager.Vibration);

        OnGameStateChanged += () => Debug.Log($"Game State is : {GameState}");

        objectPoolingManager.Initialize();
        cameraManager.Initialize();
        touchManager.Initialize();
        audioManager.Initialize(); //This depends on CameraManager
        levelManager.Initialize();
        
        UIManager.Instance.Initialize();
    }

    private void Start()
    {
        GameState = GameState.Ready;

        if (GameSettingsData.Instance.skipReadyState)
            LevelStart();
    }

    public void LevelStart()
    {
        if (GameState != GameState.Ready)
        {
            Debug.LogError("Game State is not Ready", this);
            return;
        }

        GameState = GameState.Gameplay;

        AnalyticsManager.OnLevelStart();
    }

    public void LevelFinish(bool isSuccess)
    {
        if (GameState != GameState.Gameplay)
        {
            return;
        }

        GameState = isSuccess ? GameState.Complete : GameState.Fail;

        AnalyticsManager.OnLevelFinish(isSuccess ? EndType.Success : EndType.Fail);

        if (isSuccess)
        {
            DataManager.CurrentLevelIndex++;
        }
    }

    private void OnValidate()
    {
        touchManager = GetComponentInChildren<TouchManager>(true);
        audioManager = GetComponentInChildren<AudioManager>(true);
        levelManager = GetComponentInChildren<LevelManager>(true);
        cameraManager = GetComponentInChildren<CameraManager>(true);
        objectPoolingManager = GetComponentInChildren<ObjectPoolingManager>(true);
    }
}