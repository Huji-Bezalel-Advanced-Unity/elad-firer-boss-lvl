using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    /// <summary>
    /// UI component for displaying individual high score entries with rank, nickname, and score.
    /// </summary>
    public class HighScoreEntryUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Text component for displaying the rank position.")]
        [SerializeField] private TextMeshProUGUI _rankText;
        
        [Tooltip("Text component for displaying the player's nickname.")]
        [SerializeField] private TextMeshProUGUI _nameText;
        
        [Tooltip("Text component for displaying the score value.")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        /// <summary>
        /// Initializes the high score entry UI with the provided data.
        /// </summary>
        /// <param name="rank">The rank position of this entry.</param>
        /// <param name="nickname">The player's nickname.</param>
        /// <param name="score">The score value.</param>
        public void Initialize(int rank, string nickname, long score)
        {
            SetRankText(rank);
            SetNameText(nickname);
            SetScoreText(score);
        }

        /// <summary>
        /// Sets the rank text with the provided rank value.
        /// </summary>
        /// <param name="rank">The rank to display.</param>
        private void SetRankText(int rank)
        {
            if (_rankText != null)
            {
                _rankText.text = rank.ToString();
            }
        }

        /// <summary>
        /// Sets the name text with the provided nickname.
        /// </summary>
        /// <param name="nickname">The nickname to display.</param>
        private void SetNameText(string nickname)
        {
            if (_nameText != null)
            {
                _nameText.text = nickname;
            }
        }

        /// <summary>
        /// Sets the score text with the provided score value.
        /// </summary>
        /// <param name="score">The score to display.</param>
        private void SetScoreText(long score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = score.ToString();
            }
        }
    }
} 