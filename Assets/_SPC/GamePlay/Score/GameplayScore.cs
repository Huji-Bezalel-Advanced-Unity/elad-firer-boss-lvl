using _SPC.GamePlay.Managers;

namespace _SPC.GamePlay.Score
{
    public class GameplayScore
    {
        private int _score;
        public int Score => _score;
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

        public void AddScore(int amount)
        {
            _score += amount;
            if (_score < 0)
                _score = 0;
            GameEvents.UpdateScore(_score);
        }

        public void UpdateCombinator()
        {
            Combinator.UpdateCombinator();
        }
    }
}