using UnityEngine.Events;

[System.Serializable]
public class EventGameScore : UnityEvent<int>
{
}

[System.Serializable]
public class EventPlayerStats : UnityEvent<PlayerStats>
{
}

[System.Serializable]
public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState>
{
}