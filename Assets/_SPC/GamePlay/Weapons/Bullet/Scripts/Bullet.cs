using _SPC.Core.BaseScripts.Generics.MonoPool;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    /// <summary>
    /// Data structure containing all initialization parameters for a bullet.
    /// Used to configure bullet behavior and movement properties.
    /// </summary>
    public struct BulletInitData
    {
        public WeaponType weaponType;
        public Transform target;
        public Vector2 startPosition;
        public float speed;
        public float buffer;
        public float? smoothFactor; // Nullable — only used by tracking bullets
        public float? turnSpeed;
        public BulletMonoPool pool;

        /// <summary>
        /// Initializes a new BulletInitData instance with the specified parameters.
        /// </summary>
        /// <param name="weaponType">Type of weapon for damage calculation.</param>
        /// <param name="target">Target transform to aim at.</param>
        /// <param name="startPosition">Starting position of the bullet.</param>
        /// <param name="speed">Movement speed of the bullet.</param>
        /// <param name="buffer">Distance buffer from start position.</param>
        /// <param name="pool">Pool to return the bullet to when deactivated.</param>
        /// <param name="smoothFactor">Smoothing factor for tracking bullets (optional).</param>
        /// <param name="turnSpeed">Turn speed for tracking bullets (optional).</param>
        public BulletInitData(WeaponType weaponType, Transform target, Vector2 startPosition,
                          float speed, float buffer, BulletMonoPool pool, float? smoothFactor = null, float? turnSpeed = null)
        {
            this.weaponType = weaponType;
            this.target = target;
            this.startPosition = startPosition;
            this.speed = speed;
            this.buffer = buffer;
            this.pool = pool;
            this.smoothFactor = smoothFactor;
            this.turnSpeed = turnSpeed;
        }
    }

    /// <summary>
    /// Base bullet class that handles movement, collision detection, and pool management.
    /// Supports pause/resume functionality and automatic return to pool on collision.
    /// </summary>
    public class Bullet : SPCBaseWeapon, IPoolable
    {
        [Header("Bullet Components")]
        [Tooltip("Rigidbody2D component for physics-based movement.")]
        [SerializeField] protected Rigidbody2D rb2D;

        [Header("Bullet State")]
        [Tooltip("Pool that manages this bullet instance.")]
        protected BulletMonoPool _pool;
        
        [Tooltip("Whether the bullet is currently active and moving.")]
        protected bool _active;
        
        [Tooltip("Current movement direction of the bullet.")]
        protected Vector2 _currentDirection;
        
        [Tooltip("Target transform the bullet is moving towards.")]
        protected Transform _target;
        
        [Tooltip("Movement speed of the bullet.")]
        protected float _speed;
        
        [Tooltip("Saved velocity for pause/resume functionality.")]
        private Vector2 _savedVelocity;

        /// <summary>
        /// Called when the game is paused. Saves current velocity and stops movement.
        /// </summary>
        protected override void OnGamePaused()
        {
            base.OnGamePaused();
            if (rb2D != null && rb2D.linearVelocity != Vector2.zero)
            {
                _savedVelocity = rb2D.linearVelocity;
                rb2D.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Called when the game is resumed. Restores saved velocity and resumes movement.
        /// </summary>
        protected override void OnGameResumed()
        {
            base.OnGameResumed();
            if (rb2D != null)
            {
                rb2D.linearVelocity = _savedVelocity;
            }
        }

        /// <summary>
        /// Activates the bullet with the specified initialization data.
        /// Sets up position, rotation, and movement parameters.
        /// </summary>
        /// <param name="data">Initialization data containing all bullet parameters.</param>
        public virtual void Activate(BulletInitData data)
        {
            if (data.target == null)
            {
                Debug.LogWarning("Bullet activation failed: target is null.");
                return;
            }

            weaponLogger?.Log("Bullet activated");
            _hitTransform = transform;
            _weaponType = data.weaponType;
            _active = true;
            _pool = data.pool;
            _target = data.target;
            _speed = data.speed;

            Vector2 direction = ((Vector2)data.target.position - data.startPosition).normalized;
            _currentDirection = direction;
            Vector2 spawnPosition = data.startPosition + direction * data.buffer;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            transform.position = spawnPosition;
            rb2D.linearVelocity = direction * data.speed;
        }

        /// <summary>
        /// Handles collision detection when the bullet hits a target.
        /// Deactivates the bullet and returns it to the pool.
        /// </summary>
        /// <param name="other">The collider that was hit.</param>
        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (!_active || _isPaused) return;
            
            base.OnTriggerEnter2D(other);
            weaponLogger?.Log($"Bullet triggered by: {other.name}");
            
            // Move bullet off-screen and return to pool
            transform.position = new Vector2(-100, -100);
            weaponLogger?.Log("Bullet returned to pool");
            _pool.Return(this);
        }

        /// <summary>
        /// Resets the bullet state for reuse from the pool.
        /// Clears all references and resets flags.
        /// </summary>
        public virtual void Reset()
        {
            _target = null;
            _hitSuccess = false;
            _active = false;
        }
    }
}