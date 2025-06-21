using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.GamePlay.Weapons.Bullet;
using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _SPC.GamePlay.Enemies.Destroyer.Scripts;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
{
    public struct BossAttackerDependencies
    {
        public BoxCollider2D ArenaCollider;
        public BulletMonoPool DestroyerPool;
        public GameObject ExplosionPrefab;
        public Transform ExplosionsFather;
        public Collider2D BossCollider;
        public Transform DummyParentTransform;
    }

    public class BossAttacker: SPCAttacker
    {
        private readonly BossStats _stats;
        private bool _attack;
        private readonly BossAttackerDependencies _bossDeps;

        public BossAttacker(BossStats stats, AttackerDependencies deps, BossAttackerDependencies bossDeps) : base(deps)
        {
            _stats = stats;
            _attack = false;
            _bossDeps = bossDeps;
            StartSpawning();
        }

        public void NormalAttack()
        {
            if (_attack) return;
            _attack = true;

            if (!ProjectilePools.TryGetValue(WeaponType.BossBullet, out var pool))
            {
                Logger?.Log("EnemyBullet pool not found!");
                return;
            }

            const int bulletCount = 6;
            const float degreesBetween = 360f / bulletCount;
            float randomStartAngle = Random.Range(0f, 360f);

            Vector2 center = EntityTransform.position; 

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = randomStartAngle + i * degreesBetween;
                float rad = angle * Mathf.Deg2Rad;
                
                Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
                Vector2 dummyPosition = center + direction * 10f;

                GameObject tempTargetObject = new GameObject("DummyBulletTarget");
                tempTargetObject.transform.SetParent(_bossDeps.DummyParentTransform);
                tempTargetObject.transform.position = dummyPosition;

                var bullet = pool.Get();
                bullet.Activate(new BulletInitData(
                    WeaponType.BossBullet,
                    tempTargetObject.transform,
                    center,
                    _stats.ProjectileSpeed,
                    _stats.ProjectileBuffer,
                    pool
                ));

                Object.Destroy(tempTargetObject, 0.2f);
            }

            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate,()=>
            {
                _attack = false;
            });
        }
        
        public void StartSpawning()
        {
            AttackerMono.StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);
                var bounds = _bossDeps.ArenaCollider.bounds;
                List<Vector2> spawnPositions = new List<Vector2>();
                int maxTries = _stats.distanceBetweenEnemiesAccuracy;
                float minDistance = _stats.minDistanceBetweenEnemies;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = Vector2.zero;
                    bool positionFound = false;
                    int tries = 0;
                    while (!positionFound && tries < maxTries)
                    {
                        tries++;
                        pos = new Vector2(
                            Random.Range(bounds.min.x, bounds.max.x),
                            Random.Range(bounds.min.y, bounds.max.y)
                        );

                        if (_bossDeps.BossCollider.OverlapPoint(pos))
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
                            MainTarget = MainTarget,
                            Targets = new List<Transform>(TargetTransforms),
                            ExplosionPrefab = _bossDeps.ExplosionPrefab,
                            ExplosionsFather = _bossDeps.ExplosionsFather,
                            Pool = _bossDeps.DestroyerPool,
                            ArenaBounds = _bossDeps.ArenaCollider
                        };
                        
                        destroyer.Init(destroyerDeps);
                    }
                }
            }
        }
    }
}