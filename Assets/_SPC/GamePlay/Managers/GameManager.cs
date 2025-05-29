using System.Collections;
using _SPC.Core.Scripts.Generics;
using _SPC.GamePlay.Score;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _SPC.GamePlay.Managers
{
    public class GameManager: SPCMonoSingleton<GameManager>
    {
        private  GameplayScore _gameplayScore;
        private  HighScoreManager _highScoreManager;
        
        public HighScoreManager HighScoreManager => _highScoreManager;

        // The single instance, lazily initialized
        
        private static readonly string[] _randomNames = new string[]
        {
            "Pixel", "Whiskers", "Shadow", "Nova", "Milo",
            "Luna", "Ziggy", "Pepper", "Mochi", "Ninja",
            "Jinx", "Maple", "Muffin", "Rocket", "Hazel",
            "Blitz", "Mocha", "Sprout", "Olive", "Cosmo"
        };

        public string CurrentNickname { get; private set; }
        

        // Private constructor prevents external instantiation
        public void Awake()
        {
            _highScoreManager = new HighScoreManager();
            int idx = Random.Range(0, _randomNames.Length);
            CurrentNickname = _randomNames[idx];
            _gameplayScore = new GameplayScore();
            GameEvents.OnGameFinished += OnGameFinished;
            GameEvents.GameStarted();
        }

        private void OnGameFinished()
        {
            _highScoreManager.TryAddHighScore(_gameplayScore.Score, CurrentNickname);
        }
       
    }
}