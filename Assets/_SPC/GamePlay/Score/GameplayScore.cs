using _SPC.Core.Scripts.Managers;

namespace _SPC.GamePlay.Score
{
    public class GameplayScore
    {
        private long _score;
        public long Score => _score;
        public GameplayCombinator Combinator { get; }

        public GameplayScore()
        {
            _score = 0;
            GameEvents.OnGameStarted += ResetScore;
            Combinator = new GameplayCombinator(this);
        }

        public void ResetScore()
        {
            _score = 0;
            GameEvents.UpdateScore(_score);
        }

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

        public void UpdateCombinator()
        {
            Combinator.UpdateCombinator();
        }
    }
}