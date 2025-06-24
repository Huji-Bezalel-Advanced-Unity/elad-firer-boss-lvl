using System;
using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using System.Collections.Generic;
using _SPC.GamePlay.Enemies.Destroyer.Scripts;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
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
    }

    public class BossSpawnDestroyersAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossSpawnDestroyersAttackDependencies _deps;
       
        public BossSpawnDestroyersAttack(BossStats stats, BossSpawnDestroyersAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
        }
        

        public override bool Attack(Action onFinished = null)
        {
            return SpawnEnemies();
        }

        private bool SpawnEnemies()
        {
            var bounds = _deps.ArenaCollider.bounds;
            List<Vector2> spawnPositions = new List<Vector2>();
            float minDistance = _stats.minDistanceBetweenEnemies;

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
                }
            }
            return true;
        }
        
    }
} 