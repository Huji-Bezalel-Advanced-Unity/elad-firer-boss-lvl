using System;
using UnityEngine;

namespace _SPC.Core.Scripts.Managers
{
    public static class GameEvents
    {
        public static event Action<Transform> OnEnemyAdded;
        public static event Action<Transform> OnEnemyRemoved;
        public static event Action<Vector3> OnEnemyHit;
        public static event Action OnGameStarted;
        public static event Action OnGameFinished;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action<Vector3> OnPlayerHit;
        public static event Action<long> OnUpdateScore;
        public static event Action OnEndSceneStarted;

        public static void UpdateScore(long score)
        {
            OnUpdateScore?.Invoke(score);
        }
        
        public static void GameStarted()
        {
            OnGameStarted?.Invoke();
        }

        public static void PlayerHit(Vector3 position)
        {
            OnPlayerHit?.Invoke(position);
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

        public static void EnemyHit(Vector3 position)
        {
            OnEnemyHit?.Invoke(position);
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