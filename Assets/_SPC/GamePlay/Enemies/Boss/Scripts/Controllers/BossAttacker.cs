using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.GamePlay.Weapons.Bullet;
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
            if (!ProjectilePools.TryGetValue(WeaponType.BossBullet, out var pool))
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
                Vector2 dummyPosition = (Vector2)center + direction * 10f;

                GameObject tempTargetObject = new GameObject("DummyBulletTarget");
                tempTargetObject.transform.position = dummyPosition;

                var bullet = pool.Get();
                bullet.Activate(new BulletInitData(
                    WeaponType.BossBullet,
                    tempTargetObject.transform,
                    center,
                    _stats.ProjectileSpeed,
                    _stats.ProjectileBuffer,
                    pool
                ));

                // Optional: Destroy dummy after short delay to clean up
                Object.Destroy(tempTargetObject, 0.2f);
            }

            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate,()=>
            {
                _attack = false;
            });
        }
        
        

    }
}