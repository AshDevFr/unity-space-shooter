using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 20.0f;
    [SerializeField][Required] private GameObject _animationPrefab;

    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        float rotationSpeedScale = _rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.forward * rotationSpeedScale);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            DestroyAsteroid();
        }
    }
    
    private void DestroyAsteroid()
    {
        Instantiate(_animationPrefab, transform.position, Quaternion.identity);
        SpawnManager.Instance.StartSpawning();
        Destroy(gameObject, 0.25f);
    }
}