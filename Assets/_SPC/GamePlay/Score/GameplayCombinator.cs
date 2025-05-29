using System.Collections.Generic;
using _SPC.GamePlay.Managers;
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
        private GameObject _updaterGO;
        private GameplayCombinatorUpdater _updater;

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

            if (_updaterGO == null)
            {
                _updaterGO = new GameObject("GameplayCombinatorUpdater");
                _updater = _updaterGO.AddComponent<GameplayCombinatorUpdater>();
                _updater.Init(this);
            }
        }

        private void DestroyGameplayCombinatorUpdater()
        {
            // Unsubscribe from events
            GameEvents.OnPlayerHit -= OnPlayerHit;

            if (_updaterGO != null)
            {
                Object.Destroy(_updaterGO);
                _updaterGO = null;
                _updater = null;
            }
        }

        private void OnPlayerHit()
        {
            _eventsThisFrame.Add(EventType.PlayerHit);
        }
        
        // Should be called once per frame (e.g., from a MonoBehaviour proxy)
        public void LateUpdate()
        {
            int playerHitCount = 0;
            foreach (var evt in _eventsThisFrame)
            {
                if (evt == EventType.PlayerHit) playerHitCount++;
            }
            _gameplayScore.AddScore(playerHitCount*100);
            _eventsThisFrame.Clear();
        }

        // MonoBehaviour proxy for calling LateUpdate
        private class GameplayCombinatorUpdater : MonoBehaviour
        {
            private GameplayCombinator _combinator;
            public void Init(GameplayCombinator combinator)
            {
                _combinator = combinator;
            }
            private void LateUpdate()
            {
                _combinator?.LateUpdate();
            }
        }
    }
} 