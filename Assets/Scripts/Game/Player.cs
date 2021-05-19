using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats = new PlayerStats();

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _xLimit = 10.0f;
    [SerializeField] private float _topLimit = 0f;
    [SerializeField] private float _bottomLimit = -3.5f;

    [Required] [ChildGameObjectsOnly] [SerializeField]
    private GameObject _shieldGameObject;

    [Required] [ChildGameObjectsOnly] [SerializeField]
    private List<GameObject> _engineFireGameObjects;

    [Required] [SerializeField] private GameObject _laserPrefab;
    [Required] [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private Vector3 _laserSpawnOffset = new Vector3(0, 1, 0);
    [SerializeField] private float _coolDownDelay = 0.15f;
    [SerializeField] private float _powerupDuration = 5f;
    [SerializeField] private float _speedBoostMultiplier = 2f;

    private Animator _animator;
    [Required] [SerializeField] private AudioClip _laserAudioClip;
    private AudioSource _audioSource;
    
    private bool _canFire = true;
    private bool _canBeDamaged = true;
    private bool _tripleShotEnabled;
    private bool _speedBoostEnabled;
    private bool _shieldEnabled;
    private int[] _engineOrder;
    private int _nextEngineIndex;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _laserAudioClip;
        
        System.Random random = new System.Random();
        _engineOrder = Enumerable.Range(0, _engineFireGameObjects.Count).OrderBy(i => random.Next()).ToArray();
        _nextEngineIndex = 0;
        
        transform.position = Vector3.zero;
        GameManager.Instance.UpdatePlayerStats(_playerStats);
    }

    void Update()
    {
        MovePlayer();
        RestrictPlayerBounds();

        if (_canFire && Input.GetKeyDown(KeyCode.Space))
        {
            FireLaser();
        }
    }

    void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        _animator.SetFloat("Turnf", horizontalInput);
        float verticalInput = Input.GetAxis("Vertical");

        float speed = _speedBoostEnabled ? _speed * _speedBoostMultiplier : _speed;
        float speedScale = speed * Time.deltaTime;
        float xScale = speedScale * horizontalInput;
        float yScale = speedScale * verticalInput;

        transform.Translate(new Vector3(xScale, yScale, 0));
    }

    private void RestrictPlayerBounds()
    {
        Vector3 position = transform.position;
        float y = Mathf.Clamp(position.y, _bottomLimit, _topLimit);
        float x = position.x;

        if (x > _xLimit)
            x = -_xLimit;
        else if (x < -_xLimit)
            x = _xLimit;

        transform.position = new Vector3(x, y, position.z);
    }

    private void EnableFire()
    {
        _canFire = true;
    }

    private void EnableDamage()
    {
        _canBeDamaged = true;
    }

    public void EnablePowerup(PowerupType powerup)
    {
        switch (powerup)
        {
            case PowerupType.TRIPLE_SHOT:
                _tripleShotEnabled = true;
                break;
            case PowerupType.SPEED:
                _speedBoostEnabled = true;
                break;
            case PowerupType.SHIELD:
                _shieldEnabled = true;
                _shieldGameObject.gameObject.SetActive(true);
                break;
        }
        
        StartCoroutine(DisablePowerup(powerup));
    }

    IEnumerator DisablePowerup(PowerupType powerup)
    {
        yield return new WaitForSeconds(_powerupDuration);
        switch (powerup)
        {
            case PowerupType.TRIPLE_SHOT:
                _tripleShotEnabled = false;
                break;
            case PowerupType.SPEED:
                _speedBoostEnabled = false;
                break;
            case PowerupType.SHIELD:
                _shieldEnabled = false;
                _shieldGameObject.gameObject.SetActive(false);
                break;
        }
    }

    private void FireLaser()
    {
        GameObject laserPrefabEnabled = _tripleShotEnabled ? _tripleShotPrefab : _laserPrefab;

        Instantiate(laserPrefabEnabled, transform.position + _laserSpawnOffset, Quaternion.identity);
        _canFire = false;
        Invoke("EnableFire", _coolDownDelay);
        _audioSource.Play();
    }

    public void Damage()
    {
        if(!_canBeDamaged)
            return;

        _canBeDamaged = false;
        Invoke("EnableDamage", 0.2f);
        
        if (_shieldEnabled)
        {
            _shieldEnabled = false;
            _shieldGameObject.gameObject.SetActive(false);
            return;
        }

        if (_nextEngineIndex < _engineOrder.Length)
        {
            int nextEngineIndex = _engineOrder[_nextEngineIndex];
            if (nextEngineIndex < _engineFireGameObjects.Count)
            {
                _engineFireGameObjects[nextEngineIndex].SetActive(true);
                _nextEngineIndex++;
            }
        }

        _playerStats.Lives -= 1;
        GameManager.Instance.UpdatePlayerStats(_playerStats);
        if (_playerStats.Lives < 1)
        {
            GameManager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}