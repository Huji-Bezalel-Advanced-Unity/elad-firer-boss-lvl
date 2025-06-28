using System;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Entities.Enemies.Destroyer;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Main controller for the boss entity, managing health, attacks, and upgrades.
    /// </summary>
    public class BossController : SpcBaseEnemy
    {
        [Header("Stats")] 
        [Tooltip("Boss statistics and configuration.")]
        [SerializeField] private BossStats stats;
        
        [Tooltip("Stats for destroyer enemies spawned by the boss.")]
        [SerializeField] private DestroyerStats destroyerStats;
        
        [Tooltip("Collider defining the arena boundaries.")]
        [SerializeField] private BoxCollider2D arenaCollider;
        
        [Tooltip("Pool for destroyer bullet objects.")]
        [SerializeField] private BulletMonoPool destroyerPool;
        
        [Tooltip("Parent transform for dummy objects.")]
        [SerializeField] private Transform dummyParentTransform;
        
        [Tooltip("Transform for laser positioning.")]
        [SerializeField] private Transform laserTransform;
        
        [Tooltip("Pool for big bullet objects.")]
        [SerializeField] private BulletMonoPool bigBulletPool;
        
        [Tooltip("Prefab for laser attack.")]
        [SerializeField] private GameObject laserPrefab;

        [Header("Face")]
        [Tooltip("Sprite renderer for the boss face.")]
        [SerializeField] private SpriteRenderer faceSpriteRenderer;
        
        [Tooltip("Normal face sprite.")]
        [SerializeField] private Sprite normalFaceSprite;
        
        [Tooltip("Rage face sprite.")]
        [SerializeField] private Sprite rageFaceSprite;

        private SPCStatsUpgrader _statsUpgrader;
        private bool _isPaused = false;

        /// <summary>
        /// Initializes the boss controller with health, attacker, and stats upgrader.
        /// </summary>
        private void Start()
        {
            InitializeHealth();
            InitializeAttacker();
            InitializeStatsUpgrader();
        }

        /// <summary>
        /// Handles boss upgrade visual feedback.
        /// </summary>
        private void OnBossUpgraded()
        {
            transform.DOPunchScale(Vector3.one * stats.upgradePunchIntensity, stats.upgradePunchTime);
        }

        /// <summary>
        /// Cleans up resources when the boss is disabled.
        /// </summary>
        private void OnDisable()
        {
            _statsUpgrader?.ResetStats();
            _attacker?.CleanUp();
        }

        /// <summary>
        /// Updates the boss attack logic each frame.
        /// </summary>
        private void Update()
        {
            _attacker.Attack();
        }

        /// <summary>
        /// Initializes the boss health system with dependencies.
        /// </summary>
        private void InitializeHealth()
        {
            var healthDeps = new HealthDependencies
            {
                healthUI = healthBarUI,
                logger = enemyLogger,
                OnDeathAction = CreateDeathAction(),
                maxHP = stats.Health,
                currentHP = stats.Health,
            };
            
            _health = new SPCHealth(healthDeps);
        }

        /// <summary>
        /// Creates the death action for the boss.
        /// </summary>
        private Action CreateDeathAction()
        {
            return () => transform.DOPunchScale(Vector3.one * stats.upgradePunchIntensity * 4, stats.upgradePunchTime)
                .OnComplete(GameEvents.GameFinished);
        }

        /// <summary>
        /// Initializes the boss attacker with all required dependencies.
        /// </summary>
        private void InitializeAttacker()
        {
            var attackerDeps = CreateAttackerDependencies();
            var bossDeps = CreateBossAttackerDependencies();
            
            _attacker = new BossAttacker(stats, attackerDeps, bossDeps);
        }

        /// <summary>
        /// Creates the base attacker dependencies.
        /// </summary>
        private AttackerDependencies CreateAttackerDependencies()
        {
            return new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = CreateProjectilePools(),
                TargetTransforms = transformTargets,
                Logger = enemyLogger,
                AttackerMono = this
            };
        }

        /// <summary>
        /// Creates the projectile pools dictionary.
        /// </summary>
        private Dictionary<WeaponType, BulletMonoPool> CreateProjectilePools()
        {
            return new Dictionary<WeaponType, BulletMonoPool>
            { 
                { WeaponType.BossBullet, bulletPool }, 
                { WeaponType.BossBigBullet, bigBulletPool } 
            };
        }

        /// <summary>
        /// Creates the boss-specific attacker dependencies.
        /// </summary>
        private BossAttackerDependencies CreateBossAttackerDependencies()
        {
            var faceChanger = CreateFaceChanger();
            
            return new BossAttackerDependencies
            {
                ArenaCollider = arenaCollider,
                DestroyerPool = destroyerPool,
                ExplosionPrefab = explosionPrefab,
                ExplosionsFather = _explosionsFather,
                BossCollider = collider,
                DummyParentTransform = dummyParentTransform,
                Health = _health,
                LaserPrefab = laserPrefab,
                LaserTransform = laserTransform,
                FaceChanger = faceChanger
            };
        }

        /// <summary>
        /// Creates the boss face changer with sprite dependencies.
        /// </summary>
        private BossFaceChanger CreateFaceChanger()
        {
            var faceChangerDeps = new BossFaceChangerDependencies
            {
                BossSpriteRenderer = faceSpriteRenderer,
                NormalFaceSprite = normalFaceSprite,
                AngryFaceSprite = rageFaceSprite
            };
            
            return new BossFaceChanger(faceChangerDeps);
        }

        /// <summary>
        /// Initializes the boss stats upgrader with dependencies.
        /// </summary>
        private void InitializeStatsUpgrader()
        {
            var upgraderDeps = new BossStatsUpgraderDependencies
            {
                Stats = stats,
                DestroyerStats = destroyerStats,
                Logger = enemyLogger,
                OnBossUpgradedActions = new Action[] { OnBossUpgraded },
            };
            
            _statsUpgrader = new BossStatsUpgrader(upgraderDeps);
        }
    }
}