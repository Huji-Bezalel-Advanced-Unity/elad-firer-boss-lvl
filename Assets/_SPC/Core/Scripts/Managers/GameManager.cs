using System;
using System.Collections;
using _SPC.Core.Scripts.Generics;
using _SPC.GamePlay.Score;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _SPC.Core.Scripts.Managers
{
    public class GameManager: SpcMonoSingleton<GameManager>
    {
        private  GameplayScore _gameplayScore;
        private  HighScoreManager _highScoreManager;
        public SceneLoader sceneLoader;
        
        public HighScoreManager HighScoreManager => _highScoreManager;

        // The single instance, lazily initialized
        
        private static readonly string[] _randomNames = new string[]
        {
            "Pixel", "Whiskers", "Shadow", "Nova", "Milo",
            "Luna", "Ziggy", "Pepper", "Mochi", "Ninja",
            "Jinx", "Maple", "Muffin", "Rocket", "Hazel",
            "Blitz", "Mocha", "Sprout", "Olive", "Cosmo"
        };
        

        public void OnEnable()
        {
            GameEvents.OnGameFinished += OnGameFinished;
        }
        

        // Private constructor prevents external instantiation
        public void Start()
        {
            sceneLoader ??= new SceneLoader(this);
            _highScoreManager ??= new HighScoreManager();
            _gameplayScore ??= new GameplayScore();
            
        }

        private void OnGameFinished()
        {
            _highScoreManager.TryAddHighScore(_gameplayScore.Score, PlayerPrefs.GetString("Nickname"));
            sceneLoader.LoadSceneWithCallback(2,()=>GameEvents.EndSceneStarted());
        }
        
        void LateUpdate()
        {
            _gameplayScore.UpdateCombinator();
        }
    }
}