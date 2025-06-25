using _SPC.Core.BaseScripts.Managers;
using _SPC.Core.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    public class GameplayScoreRenderer : MonoBehaviour
    {
        [Header("TextMeshPro References")]
        [SerializeField] private TextMeshProUGUI scoreText;

        private GameplayScore _gameplayScore;

        private void Start()
        {
            GameEvents.OnUpdateScore += UpdateScoreDisplay;
            
            UpdateScoreDisplay(0);
        }

        private void OnDestroy()
        {
            GameEvents.OnUpdateScore -= UpdateScoreDisplay;
            
        }

        private void UpdateScoreDisplay(long score)
        {
            if (scoreText != null)
                scoreText.text = $"{score}";
        }
    }
}