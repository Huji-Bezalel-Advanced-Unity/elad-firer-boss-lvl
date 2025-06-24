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
    public class BossController : SpcBaseEnemy
    {
        [Header("Stats")] [SerializeField] private BossStats stats;
        [SerializeField] private DestroyerStats destroyerStats;
        [SerializeField] private BoxCollider2D arenaCollider;
        [SerializeField] private BulletMonoPool destroyerPool;
        [SerializeField] private Transform dummyParentTransform;
        [SerializeField] private BulletMonoPool bigBulletPool;

        [Header("Face")]
        [SerializeField] private SpriteRenderer faceSpriteRenderer;
        [SerializeField] private Sprite normalFaceSprite;
        [SerializeField] private Sprite rageFaceSprite;

        private SPCStatsUpgrader _statsUpgrader;
        private bool _isPaused = false;

       
        private void Start()
        {
            var healthDeps = new HealthDependencies
            {
                healthUI = healthBarUI,
                logger = enemyLogger,
                OnDeathAction = (
                    () => transform.DOPunchScale(Vector3.one * stats.upgradePunchIntensity * 4, stats.upgradePunchTime)
                        .OnComplete(GameEvents.GameFinished)),
                maxHP = stats.Health,
                currentHP = stats.Health,
            };
            
            _health = new SPCHealth(healthDeps);
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<WeaponType, BulletMonoPool>
                    { { WeaponType.BossBullet, bulletPool }, { WeaponType.BossBigBullet, bigBulletPool } },
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
                DummyParentTransform = dummyParentTransform,
                Health = _health,
            };

            // Create face changer
            var faceChangerDeps = new BossFaceChangerDependencies
            {
                BossSpriteRenderer = faceSpriteRenderer,
                NormalFaceSprite = normalFaceSprite,
                AngryFaceSprite = rageFaceSprite
            };
            var faceChanger = new BossFaceChanger(faceChangerDeps);
            bossDeps.FaceChanger = faceChanger;

            _attacker = new BossAttacker(stats, deps, bossDeps);
            var upgraderDeps = new BossStatsUpgraderDependencies
            {
                Stats = stats,
                DestroyerStats = destroyerStats,
                Logger = enemyLogger,
                OnBossUpgradedActions = new Action[] { OnBossUpgraded },
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
            _attacker?.CleanUp();
        }

        private void Update()
        {
            _attacker.Attack();
        }


    
    }
}