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

    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

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
                break;

            case GameState.RUNNING:
                break;

            case GameState.GAMEOVER:
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

    public void AddToScore(int increment)
    {
        _score += increment;
        eventGameScore.Invoke(_score);
    }

    public void LoadLevel(string levelName)
    {
        if(_isLoadingLevel )
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
}