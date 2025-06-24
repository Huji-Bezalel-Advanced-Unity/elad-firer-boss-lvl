using System;
using System.Collections.Generic;
using _SPC.Core.Scripts.Managers;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    public class GameplayCombinator
    {
        private enum EventType
        {
            PlayerHit,
            EnemyHit
        }

        private readonly GameplayScore _gameplayScore;
        private readonly List<KeyValuePair<EventType,Vector3>> _eventsThisFrame = new();
        private float _comboTimer = 0f;
        private float _playerPointsMultiplier = 1.07f;
        private const float ComboResetTime = 0.12f;
        private float _lastPlayerHitTime = -1f;

        public GameplayCombinator(GameplayScore gameplayScore)
        {
            _gameplayScore = gameplayScore;
            GameEvents.OnGameStarted += SubscribeEvents;
            GameEvents.OnGameFinished += UnsubscribeEvents;
            GameEvents.OnGameLoss += UnsubscribeEvents;
            
        }

        private void SubscribeEvents()
        {
            // Subscribe to events
            GameEvents.OnPlayerHit += OnPlayerHit;
            GameEvents.OnEnemyHit += OnEnemyHit;

        }

        private void OnEnemyHit(Vector3 obj)
        {
            _eventsThisFrame.Add(new KeyValuePair<EventType, Vector3>(EventType.EnemyHit, obj));
        }

        private void UnsubscribeEvents()
        {
            // Unsubscribe from events
            GameEvents.OnGameStarted -= SubscribeEvents;
            GameEvents.OnGameFinished -= UnsubscribeEvents;
            GameEvents.OnPlayerHit -= OnPlayerHit;
            GameEvents.OnEnemyHit -= OnEnemyHit;
        }

        private void OnPlayerHit(Vector3 hitPoint)
        {
            _eventsThisFrame.Add(new KeyValuePair<EventType, Vector3>(EventType.PlayerHit,hitPoint));
        }
        
        
        public void UpdateCombinator()
        {
            int playerHitCount = 0;
            int enemyHitCount = 0;
            float currentTime = Time.time;
            bool enemyHitThisFrame = false;
            bool playerHitThisFrame = false;
            foreach (var evt in _eventsThisFrame)
            {
                if (evt.Key == EventType.PlayerHit) playerHitCount++;
                if (evt.Key == EventType.EnemyHit) enemyHitCount++;
            }

            // If enemy hit, reset multiplier
            if (enemyHitCount > 0)
            {
                _playerPointsMultiplier = 1.07f;
                _lastPlayerHitTime = -1f;
                enemyHitThisFrame = true;
            }

            foreach (var evt in _eventsThisFrame)
            {
                if (evt.Key == EventType.PlayerHit)
                {
                    playerHitThisFrame = true;
                    long points = (long)(100 * _playerPointsMultiplier);
                    long actual = _gameplayScore.AddScore(points);
                    OnRenderPoints(evt.Value, actual);
                    _lastPlayerHitTime = currentTime;
                }
            }

            // If no player hit in the last 0.2s, reset multiplier
            if (_lastPlayerHitTime > 0 && currentTime - _lastPlayerHitTime > ComboResetTime)
            {
                _playerPointsMultiplier = 1.07f;
            }
            else if(playerHitThisFrame)
            {
                _playerPointsMultiplier *= 1.07f;
            }

            foreach (var evt in _eventsThisFrame)
            {
                if (evt.Key == EventType.EnemyHit)
                {
                    long penalty = 300 + _gameplayScore.Score / 100;
                    long actual = _gameplayScore.AddScore(-penalty);
                    OnRenderPoints(evt.Value, actual);
                }
            }

            _eventsThisFrame.Clear();
        }

        public static event Action<Vector3, long> RenderPoints;

        private static void OnRenderPoints(Vector3 arg1, long arg2)
        {
            RenderPoints?.Invoke(arg1, arg2);
        }
    }
} 