using System;
using UnityEngine;

namespace _SPC.Core.BaseScripts.Managers
{
    /// <summary>
    /// Static class for broadcasting and handling global game events.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Triggered when the game is lost.</summary>
        public static event Action OnGameLoss;
        /// <summary>Triggered when an enemy is added.</summary>
        public static event Action<Transform> OnEnemyAdded;
        /// <summary>Triggered when an enemy is removed.</summary>
        public static event Action<Transform> OnEnemyRemoved;
        /// <summary>Triggered when an enemy is hit.</summary>
        public static event Action<Vector3> OnEnemyHit;
        /// <summary>Triggered when the game starts.</summary>
        public static event Action OnGameStarted;
        /// <summary>Triggered when the game is finished.</summary>
        public static event Action OnGameFinished;
        /// <summary>Triggered when the game is paused.</summary>
        public static event Action OnGamePaused;
        /// <summary>Triggered when the game is resumed.</summary>
        public static event Action OnGameResumed;
        /// <summary>Triggered when the player is hit.</summary>
        public static event Action<Vector3> OnPlayerHit;
        /// <summary>Triggered when the score is updated.</summary>
        public static event Action<long> OnUpdateScore;
        /// <summary>Triggered when the end scene starts.</summary>
        public static event Action OnEndSceneStarted;

        /// <summary>Broadcasts a score update event.</summary>
        public static void UpdateScore(long score) => OnUpdateScore?.Invoke(score);
        /// <summary>Broadcasts a game started event.</summary>
        public static void GameStarted() => OnGameStarted?.Invoke();
        /// <summary>Broadcasts a player hit event.</summary>
        public static void PlayerHit(Vector3 position) => OnPlayerHit?.Invoke(position);
        /// <summary>Broadcasts a game finished event.</summary>
        public static void GameFinished() => OnGameFinished?.Invoke();
        /// <summary>Broadcasts a game paused event.</summary>
        public static void GamePaused() => OnGamePaused?.Invoke();
        /// <summary>Broadcasts a game resumed event.</summary>
        public static void GameResumed() => OnGameResumed?.Invoke();
        /// <summary>Broadcasts an end scene started event.</summary>
        public static void EndSceneStarted() => OnEndSceneStarted?.Invoke();
        /// <summary>Broadcasts an enemy hit event.</summary>
        public static void EnemyHit(Vector3 position) => OnEnemyHit?.Invoke(position);
        /// <summary>Broadcasts an enemy added event.</summary>
        public static void EnemyAdded(Transform enemy) => OnEnemyAdded?.Invoke(enemy);
        /// <summary>Broadcasts an enemy removed event.</summary>
        public static void EnemyRemoved(Transform enemy) => OnEnemyRemoved?.Invoke(enemy);
        /// <summary>Broadcasts a game loss event.</summary>
        public static void GameLoss() => OnGameLoss?.Invoke();
    }
}