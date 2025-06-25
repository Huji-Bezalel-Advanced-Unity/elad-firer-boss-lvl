using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Destroyer
{
    public struct DestroyerDependencies
    {
        public Transform MainTarget;
        public List<Transform> Targets;
        public GameObject ExplosionPrefab;
        public Transform ExplosionsFather;
        public BulletMonoPool Pool;
        public BoxCollider2D ArenaBounds;
    }

    public class DestroyerController: SpcBaseEnemy
    {
        [SerializeField] private DestroyerStats stats;
        private SPCMovement _movement;
        private BoxCollider2D _arenaCollider;
        private bool _initialized = false;
        private Coroutine _flashCoroutine;
        private bool _isPaused = false;
        [Header("Sprite Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb2D;
        [SerializeField] private Transform spaceshipTransform;

        private void Awake()
        {
            GameEvents.OnGamePaused += () => _isPaused = true;
            GameEvents.OnGameResumed += () => _isPaused = false;
        }

        public void Init(DestroyerDependencies destroyerDeps)
        {
            _arenaCollider = destroyerDeps.ArenaBounds;

            base.Init(destroyerDeps.MainTarget, destroyerDeps.Targets, destroyerDeps.ExplosionPrefab, destroyerDeps.ExplosionsFather, destroyerDeps.Pool);
            
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<WeaponType, BulletMonoPool> { { WeaponType.DestroyerBullet, bulletPool } },
                TargetTransforms = transformTargets,
                Logger = enemyLogger,
                AttackerMono = this
            };
            _attacker = new DestroyerAttacker(stats, deps);
            
            var moveDeps = new DestroyerMovementDependencies
            {
                EntityTransform = transform,
                Stats = stats,
                ArenaBounds = _arenaCollider,
                SpaceshipTransform = spaceshipTransform,
                TransformTargets = transformTargets
            };
            _movement = new DestroyerMovement(moveDeps);

            _initialized = true;

            var healthDeps = new HealthDependencies
            {
                healthUI = healthBarUI,
                logger = enemyLogger,
                OnDeathAction = (() =>
                {
                    Destroy(gameObject);
                }),
                maxHP = stats.Health,
                currentHP = stats.Health,
                OnDamageActions = new List<Action<float, float>>{FlashCourtine}
            };
            _health = new SPCHealth(healthDeps);
        }

        private void Update()
        {
            if (!_initialized || _isPaused) return;
            _attacker.Attack();
            _movement.UpdateMovement();
        }
        

        private void FlashCourtine(float health, float damage)
        {
            if (_flashCoroutine != null)
                return;
            _flashCoroutine = StartCoroutine(FlashRed());
        }
        

        private IEnumerator FlashRed()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            _flashCoroutine = null;
        }

        private void OnDisable()
        {
            _movement?.Cleanup();
        }
    }
}