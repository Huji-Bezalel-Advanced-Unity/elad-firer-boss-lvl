using _SPC.Core.Audio;
using _SPC.Core.BaseScripts.Generics.MonoSingletone;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Text;
using _SPC.GamePlay.Score;
using UnityEngine;

namespace _SPC.Core.BaseScripts.Managers
{
    public class GameManager : SpcMonoSingleton<GameManager>
    {
        private GameplayScore _gameplayScore;
        private HighScoreManager _highScoreManager;
        public SceneLoader sceneLoader;
        private GameCheat gameCheat;

        public HighScoreManager HighScoreManager => _highScoreManager;

        public void OnEnable()
        {
            GameEvents.OnGameFinished += OnGameFinished;
            GameEvents.OnGameStarted += OnGameStarted;
            GameEvents.OnGameLoss += OnGameLoss;
        }

        private void OnGameStarted()
        {
            AudioManager.Instance.Play(AudioName.GamePlayMusic, Vector3.zero);
        }

        public void OnDisable()
        {
            GameEvents.OnGameFinished -= OnGameFinished;
            GameEvents.OnGameLoss -= OnGameLoss;
        }

        private void OnGameLoss()
        {
            AudioManager.Instance.StopAll();
            GameEvents.GamePaused();
            AudioManager.Instance.Play(AudioName.GameOverMusic, Vector3.zero, () => sceneLoader.LoadSceneWithCallback(3,
                () =>
                {
                    GameEvents.GameResumed();
                    AudioManager.Instance.Play(AudioName.GameLossMusicScene, Vector3.zero);
                }));
        }

        
        public void Start()
        {
            sceneLoader ??= new SceneLoader(this);
            _highScoreManager ??= new HighScoreManager();
            _gameplayScore ??= new GameplayScore();
            gameCheat ??= new GameCheat();
        }

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

        void LateUpdate()
        {
            _gameplayScore.UpdateCombinator();
        }
    }
}