using _SPC.Core.Scripts.Abstracts;
using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Destroyer.Scripts.Controllers
{
    public class DestroyerAttacker: SPCAttacker
    {
        private readonly DestroyerStats _stats;
        private bool _attack = false;

        public DestroyerAttacker(DestroyerStats destroyerStats, AttackerDependencies deps) : base(deps)
        {
            _stats = destroyerStats;
        }

        public void NormalAttack()
        {
            if (_attack) return;
            _attack = true;
            
            if (!ProjectilePools.TryGetValue(WeaponType.DestroyerBullet, out var pool))
            {
                Logger.LogWarning("No Pool for DestroyerBullet");
                return;
            }
            var bullet = pool.Get();
            Vector2 startPosition = EntityTransform.position;
            Vector2 targetPosition = MainTarget.position;
            float speed = _stats.ProjectileSpeed;
            float buffer = _stats.ProjectileBuffer;
            bullet.Activate(WeaponType.DestroyerBullet, targetPosition, startPosition, speed, buffer, pool);
            
            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate,()=>
            {
                _attack = false;
            });
        }
    }
}