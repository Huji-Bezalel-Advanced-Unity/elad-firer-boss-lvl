using _SPC.Core.Audio;
using _SPC.Core.BaseScripts.InputSystem.Scripts;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.GamePlay.Entities.Player
{
    /// <summary>
    /// Handles player attack logic, including input processing and projectile spawning.
    /// </summary>
    public sealed class PlayerAttacker : SPCAttacker
    {
        private readonly InputSystem_Actions _inputSystem;
        private bool _attack;
        private readonly PlayerStats _stats;

        /// <summary>
        /// Initializes the player attacker with stats and dependencies.
        /// </summary>
        /// <param name="stats">Player statistics for attack configuration.</param>
        /// <param name="dependencies">Dependencies required for the attacker.</param>
        public PlayerAttacker(PlayerStats stats, AttackerDependencies dependencies) : base(dependencies) 
        {
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.Attack.performed += OnAttackPerformed;
            _stats = stats;
        }

        /// <summary>
        /// Executes the attack logic when input is received.
        /// </summary>
        public override void Attack()
        {
            if (!_attack) return;
            
            PlayAttackSound();
            FireMultipleShots();
            ResetAttackFlag();
        }

        /// <summary>
        /// Cleans up input subscriptions when the attacker is destroyed.
        /// </summary>
        public override void CleanUp()
        {
            _inputSystem.Player.Attack.performed -= OnAttackPerformed;
        }

        /// <summary>
        /// Handles attack input from the player.
        /// </summary>
        /// <param name="obj">Input action callback context.</param>
        private void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            _attack = true;
        }

        /// <summary>
        /// Fires multiple projectiles based on the number of shots configured.
        /// </summary>
        private void FireMultipleShots()
        {
            for (int i = 0; i < _stats.NumberOfShots; i++)
            {
                FireSingleShot(i);
            }
        }

        /// <summary>
        /// Fires a single projectile at the specified shot index.
        /// </summary>
        /// <param name="shotIndex">Index of the shot for spacing calculations.</param>
        private void FireSingleShot(int shotIndex)
        {
            var target = GetClosestTarget();
            if (target == null) return;

            var projectile = GetProjectileFromPool();
            if (projectile == null) return;

            var spawnPosition = CalculateSpawnPosition(shotIndex);
            ActivateProjectile(projectile, target, spawnPosition);
        }

        /// <summary>
        /// Gets the closest target from the available targets.
        /// </summary>
        /// <returns>The closest target transform, or null if no targets available.</returns>
        private Transform GetClosestTarget()
        {
            return UsedAlgorithms.GetClosestTarget(TargetTransforms, EntityTransform);
        }

        /// <summary>
        /// Gets a projectile from the pool.
        /// </summary>
        /// <returns>The projectile GameObject, or null if pool is empty.</returns>
        private Bullet GetProjectileFromPool()
        {
            return ProjectilePools[WeaponType.PlayerBullet].Get();
        }

        /// <summary>
        /// Calculates the spawn position for a projectile based on shot index and spacing.
        /// </summary>
        /// <param name="shotIndex">Index of the shot for spacing calculations.</param>
        /// <returns>The calculated spawn position.</returns>
        private Vector2 CalculateSpawnPosition(int shotIndex)
        {
            var target = GetClosestTarget();
            if (target == null) return EntityTransform.position;

            Vector2 fireDirection = (target.position - EntityTransform.position).normalized;
            Vector2 perpendicular = new Vector2(-fireDirection.y, fireDirection.x);
            
            float totalWidth = (_stats.NumberOfShots - 1) * _stats.ShotSpacing;
            float offset = (_stats.NumberOfShots > 1) ? (shotIndex * _stats.ShotSpacing) - (totalWidth / 2f) : 0;
            
            return (Vector2)EntityTransform.position + perpendicular * offset;
        }

        /// <summary>
        /// Activates a projectile with the specified target and spawn position.
        /// </summary>
        /// <param name="projectile">The projectile to activate.</param>
        /// <param name="target">The target for the projectile.</param>
        /// <param name="spawnPosition">The spawn position for the projectile.</param>
        private void ActivateProjectile(Bullet projectile, Transform target, Vector2 spawnPosition)
        {
            projectile.Activate(new BulletInitData(
                WeaponType.PlayerBullet,
                target,
                spawnPosition,
                _stats.ProjectileSpeed,
                _stats.ProjectileBuffer,
                ProjectilePools[WeaponType.PlayerBullet]
            ));
        }

        /// <summary>
        /// Plays the attack sound effect.
        /// </summary>
        private void PlayAttackSound()
        {
            AudioManager.Instance.Play(AudioName.PlayerShotMusic, EntityTransform.position);
        }

        /// <summary>
        /// Resets the attack flag after firing.
        /// </summary>
        private void ResetAttackFlag()
        {
            _attack = false;
        }
    }
}