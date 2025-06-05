using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Enemies.BaseEnemy;
using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;
using _SPC.GamePlay.Enemies.Destroyer.Scripts;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Boss.Scripts
{
    public class BossController : SPCBaseEnemy
    {
        [Header("Stats")] [SerializeField] private BossStats stats;
        [SerializeField] private BoxCollider2D arenaCollider;
        [SerializeField] private BulletMonoPool destroyerPool;

        private BossAttacker _attacker;

        private void Awake()
        {
            SetBossMissingStats();
        }

        private void SetBossMissingStats()
        {
            stats.ArenaCollider = arenaCollider;
            stats.DestroyerPool = destroyerPool;
        }

        private void Start()
        {
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<WeaponType, BulletMonoPool> { { WeaponType.BossBullet, bulletPool } },
                TargetTransforms = transformTargets,
                Logger = enemyLogger
            };
            _attacker = new BossAttacker(stats, deps);
            StartCoroutine(SpawnEnemies());
        }

        private void Update()
        {
            _attacker.NormalAttack();
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);
                var bounds = stats.ArenaCollider.bounds;
                List<Vector2> spawnPositions = new List<Vector2>();
                int maxTries = 20;
                float minDistance = 2.0f; // Minimum distance between spawns

                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = Vector2.zero;
                    bool valid = false;
                    int tries = 0;
                    while (!valid && tries < maxTries)
                    {
                        pos = new Vector2(
                            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                            UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
                        );
                        valid = true;
                        foreach (var other in spawnPositions)
                        {
                            if (Vector2.Distance(pos, other) < minDistance)
                            {
                                valid = false;
                                break;
                            }
                        }

                        tries++;
                    }

                    spawnPositions.Add(pos);

                    // Instantiate and initialize the destroyer
                    GameObject destroyerObj = Instantiate(stats.DestroyerPrefab, pos, Quaternion.identity);
                    var destroyer = destroyerObj.GetComponent<DestroyerController>();

                    // You need to provide these arguments:
                    // - mainTarget: e.g., targetTransform
                    // - targets: e.g., transformTargets
                    // - explosionPrefab, explosionsFather, bulletPool: from boss or scene
                    destroyer.Init(
                        mainTarget: targetTransform,
                        targets: new List<Transform>(transformTargets),
                        explosionPrefab: explosionPrefab,
                        explosionsFather: _explosionsFather,
                        pool: destroyerPool
                    );
                }
            }
        }
    }
}