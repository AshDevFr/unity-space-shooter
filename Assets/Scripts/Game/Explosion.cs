﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return;

        Destroy(gameObject);
    }
}
