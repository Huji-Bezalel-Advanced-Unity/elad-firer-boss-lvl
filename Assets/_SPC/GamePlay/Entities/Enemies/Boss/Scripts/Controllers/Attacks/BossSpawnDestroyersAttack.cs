using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Entities.Enemies.Destroyer;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies for the BossSpawnDestroyersAttack, including arena, pool, and spawn parameters.
    /// </summary>
    public struct BossSpawnDestroyersAttackDependencies
    {
        public BoxCollider2D ArenaCollider;
        public BulletMonoPool DestroyerPool;
        public GameObject ExplosionPrefab;
        public Transform ExplosionsFather;
        public Collider2D BossCollider;
        public Transform MainTarget;
        public List<Transform> TargetTransforms;
        public MonoBehaviour AttackerMono;
    }

    /// <summary>
    /// Handles spawning destroyer enemies in the arena with proper spacing and positioning.
    /// </summary>
    public class BossSpawnDestroyersAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossSpawnDestroyersAttackDependencies _deps;
        private bool _isPaused = false;
        private bool _isSpawning = false;
       
        /// <summary>
        /// Initializes the BossSpawnDestroyersAttack with stats and dependencies.
        /// </summary>
        public BossSpawnDestroyersAttack(BossStats stats, BossSpawnDestroyersAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Handles game pause events.
        /// </summary>
        private void OnGamePaused()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Handles game resume events.
        /// </summary>
        private void OnGameResumed()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Executes the destroyer spawning attack.
        /// </summary>
        public override bool Attack(Action onFinished = null)
        {
            _deps.AttackerMono.StartCoroutine(SpawnEnemiesSequentially(onFinished));
            return true;
        }

        /// <summary>
        /// Spawns destroyers sequentially with proper positioning and delays.
        /// </summary>
        private IEnumerator SpawnEnemiesSequentially(Action onFinished)
        {
            var bounds = _deps.ArenaCollider.bounds;
            List<Vector2> spawnPositions = FindValidSpawnPositions(bounds);

            yield return SpawnDestroyersAtPositions(spawnPositions);
            
            _isSpawning = false;
            onFinished?.Invoke();
        }

        /// <summary>
        /// Subscribes to game pause/resume events.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        /// <summary>
        /// Finds valid spawn positions for destroyers within the arena bounds.
        /// </summary>
        private List<Vector2> FindValidSpawnPositions(Bounds bounds)
        {
            List<Vector2> spawnPositions = new List<Vector2>();
            float minDistance = _stats.minDistanceBetweenEnemies;

            for (int i = 0; i < _stats.numberOfEnemiesToSpawn; i++)
            {
                Vector2 position = FindValidPosition(bounds, spawnPositions, minDistance);
                if (position != Vector2.zero)
                {
                    spawnPositions.Add(position);
                }
            }

            return spawnPositions;
        }

        /// <summary>
        /// Finds a valid spawn position that doesn't overlap with existing positions or the boss.
        /// </summary>
        private Vector2 FindValidPosition(Bounds bounds, List<Vector2> existingPositions, float minDistance)
        {
            int tries = 0;
            while (tries < _stats.distanceBetweenEnemiesAccuracy)
            {
                Vector2 pos = GenerateRandomPosition(bounds);
                
                if (IsValidSpawnPosition(pos, existingPositions, minDistance))
                {
                    return pos;
                }
                
                tries++;
            }
            
            return Vector2.zero;
        }

        /// <summary>
        /// Generates a random position within the arena bounds.
        /// </summary>
        private Vector2 GenerateRandomPosition(Bounds bounds)
        {
            return new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
        }

        /// <summary>
        /// Checks if a position is valid for spawning (not overlapping with boss or other destroyers).
        /// </summary>
        private bool IsValidSpawnPosition(Vector2 pos, List<Vector2> existingPositions, float minDistance)
        {
            if (_deps.BossCollider.OverlapPoint(pos))
            {
                return false;
            }

            foreach (var other in existingPositions)
            {
                if (Vector2.Distance(pos, other) < minDistance)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Spawns destroyers at the given positions with delays between spawns.
        /// </summary>
        private IEnumerator SpawnDestroyersAtPositions(List<Vector2> spawnPositions)
        {
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                yield return WaitForUnpaused();

                Vector2 pos = spawnPositions[i];
                SpawnDestroyer(pos);

                if (i < spawnPositions.Count - 1)
                {
                    yield return new WaitForSeconds(_stats.destroyerSpawnTime);
                }
            }
        }

        /// <summary>
        /// Waits until the game is no longer paused.
        /// </summary>
        private IEnumerator WaitForUnpaused()
        {
            if (_isPaused)
            {
                yield return new WaitUntil(() => !_isPaused);
            }
        }

        /// <summary>
        /// Spawns a single destroyer at the specified position.
        /// </summary>
        private void SpawnDestroyer(Vector2 position)
        {
            GameObject destroyerObj = Object.Instantiate(_stats.DestroyerPrefab, position, Quaternion.identity);
            var destroyer = destroyerObj.GetComponent<DestroyerController>();

            var destroyerDeps = CreateDestroyerDependencies();
            destroyer.Init(destroyerDeps);
        }

        /// <summary>
        /// Creates dependencies for the destroyer controller.
        /// </summary>
        private DestroyerDependencies CreateDestroyerDependencies()
        {
            return new DestroyerDependencies
            {
                MainTarget = _deps.MainTarget,
                Targets = new List<Transform>(_deps.TargetTransforms),
                ExplosionPrefab = _deps.ExplosionPrefab,
                ExplosionsFather = _deps.ExplosionsFather,
                Pool = _deps.DestroyerPool,
                ArenaBounds = _deps.ArenaCollider
            };
        }
    }
} 