using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.TogglePause();
    }
    
    public void Exit()
    {
        GameManager.Instance.Exit();
    }
}
