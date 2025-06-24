using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Utils;
using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;
using _SPC.GamePlay.Player.Scripts.Controllers;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using _SPC.GamePlay.UI.Scripts;
using _SPC.GamePlay.UI.Scripts.Scripts;

namespace _SPC.GamePlay.Player.Scripts
{
    public sealed class PlayerController : SPCBaseMono, IHitable
    {
        private static readonly int Flame = Animator.StringToHash("Flame");

        [Header("Movement")] [SerializeField] private Rigidbody2D rb2D;
        private SPCMovement _movement;

        [Header("Data")] [SerializeField] private Collider2D entityCollider;

        [Header("Animations")] [SerializeField]
        private Animator animator;

        [SerializeField] private GameObject flame;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform spaceshipTransform;


        [Header("Stats")] [SerializeField] private PlayerStats stats;
        [SerializeField] private GameLogger playerLogger;

        [Header("Attacker")] [SerializeField] private Transform targetTransform;
        [SerializeField] private BulletMonoPool playerPool;
        [SerializeField] internal List<Transform> transformTargets = new List<Transform>();
        private SPCAttacker _attacker;
        private SPCHealth _health;

        [Header("UI")] [SerializeField] private HealthBarUI healthBarUI;
        [SerializeField] private PlayerUpgradeChooseRenderer upgradeRenderer;

        private Coroutine _flashCoroutine;
        private TargetsHandler _targetsHandler;
        private SPCStatsUpgrader _statsUpgrader;
        private bool _isPaused = false;

        void Awake()
        {
            _targetsHandler = new TargetsHandler(this);
            
        }

        void OnEnable()
        {
            GameEvents.OnGamePaused += () => _isPaused = true;
            GameEvents.OnGameResumed += () => _isPaused = false;
        }
        
        private void OnDisable()
        {
            _statsUpgrader.ResetStats();
            _movement?.Cleanup();
        }
        
        void Start()
        {
            var moveDeps = new PlayerMovementDependencies
            {
                Rb = rb2D,
                Stats = stats,
                PlayerLogger = playerLogger,
                SpaceshipTransform = spaceshipTransform,
                TransformTargets = transformTargets
            };
            _movement = new PlayerMovement(moveDeps);

            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools =
                    new Dictionary<WeaponType, BulletMonoPool> { { WeaponType.PlayerBullet, playerPool } },
                TargetTransforms = transformTargets,
                Logger = playerLogger,
                AttackerMono = this
            };
            _attacker = new PlayerAttacker(stats, deps);
            var healthDeps = new HealthDependencies(playerLogger, healthBarUI, null, GameEvents.GameLoss,
                stats.Health, stats.Health, new List<Action<float, float>> { FlashCourtine });
            _health = new SPCHealth(healthDeps);
            _statsUpgrader = new PlayerStatsUpgrader(stats, _health, upgradeRenderer);
        }


        public void Update()
        {
            if (_isPaused) return;

            _attacker.Attack();
            _movement.UpdateMovement();
            HandleFlame();
        }

        private void HandleFlame()
        {
            if (!_movement.IsMoving)
            {
                animator.SetBool(Flame, false);
            }
            else
            {
                animator.SetBool(Flame, true);
                flame.SetActive(true);
            }
        }


        public void GotHit(Vector3 projectileTransform, WeaponType weaponType)
        {
            GameEvents.EnemyHit(transform.position);
            _health.ReduceLife(SPCAttacker.damage[weaponType]);
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

        
    
    }

    internal class TargetsHandler
    {
        private readonly PlayerController _playerController;

        public TargetsHandler(PlayerController playerController)
        {
            _playerController = playerController;
            GameEvents.OnEnemyRemoved += RemoveTransform;
            GameEvents.OnEnemyAdded += AddTranform;
            GameEvents.OnGameFinished += DisableEvents;
            GameEvents.OnGameLoss += DisableEvents;
        }

        private void RemoveTransform(Transform enemy)
        {
            _playerController.transformTargets.Remove(enemy);
        }

        private void AddTranform(Transform target)
        {
            _playerController.transformTargets.Add(target);
        }

        private void DisableEvents()
        {
            GameEvents.OnEnemyRemoved -= RemoveTransform;
            GameEvents.OnEnemyAdded -= AddTranform;
        }
    }
}