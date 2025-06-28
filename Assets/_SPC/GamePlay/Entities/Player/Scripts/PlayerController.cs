using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.Audio;
using _SPC.Core.BaseScripts.BaseMono;
using _SPC.Core.BaseScripts.Managers;
using UnityEngine;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;

namespace _SPC.GamePlay.Entities.Player
{
    /// <summary>
    /// Main controller for the player entity, managing movement, attacks, health, and upgrade systems.
    /// </summary>
    public sealed class PlayerController : SPCBaseMono, IHitable
    {
        private static readonly int Flame = Animator.StringToHash("Flame");

        [Header("Movement")]
        [Tooltip("Rigidbody2D component for physics-based movement.")]
        [SerializeField] private Rigidbody2D rb2D;
        private SPCMovement _movement;

        [Header("Data")]
        [Tooltip("Collider component for collision detection.")]
        [SerializeField] private Collider2D entityCollider;

        [Header("Animations")]
        [Tooltip("Animator component for player animations.")]
        [SerializeField] private Animator animator;

        [Tooltip("Flame effect GameObject for movement feedback.")]
        [SerializeField] private GameObject flame;
        
        [Tooltip("Sprite renderer for visual feedback effects.")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Tooltip("Transform of the spaceship for rotation.")]
        [SerializeField] private Transform spaceshipTransform;

        [Header("Stats")]
        [Tooltip("Player statistics and configuration.")]
        [SerializeField] private PlayerStats stats;
        
        [Tooltip("Logger for player-specific debug messages.")]
        [SerializeField] private GameLogger playerLogger;

        [Header("Attacker")]
        [Tooltip("Primary target transform for the player.")]
        [SerializeField] private Transform targetTransform;
        
        [Tooltip("Pool for managing player bullet objects.")]
        [SerializeField] private BulletMonoPool playerPool;
        
        [Tooltip("List of potential targets for the player.")]
        [SerializeField] internal List<Transform> transformTargets = new List<Transform>();
        private SPCAttacker _attacker;
        private SPCHealth _health;

        [Header("UI")]
        [Tooltip("Health bar UI component for displaying player health.")]
        [SerializeField] private HealthBarUI healthBarUI;
        
        [Tooltip("Renderer for upgrade choice UI.")]
        [SerializeField] private PlayerUpgradeChooseRenderer upgradeRenderer;

        private Coroutine _flashCoroutine;
        private TargetsHandler _targetsHandler;
        private SPCStatsUpgrader _statsUpgrader;
        private bool _isPaused = false;

        /// <summary>
        /// Initializes the targets handler for managing enemy targets.
        /// </summary>
        private void Awake()
        {
            _targetsHandler = new TargetsHandler(this);
        }

        /// <summary>
        /// Subscribes to game pause/resume events.
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }
        
        /// <summary>
        /// Cleans up resources and resets stats when the player is disabled.
        /// </summary>
        private void OnDisable()
        {
            _statsUpgrader?.ResetStats();
            _movement?.Cleanup();
            _attacker?.CleanUp();
        }
        
        /// <summary>
        /// Initializes all player systems with dependencies.
        /// </summary>
        private void Start()
        {
            InitializeMovement();
            InitializeAttacker();
            InitializeHealth();
            InitializeStatsUpgrader();
        }

        /// <summary>
        /// Updates player systems each frame when not paused.
        /// </summary>
        private void Update()
        {
            if (_isPaused) return;

            _attacker.Attack();
            _movement.UpdateMovement();
            HandleFlame();
        }

        /// <summary>
        /// Handles the player being hit by a projectile.
        /// </summary>
        /// <param name="projectileTransform">Position where the projectile hit.</param>
        /// <param name="weaponType">Type of weapon that hit the player.</param>
        public void GotHit(Vector3 projectileTransform, WeaponType weaponType)
        {
            PlayHitSound();
            TriggerHitEvents(projectileTransform);
            ApplyDamage(weaponType);
        }

        /// <summary>
        /// Handles flame animation based on movement state.
        /// </summary>
        private void HandleFlame()
        {
            if (!_movement.IsMoving)
            {
                DisableFlameAnimation();
            }
            else
            {
                EnableFlameAnimation();
            }
        }

        /// <summary>
        /// Handles damage feedback by flashing the sprite red.
        /// </summary>
        /// <param name="health">Current health value.</param>
        /// <param name="damage">Amount of damage taken.</param>
        private void FlashCourtine(float health, float damage)
        {
            if (_flashCoroutine != null)
                return;
            _flashCoroutine = StartCoroutine(FlashRed());
        }

