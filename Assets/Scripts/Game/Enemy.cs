using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _bottomLimit = -6.0f;
    [SerializeField] private float _ySpawn = 8.0f;
    [SerializeField] private float _xSpawnLimit = 6.0f;
    [SerializeField] private int _scorePoints = 5;

    private Animator _animator;
    private AudioSource _audioSource;
    private bool _isDestroy;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
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


    private IEnumerator DestroyEnemy()
    {
        _isDestroy = true;
        _animator.SetTrigger("ExplosionTrigger");
        _audioSource.Play();

        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Explosion_anim"))
            yield return null;

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;

        Destroy(gameObject);
    }
}