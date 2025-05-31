using _SPC.GamePlay.Managers;
using _SPC.GamePlay.Score;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.UI.Scripts.Scripts
{
    public class GameplayScoreRenderer : MonoBehaviour
    {
        [Header("TextMeshPro References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI nicknameText;

        private GameplayScore _gameplayScore;

        private void Start()
        {
            // Get nickname from GameManager
            string currentNickname = GameManager.Instance.CurrentNickname;
            if (nicknameText != null)
                nicknameText.text = "Name: "+ currentNickname;

            // Subscribe to global score update event
            GameEvents.OnUpdateScore += UpdateScoreDisplay;

            // Set initial value
            UpdateScoreDisplay(0);
        }

        private void OnDestroy()
        {
            GameEvents.OnUpdateScore -= UpdateScoreDisplay;
            
        }

        private void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
                scoreText.text = $"Current Points: {score}";
        }
    }
}