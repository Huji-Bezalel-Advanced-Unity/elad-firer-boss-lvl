using System.Collections.Generic;
using _SPC.Core.BaseScripts.BaseMono;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies
{
    /// <summary>
    /// Base class for all enemy entities, providing common functionality for health, attacks, and collision handling.
    /// </summary>
    public abstract class SpcBaseEnemy: SPCBaseMono, IHitable
    {
        [Header("Attacker")]
        [Tooltip("Prefab for explosion effects when the enemy is hit.")]
        [SerializeField] protected GameObject explosionPrefab;
        
        [Tooltip("Parent transform for organizing explosion effects.")]
        [SerializeField] protected Transform _explosionsFather;
        
        [Tooltip("Pool for managing bullet objects.")]
        [SerializeField] protected BulletMonoPool bulletPool;
        
        [Tooltip("List of potential targets for the enemy.")]
        [SerializeField] protected List<Transform> transformTargets = new List<Transform>();
        
        [Tooltip("Primary target transform for the enemy.")]
        [SerializeField] protected Transform targetTransform;
        
        [Tooltip("Logger for enemy-specific debug messages.")]
        [SerializeField] protected GameLogger enemyLogger;
        
        [Tooltip("Collider component for collision detection.")]
        [SerializeField] protected Collider2D collider;
        
        [Header("UI")] 
        [Tooltip("Health bar UI component for displaying enemy health.")]
        [SerializeField] protected HealthBarUI healthBarUI;
        
        protected SPCHealth _health;
        protected SPCAttacker _attacker;

        /// <summary>
        /// Registers this enemy with the game events system.
        /// </summary>
        private void Start()
        {
            RegisterEnemy();
        }

        /// <summary>
        /// Unregisters this enemy from the game events system.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterEnemy();
        }

        /// <summary>
        /// Handles the enemy being hit by a projectile.
        /// </summary>
        /// <param name="projectileTransform">Position where the projectile hit.</param>
        /// <param name="weaponType">Type of weapon that hit the enemy.</param>
        public virtual void GotHit(Vector3 projectileTransform, WeaponType weaponType)
        {
            ApplyDamage(weaponType);
            CreateExplosionEffect(projectileTransform);
            HandlePlayerBulletHit(weaponType, projectileTransform);
        }

        /// <summary>
        /// Handles collision with other objects that implement IHitable.
        /// </summary>
        /// <param name="other">The collision information.</param>
        public void OnCollisionEnter2D(Collision2D other)
        {
            var hit = other.gameObject.GetComponentInParent<IHitable>();
            if (hit != null)
            {
                hit.GotHit(transform.position, WeaponType.EnemyBody);
            }
        }

        /// <summary>
        /// Initializes the base enemy with common dependencies.
        /// </summary>
        /// <param name="mainTarget">Primary target for the enemy.</param>
        /// <param name="targets">List of potential targets.</param>
        /// <param name="explosionPrefab">Prefab for explosion effects.</param>
        /// <param name="explosionsFather">Parent transform for explosions.</param>
        /// <param name="pool">Bullet pool for the enemy.</param>
        protected virtual void Init(Transform mainTarget, List<Transform> targets, GameObject explosionPrefab, Transform explosionsFather, BulletMonoPool pool)
        {
            SetTargets(mainTarget, targets);
            SetExplosionSettings(explosionPrefab, explosionsFather);
            SetBulletPool(pool);
        }

        /// <summary>
        /// Registers this enemy with the game events system.
        /// </summary>
        private void RegisterEnemy()
        {
            GameEvents.EnemyAdded(transform);
        }

        /// <summary>
        /// Unregisters this enemy from the game events system.
        /// </summary>
        private void UnregisterEnemy()
        {
            GameEvents.EnemyRemoved(transform);
        }

        /// <summary>
        /// Applies damage to the enemy based on the weapon type.
        /// </summary>
        /// <param name="weaponType">Type of weapon that caused the damage.</param>
        private void ApplyDamage(WeaponType weaponType)
        {
            _health.ReduceLife(SPCAttacker.damage[weaponType]);
        }

        /// <summary>
        /// Creates an explosion effect at the specified position.
        /// </summary>
        /// <param name="position">Position where the explosion should occur.</param>
        private void CreateExplosionEffect(Vector3 position)
        {
            Instantiate(explosionPrefab, position, Quaternion.identity, _explosionsFather);
        }

        /// <summary>
        /// Handles special logic for player bullet hits.
        /// </summary>
        /// <param name="weaponType">Type of weapon that hit.</param>
        /// <param name="position">Position of the hit.</param>
        private void HandlePlayerBulletHit(WeaponType weaponType, Vector3 position)
        {
            if (weaponType == WeaponType.PlayerBullet)
            {
                GameEvents.PlayerHit(position);
            }
        }

        /// <summary>
        /// Sets the target information for the enemy.
        /// </summary>
        /// <param name="mainTarget">Primary target transform.</param>
        /// <param name="targets">List of potential targets.</param>
        private void SetTargets(Transform mainTarget, List<Transform> targets)
        {
            targetTransform = mainTarget;
            transformTargets = targets;
        }

        /// <summary>
        /// Sets the explosion-related settings for the enemy.
        /// </summary>
        /// <param name="explosionPrefab">Explosion effect prefab.</param>
        /// <param name="explosionsFather">Parent transform for explosions.</param>
        private void SetExplosionSettings(GameObject explosionPrefab, Transform explosionsFather)
        {
            this.explosionPrefab = explosionPrefab;
            _explosionsFather = explosionsFather;
        }

        /// <summary>
        /// Sets the bullet pool for the enemy.
        /// </summary>
        /// <param name="pool">Bullet pool to use.</param>
        private void SetBulletPool(BulletMonoPool pool)
        {
            bulletPool = pool;
        }
    }
}