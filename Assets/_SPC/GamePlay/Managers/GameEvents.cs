using System;
using _SPC.GamePlay.Enemies.BaseEnemy;
using UnityEngine;

namespace _SPC.GamePlay.Managers
{
    public static class GameEvents
    {
        public static event Action<Transform> OnEnemyAdded;
        public static event Action<Transform> OnEnemyRemoved;
        public static event Action<int> OnEnemyHit;
        public static event Action OnGameStarted;
        public static event Action OnGameFinished;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnPlayerHit;
        public static event Action<int> OnUpdateScore;
        public static event Action OnEndSceneStarted;

        public static void UpdateScore(int score)
        {
            OnUpdateScore?.Invoke(score);
        }
        
        public static void GameStarted()
        {
            OnGameStarted?.Invoke();
        }

        public static void PlayerHit()
        {
            OnPlayerHit?.Invoke();
        }

        public static void GameFinished()
        {
            OnGameFinished?.Invoke();
        }

        public static void GamePaused()
        {
            OnGamePaused?.Invoke();
        }

        public static void GameResumed()
        {
            OnGameResumed?.Invoke();
        }

        public static void EndSceneStarted()
        {
            OnEndSceneStarted?.Invoke();
        }

        public static void EnemyHit(int damage)
        {
            OnEnemyHit?.Invoke(damage);
        }

        public static void EnemyAdded(Transform enemy)
        {
            OnEnemyAdded?.Invoke(enemy);
        }

        public static void EnemyRemoved(Transform enemy)
        {
            OnEnemyRemoved?.Invoke(enemy);
        }
    }
}