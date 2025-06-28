using _SPC.Core.BaseScripts.BaseMono;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Entities;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Weapons
{
    /// <summary>
    /// Base class for all weapons in the game. Provides common functionality for weapon behavior,
    /// collision detection, and pause/resume handling.
    /// </summary>
    public class SPCBaseWeapon : SPCBaseMono
    {
        [Header("Weapon Configuration")]
        [Tooltip("Logger component for weapon-specific debug messages.")]
        [SerializeField] protected GameLogger weaponLogger;
        
        [Tooltip("Type of weapon for damage calculation and collision handling.")]
        [SerializeField] protected WeaponType _weaponType;

        [Header("Weapon State")]
        [Tooltip("Whether the weapon successfully hit a target.")]
        protected bool _hitSuccess;
        
        [Tooltip("Current pause state of the weapon.")]
        protected bool _isPaused = false;
        
        [Tooltip("Current target that the weapon is interacting with.")]
        protected IHitable _target;
        
        [Tooltip("Transform of the last hit target.")]
        protected Transform _hitTransform;

        /// <summary>
        /// Called when the weapon becomes active. Subscribes to game pause/resume events.
        /// </summary>
        protected virtual void OnEnable()
        {
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        /// <summary>
        /// Called when the weapon becomes inactive. Unsubscribes from game pause/resume events.
        /// </summary>
        protected virtual void OnDisable()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
        }
        
        /// <summary>
        /// Handles collision detection when the weapon enters a trigger collider.
        /// Attempts to find an IHitable component and applies damage.
        /// </summary>
        /// <param name="other">The collider that was entered.</param>
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null) return;

            var target = other.GetComponentInParent<IHitable>();
            if (target != null)
            {
                _target = target;
                target.GotHit(_hitTransform.position, _weaponType);
                weaponLogger?.Log($"Weapon hit target: {target}");
                _hitSuccess = true;
            }
        }

        /// <summary>
        /// Handles collision detection when the weapon exits a trigger collider.
        /// Clears the current target reference.
        /// </summary>
        /// <param name="other">The collider that was exited.</param>
        public virtual void OnTriggerExit2D(Collider2D other)
        {
            if (_target != null)
            {
                _target = null;
                _hitSuccess = false;
            }
        }

        /// <summary>
        /// Called when the game is resumed. Resets the pause state.
        /// </summary>
        protected virtual void OnGameResumed()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Called when the game is paused. Sets the pause state.
        /// </summary>
        protected virtual void OnGamePaused()
        {
            _isPaused = true;
        }
    }
    
    /// <summary>
    /// Enumeration of all weapon types in the game for damage calculation and collision handling.
    /// </summary>
    public enum WeaponType
    {
        BossBullet,
        BossBigBullet,
        PlayerBullet,
        EnemyBody,
        DestroyerBullet,
        Laser
    }
}