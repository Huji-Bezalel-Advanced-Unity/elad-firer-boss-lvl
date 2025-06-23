using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Managers;
using _SPC.GamePlay.Enemies.BaseEnemy;
using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;
using _SPC.GamePlay.Enemies.Destroyer.Scripts;
using _SPC.GamePlay.Weapons.Bullet;
using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Boss.Scripts
{
    public class BossController : SPCBaseEnemy
    {
        [Header("Stats")] [SerializeField] private BossStats stats;
        [SerializeField] private DestroyerStats destroyerStats;
        [SerializeField] private BoxCollider2D arenaCollider;
        [SerializeField] private BulletMonoPool destroyerPool;
        [SerializeField] private Transform dummyParentTransform;
        
        
        private SPCStatsUpgrader _statsUpgrader;
        private bool _isPaused = false;
        private SpcBasicAiModule _aiModule;

        private void OnEnable()
        {
            GameEvents.OnGamePaused += () => _isPaused = true;
            GameEvents.OnGameResumed += () => _isPaused = false;
        }

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
            _aiModule = new BossAiStatsChooser(enemyLogger);
            _aiModule.Fit();
            var upgraderDeps = new BossStatsUpgraderDependencies
            {
                Stats = stats,
                DestroyerStats = destroyerStats,
                Logger = enemyLogger,
                OnBossUpgradedActions = new Action[] { OnBossUpgraded },
                AiModule = _aiModule
            };
            _statsUpgrader = new BossStatsUpgrader(upgraderDeps);
        }

        private void OnBossUpgraded()
        {
            transform.DOPunchScale(Vector3.one * stats.upgradePunchIntensity, stats.upgradePunchTime);
        }

        private void OnDisable()
        {
            _statsUpgrader?.ResetStats();
        }

        private void Update()
        {
            if (_isPaused) return;
            _attacker.Attack();
        }
    }
}