using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
{
    public class BossAttacker: SPCAttacker
    {
        private readonly BossStats _stats;
        private bool _attack;

        public BossAttacker(BossStats stats,AttackerDependencies deps) : base(deps)
        {
            _stats = stats;
            _attack = false;
        }

        public void NormalAttack()
        {
            if (_attack) return;
            _attack = true;

            // Get the bullet pool for enemy bullets
            if (!ProjectilePools.TryGetValue(WeaponType.EnemyBullet, out var pool))
            {
                Logger?.Log("EnemyBullet pool not found!");
                return;
            }

            const int bulletCount = 6;
            const float degreesBetween = 360f / bulletCount;
            float randomStartAngle = Random.Range(0f, 360f);

            Vector2 center = EntityTransform.position; // enemy's position

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = randomStartAngle + i * degreesBetween;
                float rad = angle * Mathf.Deg2Rad;

                // Calculate direction
                Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
                Vector2 dummyTarget = (Vector2)center + direction * 10f; // 10 is just to make a point in that direction

                // Get and activate the bullet
                var bullet = pool.Get();
                bullet.Activate(WeaponType.EnemyBullet,
                    dummyTarget,               // Fake target far in direction
                    center,          // Start at enemy
                    _stats.ProjectileSpeed,    // Use your speed
                    _stats.ProjectileBuffer,   // Use your buffer
                    pool
                );
            }

            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate,()=>
            {
                _attack = false;
            });
        }

    }
}