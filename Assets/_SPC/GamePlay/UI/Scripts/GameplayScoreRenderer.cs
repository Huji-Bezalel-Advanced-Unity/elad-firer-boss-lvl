using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Text;
using _SPC.GamePlay.Score;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.UI.Scripts.Scripts
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