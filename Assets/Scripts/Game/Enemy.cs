using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _bottomLimit = -6.0f;
    [SerializeField] private float _ySpawn = 8.0f;
    [SerializeField] private float _xSpawnLimit = 6.0f;
    [SerializeField] private int _scorePoints = 5;

    [SerializeField] private float _fireStartDelay = 1.0f;
    [SerializeField] private float _fireRepeatDelayMin = 3.0f;
    [SerializeField] private float _fireRepeatDelayMax = 7.0f;

    [Required] [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private Vector3 _laserSpawnOffset = new Vector3(0, 1, 0);
    
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _isDestroy;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(Fire());
    }

    void Update()
    {
        Move();
        OutOfBounds();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDestroy)
            return;

        if (other.CompareTag("Laser"))
        {
            GameManager.Instance.AddToScore(_scorePoints);
            Destroy(other.gameObject);
            StartCoroutine(DestroyEnemy());
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Damage();
            StartCoroutine(DestroyEnemy());
        }
    }

    private void Move()
    {
        if (_isDestroy)
            return;

        float speedScale = _speed * Time.deltaTime;

        transform.Translate(Vector3.down * speedScale);
    }

    private void OutOfBounds()
    {
        if (transform.position.y < _bottomLimit)
            Respawn();
    }

    private void Respawn()
    {
        transform.position = new Vector3(Random.Range(-_xSpawnLimit, _xSpawnLimit), _ySpawn, 0);
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(_fireStartDelay);
        while (!_isDestroy)
        {
            GameObject enemy = Instantiate(_laserPrefab, transform.position + _laserSpawnOffset, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_fireRepeatDelayMin, _fireRepeatDelayMax));
        }
    }


    private IEnumerator DestroyEnemy()
    {
        _isDestroy = true;
        _animator.SetTrigger("ExplosionTrigger");
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Explosion_anim"))
            yield return null;

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;

        Destroy(gameObject);
    }
}