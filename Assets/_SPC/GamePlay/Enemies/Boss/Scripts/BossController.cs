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
        [SerializeField] private Transform dummyParentTransform;
        

        private BossAttacker _attacker;
        

        private void Start()
        {
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<WeaponType, BulletMonoPool> { { WeaponType.BossBullet, bulletPool } },
                TargetTransforms = transformTargets,
                Logger = enemyLogger,
                AttackerMono = this
            };

            var bossDeps = new BossAttackerDependencies
            {
                ArenaCollider = arenaCollider,
                DestroyerPool = destroyerPool,
                ExplosionPrefab = explosionPrefab,
                ExplosionsFather = _explosionsFather,
                BossCollider = collider,
                DummyParentTransform = dummyParentTransform
            };

            _attacker = new BossAttacker(stats, deps, bossDeps);
        }

        private void Update()
        {
            _attacker.NormalAttack();
        }
    }
}