        /// <summary>
        /// Coroutine for flashing the sprite red when taking damage.
        /// </summary>
        private IEnumerator FlashRed()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            _flashCoroutine = null;
        }

        /// <summary>
        /// Initializes the player movement system.
        /// </summary>
        private void InitializeMovement()
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
        }

        /// <summary>
        /// Initializes the player attacker system.
        /// </summary>
        private void InitializeAttacker()
        {
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<WeaponType, BulletMonoPool> { { WeaponType.PlayerBullet, playerPool } },
                TargetTransforms = transformTargets,
                Logger = playerLogger,
                AttackerMono = this
            };
            _attacker = new PlayerAttacker(stats, deps);
        }

        /// <summary>
        /// Initializes the player health system.
        /// </summary>
        private void InitializeHealth()
        {
            var healthDeps = new HealthDependencies(playerLogger, healthBarUI, null, GameEvents.GameLoss,
                stats.Health, stats.Health, new List<Action<float, float>> { FlashCourtine });
            _health = new SPCHealth(healthDeps);
        }

        /// <summary>
        /// Initializes the player stats upgrader system.
        /// </summary>
        private void InitializeStatsUpgrader()
        {
            _statsUpgrader = new PlayerStatsUpgrader(stats, _health, upgradeRenderer);
        }

        /// <summary>
        /// Plays the hit sound effect.
        /// </summary>
        private void PlayHitSound()
        {
            AudioManager.Instance.Play(AudioName.EnemySuccessfulShotMusic, transform.position);
        }

        /// <summary>
        /// Triggers hit-related events.
        /// </summary>
        /// <param name="projectileTransform">Position of the hit.</param>
        private void TriggerHitEvents(Vector3 projectileTransform)
        {
            GameEvents.EnemyHit(projectileTransform);
        }

        /// <summary>
        /// Applies damage to the player based on the weapon type.
        /// </summary>
        /// <param name="weaponType">Type of weapon that caused the damage.</param>
        private void ApplyDamage(WeaponType weaponType)
        {
            _health.ReduceLife(SPCAttacker.damage[weaponType]);
        }

        /// <summary>
        /// Disables the flame animation.
        /// </summary>
        private void DisableFlameAnimation()
        {
            animator.SetBool(Flame, false);
        }

        /// <summary>
        /// Enables the flame animation.
        /// </summary>
        private void EnableFlameAnimation()
        {
            animator.SetBool(Flame, true);
            flame.SetActive(true);
        }

        /// <summary>
        /// Handles game pause event.
        /// </summary>
        private void OnGamePaused()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Handles game resume event.
        /// </summary>
        private void OnGameResumed()
        {
            _isPaused = false;
        }
    }

    /// <summary>
    /// Handles target management for the player, tracking enemies as they spawn and despawn.
    /// </summary>
    internal class TargetsHandler
    {
        private readonly PlayerController _playerController;

        /// <summary>
        /// Initializes the targets handler and subscribes to game events.
        /// </summary>
        /// <param name="playerController">The player controller to manage targets for.</param>
        public TargetsHandler(PlayerController playerController)
        {
            _playerController = playerController;
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Removes an enemy transform from the player's target list.
        /// </summary>
        /// <param name="enemy">The enemy transform to remove.</param>
        private void RemoveTransform(Transform enemy)
        {
            _playerController.transformTargets.Remove(enemy);
        }

        /// <summary>
        /// Adds a target transform to the player's target list.
        /// </summary>
        /// <param name="target">The target transform to add.</param>
        private void AddTransform(Transform target)
        {
            _playerController.transformTargets.Add(target);
        }

        /// <summary>
        /// Unsubscribes from all game events.
        /// </summary>
        private void DisableEvents()
        {
            GameEvents.OnEnemyRemoved -= RemoveTransform;
            GameEvents.OnEnemyAdded -= AddTransform;
            GameEvents.OnGameFinished -= DisableEvents;
            GameEvents.OnGameLoss -= DisableEvents;
        }

        /// <summary>
        /// Subscribes to game events for target management.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            GameEvents.OnEnemyRemoved += RemoveTransform;
            GameEvents.OnEnemyAdded += AddTransform;
            GameEvents.OnGameFinished += DisableEvents;
            GameEvents.OnGameLoss += DisableEvents;
        }
    }
}