using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] [Required] [ChildGameObjectsOnly]
    private GameObject _mainMenu;

    [SerializeField] [Required] [ChildGameObjectsOnly]
    private GameObject _pauseMenu;

    [SerializeField] [Required] [ChildGameObjectsOnly]
    private GameObject _gameOverMenu;

    [SerializeField] [Required] [ChildGameObjectsOnly]
    private GameObject _GameUI;

    [SerializeField] [Required] [ChildGameObjectsOnly]
    private Camera _dummyCamera;


    private void Start()
    {
        GameManager.Instance.eventGameState.AddListener(HandleGameStateChange);
    }

    void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameManager.GameState.PREGAME:
                if (Input.GetKeyDown(KeyCode.Space))
                    GameManager.Instance.StartGame();

                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.Instance.Exit();
                break;

            case GameManager.GameState.RUNNING:
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.Instance.TogglePause();
                break;

            case GameManager.GameState.PAUSED:
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.Instance.TogglePause();
                break;

            case GameManager.GameState.GAMEOVER:
                if (Input.GetKeyDown(KeyCode.R))
                    GameManager.Instance.RestartGame();

                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.Instance.Exit();
                break;
        }
    }

    private void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        switch (currentState)
        {
            case GameManager.GameState.PREGAME:
                LoadMainMenu();
                break;

            case GameManager.GameState.RUNNING:
                LoadGameUI();
                break;

            case GameManager.GameState.PAUSED:
                LoadPauseMenu();
                break;

            case GameManager.GameState.GAMEOVER:
                LoadGameOverMenu();
                break;

            default:
                break;
        }
    }

    public void LoadGameUI()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(false);
        SetDummyCameraActive(false);
        _GameUI.SetActive(true);
    }

    public void LoadMainMenu()
    {
        _mainMenu.SetActive(true);
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(false);
        SetDummyCameraActive(true);
        _GameUI.SetActive(false);
    }

    public void LoadPauseMenu()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(true);
        _gameOverMenu.SetActive(false);
        SetDummyCameraActive(false);
        _GameUI.SetActive(true);
    }

    public void LoadGameOverMenu()
    {
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(true);
        SetDummyCameraActive(true);
        _GameUI.SetActive(true);
    }

    public void SetDummyCameraActive(bool active)
    {
        _dummyCamera.gameObject.SetActive(active);
    }
}