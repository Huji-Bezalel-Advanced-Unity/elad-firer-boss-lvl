using System;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.Core.Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Score
{
    /// <summary>
    /// Handles score combination logic, including multipliers, combos, and event processing for player and enemy hits.
    /// </summary>
    public class GameplayCombinator
    {
        /// <summary>
        /// Types of events that can be processed by the combinator.
        /// </summary>
        private enum EventType
        {
            PlayerHit,
            EnemyHit
        }

        private readonly GameplayScore _gameplayScore;
        private readonly List<KeyValuePair<EventType, Vector3>> _eventsThisFrame = new();
        private readonly float[] _playerPointsMultiplier = { 1.01f, 1.02f, 1.03f };
        private float _currentPlayerPointsMultiplier;

        /// <summary>
        /// Event triggered when points should be rendered at a specific position.
        /// </summary>
        public static event Action<Vector3, long> RenderPoints;

        /// <summary>
        /// Initializes the gameplay combinator with event subscriptions.
        /// </summary>
        /// <param name="gameplayScore">The gameplay score system to interact with.</param>
        public GameplayCombinator(GameplayScore gameplayScore)
        {
            _gameplayScore = gameplayScore;
            SubscribeToGameStateEvents();
            ResetPlayerMultiplier();
        }

        /// <summary>
        /// Updates the combinator system, processing all events from the current frame.
        /// </summary>
        public void UpdateCombinator()
        {
            var eventCounts = CountEvents();
            UpdateComboMultiplier(eventCounts.playerHitCount > 0);
            if (_eventsThisFrame.Count == 0) return;

            ProcessEnemyHits(eventCounts.enemyHitCount);
            ProcessPlayerHits(eventCounts.playerHitCount);
            
            _eventsThisFrame.Clear();
        }

        /// <summary>
        /// Handles enemy hit events by adding them to the current frame's event list.
        /// </summary>
        /// <param name="hitPosition">The position where the enemy was hit.</param>
        private void OnEnemyHit(Vector3 hitPosition)
        {
            _eventsThisFrame.Add(new KeyValuePair<EventType, Vector3>(EventType.EnemyHit, hitPosition));
        }

        /// <summary>
        /// Handles player hit events by adding them to the current frame's event list.
        /// </summary>
        /// <param name="hitPosition">The position where the player was hit.</param>
        private void OnPlayerHit(Vector3 hitPosition)
        {
            _eventsThisFrame.Add(new KeyValuePair<EventType, Vector3>(EventType.PlayerHit, hitPosition));
        }

        /// <summary>
        /// Subscribes to game state events for proper lifecycle management.
        /// </summary>
        private void SubscribeToGameStateEvents()
        {
            GameEvents.OnGameStarted += SubscribeToGameplayEvents;
            GameEvents.OnGameFinished += UnsubscribeFromGameplayEvents;
            GameEvents.OnGameLoss += UnsubscribeFromGameplayEvents;
        }

        /// <summary>
        /// Subscribes to gameplay events for hit detection.
        /// </summary>
        private void SubscribeToGameplayEvents()
        {
            GameEvents.OnPlayerHit += OnPlayerHit;
            GameEvents.OnEnemyHit += OnEnemyHit;
        }

        /// <summary>
        /// Unsubscribes from gameplay events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromGameplayEvents()
        {
            GameEvents.OnPlayerHit -= OnPlayerHit;
            GameEvents.OnEnemyHit -= OnEnemyHit;
        }

        /// <summary>
        /// Counts the number of each type of event in the current frame.
        /// </summary>
        /// <returns>A tuple containing the count of player hits and enemy hits.</returns>
        private (int playerHitCount, int enemyHitCount) CountEvents()
        {
            int playerHitCount = 0;
            int enemyHitCount = 0;
            
            foreach (var evt in _eventsThisFrame)
            {
                if (evt.Key == EventType.PlayerHit) playerHitCount++;
                if (evt.Key == EventType.EnemyHit) enemyHitCount++;
            }
            
            return (playerHitCount, enemyHitCount);
        }

        /// <summary>
        /// Processes enemy hit events, applying penalties and resetting multipliers.
        /// </summary>
        /// <param name="enemyHitCount">Number of enemy hits in the current frame.</param>
        private void ProcessEnemyHits(int enemyHitCount)
        {
            if (enemyHitCount <= 0) return;

            ResetPlayerMultiplier();

            foreach (var evt in _eventsThisFrame)
            {
                if (evt.Key == EventType.EnemyHit)
                {
                    long penalty = CalculateEnemyHitPenalty();
                    long actual = _gameplayScore.AddScore(-penalty);
                    TriggerPointsRender(evt.Value, actual);
                }
            }
        }

        /// <summary>
        /// Processes player hit events, applying bonuses and updating multipliers.
        /// </summary>
        /// <param name="playerHitCount">Number of player hits in the current frame.</param>
        private void ProcessPlayerHits(int playerHitCount)
        {
            if (playerHitCount <= 0) return;

            float currentTime = Time.time;
            
            foreach (var evt in _eventsThisFrame)
            {
                if (evt.Key == EventType.PlayerHit)
                {
                    long points = CalculatePlayerHitPoints();
                    long actual = _gameplayScore.AddScore(points);
                    TriggerPointsRender(evt.Value, actual);
                }
            }
        }

        /// <summary>
        /// Updates the combo multiplier based on hit events.
        /// </summary>
        /// <param name="playerHitThisFrame">Whether a player hit occurred this frame.</param>
        private void UpdateComboMultiplier(bool playerHitThisFrame)
        {
            if (playerHitThisFrame)
            {
                IncreasePlayerMultiplier();
            }
        }

        /// <summary>
        /// Calculates the penalty for enemy hits based on current score.
        /// </summary>
        /// <returns>The calculated penalty amount.</returns>
        private long CalculateEnemyHitPenalty()
        {
            return 300 + _gameplayScore.Score / 100;
        }

        /// <summary>
        /// Calculates the points for player hits using the current multiplier.
        /// </summary>
        /// <returns>The calculated points amount.</returns>
        private long CalculatePlayerHitPoints()
        {
            return (long)(100 * _currentPlayerPointsMultiplier);
        }
        

        /// <summary>
        /// Resets the player points multiplier to a random value.
        /// </summary>
        private void ResetPlayerMultiplier()
        {
            _currentPlayerPointsMultiplier = Random.Range(_playerPointsMultiplier[0], _playerPointsMultiplier[^1]);
        }

        /// <summary>
        /// Increases the player points multiplier by a random factor.
        /// </summary>
        private void IncreasePlayerMultiplier()
        {
            _currentPlayerPointsMultiplier *= Random.Range(_playerPointsMultiplier[0], _playerPointsMultiplier[^1]);
        }

        /// <summary>
        /// Triggers the RenderPoints event to display points at the specified position.
        /// </summary>
        /// <param name="position">World position where points should be rendered.</param>
        /// <param name="points">Amount of points to display.</param>
        private static void TriggerPointsRender(Vector3 position, long points)
        {
            RenderPoints?.Invoke(position, points);
        }
    }
} 