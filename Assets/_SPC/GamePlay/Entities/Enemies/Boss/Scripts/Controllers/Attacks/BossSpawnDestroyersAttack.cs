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

    public class BossSpawnDestroyersAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossSpawnDestroyersAttackDependencies _deps;
        private bool _isPaused = false;
        private bool _isSpawning = false;
       
        public BossSpawnDestroyersAttack(BossStats stats, BossSpawnDestroyersAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        private void OnGamePaused()
        {
            _isPaused = true;
        }

        private void OnGameResumed()
        {
            _isPaused = false;
        }
        

        public override bool Attack(Action onFinished = null)
        {
            _deps.AttackerMono.StartCoroutine(SpawnEnemiesSequentially(onFinished));
            return true;
        }

        private IEnumerator SpawnEnemiesSequentially(Action onFinished)
        {
            var bounds = _deps.ArenaCollider.bounds;
            List<Vector2> spawnPositions = new List<Vector2>();
            float minDistance = _stats.minDistanceBetweenEnemies;

            // First, find all valid spawn positions
            for (int i = 0; i < _stats.numberOfEnemiesToSpawn; i++)
            {
                Vector2 pos = Vector2.zero;
                bool positionFound = false;
                int tries = 0;
                while (!positionFound && tries < _stats.distanceBetweenEnemiesAccuracy)
                {
                    tries++;
                    pos = new Vector2(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y)
                    );

                    if (_deps.BossCollider.OverlapPoint(pos))
                    {
                        continue;
                    }

                    bool tooClose = false;
                    foreach (var other in spawnPositions)
                    {
                        if (Vector2.Distance(pos, other) < minDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose)
                    {
                        continue;
                    }

                    positionFound = true;
                }

                if (positionFound)
                {
                    spawnPositions.Add(pos);
                }
            }

            // Now spawn destroyers one by one with delays
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                if (_isPaused)
                {
                    yield return new WaitUntil(() => !_isPaused);
                }

                // Spawn the destroyer
                Vector2 pos = spawnPositions[i];
                GameObject destroyerObj = Object.Instantiate(_stats.DestroyerPrefab, pos, Quaternion.identity);
                var destroyer = destroyerObj.GetComponent<DestroyerController>();

                var destroyerDeps = new DestroyerDependencies
                {
                    MainTarget = _deps.MainTarget,
                    Targets = new List<Transform>(_deps.TargetTransforms),
                    ExplosionPrefab = _deps.ExplosionPrefab,
                    ExplosionsFather = _deps.ExplosionsFather,
                    Pool = _deps.DestroyerPool,
                    ArenaBounds = _deps.ArenaCollider
                };
                
                destroyer.Init(destroyerDeps);

                // Wait before spawning next destroyer (except for the last one)
                if (i < spawnPositions.Count - 1)
                {
                    yield return new WaitForSeconds(_stats.destroyerSpawnTime);
                }
            }

            _isSpawning = false;
            onFinished?.Invoke();
        }
    }
} 