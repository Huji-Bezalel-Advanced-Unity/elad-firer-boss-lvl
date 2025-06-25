using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using DG.Tweening;

namespace _SPC.GamePlay.Entities.Enemies.Destroyer
{
    public class DestroyerAttacker: SPCAttacker
    {
        private readonly DestroyerStats _stats;
        private bool _attack = false;

        public DestroyerAttacker(DestroyerStats destroyerStats, AttackerDependencies deps) : base(deps)
        {
            _stats = destroyerStats;
        }

        public override void Attack()
        {
            if (_attack) return;
            _attack = true;
            
            if (!ProjectilePools.TryGetValue(WeaponType.DestroyerBullet, out var pool))
            {
                Logger.LogWarning("No Pool for DestroyerBullet");
                return;
            }
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
            
            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate,()=>
            {
                _attack = false;
            });
        }

        public override void CleanUp()
        {
            
        }
    }
}