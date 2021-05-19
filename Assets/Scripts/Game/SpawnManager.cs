using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : Singleton<SpawnManager>
{
    [Required] [SerializeField] private GameObject _enemyPrefab;
    [Required] [SerializeField] private List<GameObject> _powerupPrefabs;

    [Required] [ChildGameObjectsOnly] [SerializeField]
    private GameObject _enemyContainer;

    [Required] [ChildGameObjectsOnly] [SerializeField]
    private GameObject _powerupContainer;

    [SerializeField] private float _ySpawn = 8.0f;
    [SerializeField] private float _xSpawnLimit = 6.0f;

    [SerializeField] private float _enemySpawnStartDelay = 1.0f;
    [SerializeField] private float _enemySpawnRepeatDelayMin = 3.0f;
    [SerializeField] private float _enemySpawnRepeatDelayMax = 7.0f;
    [SerializeField] private float _powerupSpawnStartDelay = 7.0f;
    [SerializeField] private float _powerupSpawnRepeatDelayMin = 5.0f;
    [SerializeField] private float _powerupSpawnRepeatDelayMax = 10.0f;

    private bool _isGameOver;

    private void Start()
    {
        GameManager.Instance.eventGameState.AddListener(HandleGameStateChange);
    }

    private Vector3 SpawnPosition()
    {
        return new Vector3(Random.Range(-_xSpawnLimit, _xSpawnLimit), _ySpawn, 0);
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(_enemySpawnStartDelay);
        while (!_isGameOver)
        {
            GameObject enemy = Instantiate(_enemyPrefab, SpawnPosition(), Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(Random.Range(_enemySpawnRepeatDelayMin, _enemySpawnRepeatDelayMax));
        }
    }

    private IEnumerator SpawnPowerup()
    {
        yield return new WaitForSeconds(_powerupSpawnStartDelay);
        while (!_isGameOver)
        {
            int powerupIndex = Random.Range(0, _powerupPrefabs.Count);
            GameObject powerup = Instantiate(_powerupPrefabs[powerupIndex], SpawnPosition(), Quaternion.identity);
            powerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(Random.Range(_powerupSpawnRepeatDelayMin, _powerupSpawnRepeatDelayMax));
        }
    }

    private void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _isGameOver = currentState == GameManager.GameState.GAMEOVER;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }
}