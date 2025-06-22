using System.Collections.Generic;
using _SPC.Core.Scripts.Managers;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    public class GameplayCombinator
    {
        private enum EventType
        {
            PlayerHit
        }

        private readonly GameplayScore _gameplayScore;
        private readonly List<EventType> _eventsThisFrame = new();

        public GameplayCombinator(GameplayScore gameplayScore)
        {
            _gameplayScore = gameplayScore;
            GameEvents.OnGameStarted += CreateGameplayCombinatorUpdater;
            GameEvents.OnGameFinished += DestroyGameplayCombinatorUpdater;
        }

        private void CreateGameplayCombinatorUpdater()
        {
            // Subscribe to events
            GameEvents.OnPlayerHit += OnPlayerHit;

          
        }

        private void DestroyGameplayCombinatorUpdater()
        {
            // Unsubscribe from events
            GameEvents.OnPlayerHit -= OnPlayerHit;
        }

        private void OnPlayerHit()
        {
            _eventsThisFrame.Add(EventType.PlayerHit);
        }
        
        public void UpdateCombinator()
        {
            int playerHitCount = 0;
            foreach (var evt in _eventsThisFrame)
            {
                if (evt == EventType.PlayerHit) playerHitCount++;
            }
            _gameplayScore.AddScore(playerHitCount*100);
            _eventsThisFrame.Clear();
        }
        
    }
} 