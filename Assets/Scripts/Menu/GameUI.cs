using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] [Required] [ChildGameObjectsOnly]
    private Text _scoreText;

    [SerializeField] [Required] [ChildGameObjectsOnly]
    private Image _livesDisplayImage;

    [SerializeField] [Required] private Sprite[] _livesSprites;

    void Start()
    {
        GameManager.Instance.eventGameScore.AddListener(OnGameScoreChange);
        GameManager.Instance.eventPlayerStats.AddListener(OnPlayerLivesChange);
    }

    void OnGameScoreChange(int score)
    {
        _scoreText.text = $"Score: {score}";
    }

    void OnPlayerLivesChange(PlayerStats playerStats)
    {
        Sprite sprite;
        if (playerStats.Lives >= _livesSprites.Length)
            sprite = _livesSprites[0];
        else
            sprite = _livesSprites[playerStats.Lives];


        _livesDisplayImage.sprite = sprite;
    }
}