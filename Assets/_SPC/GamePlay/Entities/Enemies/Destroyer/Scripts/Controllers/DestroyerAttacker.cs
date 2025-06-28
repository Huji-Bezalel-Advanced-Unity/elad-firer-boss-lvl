using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using DG.Tweening;

namespace _SPC.GamePlay.Entities.Enemies.Destroyer
{
    /// <summary>
    /// Handles the destroyer's bullet attack logic with cooldown management.
    /// </summary>
    public class DestroyerAttacker: SPCAttacker
    {
        private readonly DestroyerStats _stats;
        private bool _attack = false;

        /// <summary>
        /// Initializes the DestroyerAttacker with stats and dependencies.
        /// </summary>
        public DestroyerAttacker(DestroyerStats destroyerStats, AttackerDependencies deps) : base(deps)
        {
            _stats = destroyerStats;
        }

        /// <summary>
        /// Executes the destroyer's bullet attack with cooldown.
        /// </summary>
        public override void Attack()
        {
            if (_attack) return;
            
            if (!ValidateBulletPool())
            {
                return;
            }

            _attack = true;
            FireBullet();
            ScheduleAttackReset();
        }

        /// <summary>
        /// Cleans up resources when the attacker is destroyed.
        /// </summary>
        public override void CleanUp()
        {
            // No cleanup needed for this attacker
        }

        /// <summary>
        /// Validates that the bullet pool is available.
        /// </summary>
        private bool ValidateBulletPool()
        {
            if (!ProjectilePools.TryGetValue(WeaponType.DestroyerBullet, out var pool))
            {
                Logger.LogWarning("No Pool for DestroyerBullet");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Fires a bullet towards the main target.
        /// </summary>
        private void FireBullet()
        {
            var pool = ProjectilePools[WeaponType.DestroyerBullet];
            var bullet = pool.Get();
            
            bullet.Activate(new BulletInitData(
                WeaponType.DestroyerBullet,
                MainTarget,
                EntityTransform.position,
                _stats.ProjectileSpeed,
                _stats.ProjectileBuffer,
                pool,
                _stats.SmoothFactor,
                _stats.turnSpeed
            ));
        }

        /// <summary>
        /// Schedules the attack reset after the spawn rate delay.
        /// </summary>
        private void ScheduleAttackReset()
        {
            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate, () =>
            {
                _attack = false;
            });
        }
    }
}