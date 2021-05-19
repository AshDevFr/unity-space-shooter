using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
        GAMEOVER
    }

    public GameObject[] SystemPrefabs;
    public EventGameState eventGameState = new EventGameState();
    public EventPlayerStats eventPlayerStats = new EventPlayerStats();
    public EventGameScore eventGameScore = new EventGameScore();

    private List<GameObject> _instancedSystemPrefabs;
    private List<AsyncOperation> _loadOperations;

    private bool _isLoadingLevel;
    private string _currentLevelName = string.Empty;
    private GameState _currentGameState = GameState.PREGAME;
    private int _score;
    private int _bestScore;

    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    public int BestScore
    {
        get { return _bestScore; }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _bestScore = PlayerPrefs.GetInt("BestScore", 0);

        _loadOperations = new List<AsyncOperation>();
        _instancedSystemPrefabs = new List<GameObject>();

        InstantiateSystemPrefabs();
    }

    private void UpdateState(GameState state)
    {
        GameState previousState = _currentGameState;
        _currentGameState = state;

        switch (_currentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1f;
                break;

            case GameState.RUNNING:
                Time.timeScale = 1f;
                break;

            case GameState.PAUSED:
                Time.timeScale = 0f;
                break;

            case GameState.GAMEOVER:
                Time.timeScale = 1f;
                break;

            default:
                break;
        }

        eventGameState.Invoke(_currentGameState, previousState);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        for (int i = 0; i < _instancedSystemPrefabs.Count; i++)
        {
            Destroy(_instancedSystemPrefabs[i]);
        }

        _instancedSystemPrefabs.Clear();
    }


    private void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        for (int i = 0; i < SystemPrefabs.Length; i++)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }


    public void StartGame()
    {
        SetScore(0);
        LoadLevel("Game");
    }

    public void GameOver()
    {
        UpdateState(GameState.GAMEOVER);
        UnloadLevel("Game");
    }

    public void RestartGame()
    {
        UpdateState(GameState.PREGAME);
    }

    public void UpdatePlayerStats(PlayerStats playerStats)
    {
        eventPlayerStats.Invoke(playerStats);
    }

    public void SetScore(int score)
    {
        _score = score;
        if (_score > _bestScore)
        {
            _bestScore = _score;
            PlayerPrefs.SetInt("BestScore", _bestScore);
        }

        eventGameScore.Invoke(_score, _bestScore);
    }

    public void AddToScore(int increment)
    {
        SetScore(_score + increment);
    }

    public void LoadLevel(string levelName)
    {
        if (_isLoadingLevel)
            return;

        _isLoadingLevel = true;

        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError($"[GameManager] Unable to load level {levelName}");
            return;
        }

        _loadOperations.Add(ao);
        ao.completed += OnLoadcompleted;
        _currentLevelName = levelName;
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError($"[GameManager] Unable to unload level {levelName}");
            return;
        }

        ao.completed += OnUnloadcompleted;
    }

    private void OnLoadcompleted(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
            _loadOperations.Remove(ao);

        if (_loadOperations.Count == 0)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentLevelName));
            UpdateState(GameState.RUNNING);
            _isLoadingLevel = false;
        }

        Debug.Log("Load complete");
    }

    private void OnUnloadcompleted(AsyncOperation ao)
    {
        Debug.Log("Unload complete");
    }

    public void TogglePause()
    {
        if (_currentGameState == GameState.RUNNING)
            UpdateState(GameState.PAUSED);
        else if (_currentGameState == GameState.PAUSED)
            UpdateState(GameState.RUNNING);
    }

    public void Exit()
    {
        for (int i = 0; i < _instancedSystemPrefabs.Count; i++)
        {
            Destroy(_instancedSystemPrefabs[i]);
        }

        Application.Quit();
    }
}