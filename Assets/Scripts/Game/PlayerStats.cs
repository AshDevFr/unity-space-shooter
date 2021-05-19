using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private int _lives = 3;

    public int Lives
    {
        get { return _lives; }
        set { _lives = value; }
    }
}