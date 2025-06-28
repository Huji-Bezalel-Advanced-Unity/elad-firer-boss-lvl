using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Destroyer
{
    /// <summary>
    /// Holds dependencies for the DestroyerController, including targets, explosion settings, and arena bounds.
    /// </summary>
    public struct DestroyerDependencies
    {
        public Transform MainTarget;
        public List<Transform> Targets;
        public GameObject ExplosionPrefab;
        public Transform ExplosionsFather;
        public BulletMonoPool Pool;
        public BoxCollider2D ArenaBounds;
    }

    /// <summary>
    /// Main controller for destroyer enemies, managing movement, attacks, and health.
    /// </summary>
    public class DestroyerController: SpcBaseEnemy
    {
        [Tooltip("Destroyer statistics and configuration.")]
        [SerializeField] private DestroyerStats stats;
        
        [Header("Sprite Settings")]
        [Tooltip("Sprite renderer for the destroyer.")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Movement")]
        [Tooltip("Rigidbody2D component for physics-based movement.")]
        [SerializeField] private Rigidbody2D rb2D;
        
        [Tooltip("Transform of the spaceship for rotation.")]
        [SerializeField] private Transform spaceshipTransform;

        private SPCMovement _movement;
        private BoxCollider2D _arenaCollider;
        private bool _initialized = false;
        private Coroutine _flashCoroutine;
        private bool _isPaused = false;

        /// <summary>
        /// Subscribes to game pause/resume events.
        /// </summary>
        private void Awake()
        {
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Initializes the destroyer with all required components and dependencies.
        /// </summary>
        public void Init(DestroyerDependencies destroyerDeps)
        {
            _arenaCollider = destroyerDeps.ArenaBounds;

            InitializeBaseEnemy(destroyerDeps);
            InitializeAttacker();
            InitializeMovement();
            InitializeHealth(destroyerDeps);

            _initialized = true;
        }

        /// <summary>
        /// Updates the destroyer's attack and movement logic each frame.
        /// </summary>
        private void Update()
        {
            if (!_initialized || _isPaused) return;
            
            _attacker.Attack();
            _movement.UpdateMovement();
        }

        /// <summary>
        /// Handles damage feedback by flashing the sprite red.
        /// </summary>
        private void FlashCourtine(float health, float damage)
        {
            if (_flashCoroutine != null)
                return;
                
            _flashCoroutine = StartCoroutine(FlashRed());
        }

        /// <summary>
        /// Cleans up resources when the destroyer is disabled.
        /// </summary>
        private void OnDisable()
        {
            _movement?.Cleanup();
        }

        /// <summary>
        /// Subscribes to game pause/resume events.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            GameEvents.OnGamePaused += () => _isPaused = true;
            GameEvents.OnGameResumed += () => _isPaused = false;
        }

        /// <summary>
        /// Initializes the base enemy functionality.
        /// </summary>
        private void InitializeBaseEnemy(DestroyerDependencies destroyerDeps)
        {
            base.Init(destroyerDeps.MainTarget, destroyerDeps.Targets, destroyerDeps.ExplosionPrefab, destroyerDeps.ExplosionsFather, destroyerDeps.Pool);
        }

        /// <summary>
        /// Initializes the destroyer attacker with dependencies.
        /// </summary>
        private void InitializeAttacker()
        {
            var attackerDeps = CreateAttackerDependencies();
            _attacker = new DestroyerAttacker(stats, attackerDeps);
        }

        /// <summary>
        /// Creates the attacker dependencies.
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
                { WeaponType.DestroyerBullet, bulletPool } 
            };
        }

        /// <summary>
        /// Initializes the destroyer movement with dependencies.
        /// </summary>
        private void InitializeMovement()
        {
            var moveDeps = new DestroyerMovementDependencies
            {
                EntityTransform = transform,
                Stats = stats,
                ArenaBounds = _arenaCollider,
                SpaceshipTransform = spaceshipTransform,
                TransformTargets = transformTargets
            };
            _movement = new DestroyerMovement(moveDeps);
        }

        /// <summary>
        /// Initializes the destroyer health system with dependencies.
        /// </summary>
        private void InitializeHealth(DestroyerDependencies destroyerDeps)
        {
            var healthDeps = new HealthDependencies
            {
                healthUI = healthBarUI,
                logger = enemyLogger,
                OnDeathAction = CreateDeathAction(),
                maxHP = stats.Health,
                currentHP = stats.Health,
                OnDamageActions = new List<Action<float, float>>{FlashCourtine}
            };
            _health = new SPCHealth(healthDeps);
        }

        /// <summary>
        /// Creates the death action for the destroyer.
        /// </summary>
        private Action CreateDeathAction()
        {
            return () => Destroy(gameObject);
        }

        /// <summary>
        /// Flashes the sprite red for damage feedback.
        /// </summary>
        private IEnumerator FlashRed()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            _flashCoroutine = null;
        }
    }
}