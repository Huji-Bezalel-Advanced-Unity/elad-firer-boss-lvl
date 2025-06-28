using _SPC.Core.BaseScripts.Managers;
using _SPC.Core.Scripts.Managers;

namespace _SPC.GamePlay.Score
{
    /// <summary>
    /// Manages the player's score during gameplay, including score tracking, updates, and combo system integration.
    /// </summary>
    public class GameplayScore
    {
        private long _score;
        
        /// <summary>
        /// Gets the current score value.
        /// </summary>
        public long Score => _score;
        
        /// <summary>
        /// Gets the combinator system for handling score multipliers and combos.
        /// </summary>
        public GameplayCombinator Combinator { get; }

        /// <summary>
        /// Initializes the gameplay score system with event subscriptions.
        /// </summary>
        public GameplayScore()
        {
            _score = 0;
            GameEvents.OnGameStarted += ResetScore;
            Combinator = new GameplayCombinator(this);
        }

        /// <summary>
        /// Resets the score to zero and notifies the game events system.
        /// </summary>
        public void ResetScore()
        {
            _score = 0;
            GameEvents.UpdateScore(_score);
        }

        /// <summary>
        /// Adds the specified amount to the current score, ensuring it never goes below zero.
        /// </summary>
        /// <param name="amount">The amount to add to the score (can be negative for penalties).</param>
        /// <returns>The actual amount added to the score after bounds checking.</returns>
        public long AddScore(long amount)    
        {
            long oldScore = _score;
            _score += amount;
            
            if (_score < 0)
                _score = 0;
                
            long actualAdded = _score - oldScore;
            GameEvents.UpdateScore(_score);
            return actualAdded;
        }

        /// <summary>
        /// Updates the combinator system to process any pending score events.
        /// </summary>
        public void UpdateCombinator()
        {
            Combinator.UpdateCombinator();
        }
    }
}