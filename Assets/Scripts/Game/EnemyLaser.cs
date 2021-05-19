using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private float _bottomLimit = -6.0f;

    void Update()
    {
        Move();
        OutOfBounds();
    }

    private void Move()
    {
        float speedScale = _speed * Time.deltaTime;

        transform.Translate(Vector3.down * speedScale);
    }

    private void OutOfBounds()
    {
        if (transform.position.y < _bottomLimit)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Damage();
            Destroy(gameObject);
        }
    }
}
