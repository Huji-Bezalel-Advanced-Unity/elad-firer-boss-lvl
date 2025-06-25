using _SPC.Core.BaseScripts.Generics.MonoSingletone;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Text;
using _SPC.GamePlay.Score;
using UnityEngine;

namespace _SPC.Core.BaseScripts.Managers
{
    public class GameManager: SpcMonoSingleton<GameManager>
    {
        private  GameplayScore _gameplayScore;
        private  HighScoreManager _highScoreManager;
        public SceneLoader sceneLoader;
        
        public HighScoreManager HighScoreManager => _highScoreManager;

        public void OnEnable()
        {
            GameEvents.OnGameFinished += OnGameFinished;
            GameEvents.OnGameLoss += OnGameLoss;
        }

        private void OnGameLoss()
        {
            sceneLoader.LoadSceneWithCallback(3);
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
            _highScoreManager.TryAddHighScore(_gameplayScore.Score, PlayerPrefs.GetString(NameInputUI.PlayerPrefsName));
            sceneLoader.LoadSceneWithCallback(2,()=>GameEvents.EndSceneStarted());
        }
        
        void LateUpdate()
        {
            _gameplayScore.UpdateCombinator();
        }
    }
}