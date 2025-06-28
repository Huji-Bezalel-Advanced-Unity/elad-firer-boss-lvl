using _SPC.Core.BaseScripts.Managers;
using _SPC.Core.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    /// <summary>
    /// Handles the rendering and display of the current gameplay score in the UI.
    /// </summary>
    public class GameplayScoreRenderer : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("TextMeshPro component for displaying the current score.")]
        [SerializeField] private TextMeshProUGUI scoreText;

        private GameplayScore _gameplayScore;

        /// <summary>
        /// Initializes the score renderer and subscribes to score update events.
        /// </summary>
        private void Start()
        {
            SubscribeToScoreEvents();
            InitializeScoreDisplay();
        }

        /// <summary>
        /// Unsubscribes from score events when the component is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            UnsubscribeFromScoreEvents();
        }

        /// <summary>
        /// Updates the score display with the new score value.
        /// </summary>
        /// <param name="score">The new score value to display.</param>
        private void UpdateScoreDisplay(long score)
        {
            if (scoreText != null)
            {
                scoreText.text = score.ToString();
            }
        }

        /// <summary>
        /// Subscribes to score update events from the game events system.
        /// </summary>
        private void SubscribeToScoreEvents()
        {
            GameEvents.OnUpdateScore += UpdateScoreDisplay;
        }

        /// <summary>
        /// Unsubscribes from score update events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromScoreEvents()
        {
            GameEvents.OnUpdateScore -= UpdateScoreDisplay;
        }

        /// <summary>
        /// Initializes the score display with a default value of 0.
        /// </summary>
        private void InitializeScoreDisplay()
        {
            UpdateScoreDisplay(0);
        }
    }
}