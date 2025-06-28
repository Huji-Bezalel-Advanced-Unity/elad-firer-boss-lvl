using _SPC.Core.Audio;
using _SPC.Core.BaseScripts.Generics.MonoSingletone;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Text;
using _SPC.GamePlay.Score;
using UnityEngine;

namespace _SPC.Core.BaseScripts.Managers
{
    /// <summary>
    /// Central manager for game state, scene transitions, and high score management.
    /// </summary>
    public class GameManager : SpcMonoSingleton<GameManager>
    {
        private GameplayScore _gameplayScore;
        private HighScoreManager _highScoreManager;
        public SceneLoader sceneLoader;
        private GameCheat _gameCheat;

        /// <summary>
        /// Gets the high score manager instance.
        /// </summary>
        public HighScoreManager HighScoreManager => _highScoreManager;

        /// <summary>
        /// Subscribes to game events on enable.
        /// </summary>
        public void OnEnable()
        {
            GameEvents.OnGameFinished += OnGameFinished;
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameLoss += OnGameLoss;
        }

        /// <summary>
        /// Unsubscribes from game events on disable.
        /// </summary>
        public void OnDisable()
        {
            GameEvents.OnGameFinished -= OnGameFinished;
            GameEvents.OnGameLoss -= OnGameLoss;
        }

        /// <summary>
        /// Initializes core managers and scene loader.
        /// </summary>
        public void Start()
        {
            sceneLoader ??= new SceneLoader(this);
            _highScoreManager ??= new HighScoreManager();
            _gameplayScore ??= new GameplayScore();
            _gameCheat ??= new GameCheat();
        }

        /// <summary>
        /// Handles logic when the game starts (e.g., play music).
        /// </summary>
        private void OnGameStarted()
        {
            AudioManager.Instance.Play(AudioName.GamePlayMusic, Vector3.zero);
        }

        /// <summary>
        /// Handles logic when the game is lost (e.g., stop music, show loss scene).
        /// </summary>
        private void OnGameLoss()
        {
            AudioManager.Instance.StopAll();
            GameEvents.GamePaused();
            AudioManager.Instance.Play(AudioName.GameOverMusic, Vector3.zero, () =>
                sceneLoader.LoadSceneWithCallback(3, () =>
                {
                    GameEvents.GameResumed();
                    AudioManager.Instance.Play(AudioName.GameLossMusicScene, Vector3.zero);
                }));
        }

        /// <summary>
        /// Handles logic when the game is finished (e.g., update high score, show win scene).
        /// </summary>
        private void OnGameFinished()
        {
            _highScoreManager.TryAddHighScore(_gameplayScore.Score, PlayerPrefs.GetString(NameInputUI.PlayerPrefsName));
            AudioManager.Instance.StopAll();
            GameEvents.GamePaused();
            AudioManager.Instance.Play(AudioName.GameWinMusic, Vector3.zero,
                () => sceneLoader.LoadSceneWithCallback(2, () =>
                {
                    GameEvents.GameResumed();
                    GameEvents.EndSceneStarted();
                    AudioManager.Instance.Play(AudioName.GameWinMusicScene, Vector3.zero);
                }));
        }

        /// <summary>
        /// Updates the gameplay score combinator each frame.
        /// </summary>
        void LateUpdate()
        {
            _gameplayScore.UpdateCombinator();
        }
    }
}