using System;
using _SPC.Core.Scripts.Abstracts;

using _SPC.Core.Scripts.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

using Object = UnityEngine.Object;
using DG.Tweening;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
{
    public struct BossBigBulletAttackDependencies
    {
        public Transform EntityTransform;
        public Transform DummyParentTransform;
        public Transform MainTarget;
        public BulletMonoPool BigBulletPool;
        public GameLogger Logger;
    }

    public class BossBigBulletAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossBigBulletAttackDependencies _deps;
        private bool _attack = false;

        public BossBigBulletAttack(BossStats stats, BossBigBulletAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
        }

        public override bool Attack(Action onFinished = null)
        {
            if (_attack) return false;
            _attack = true;

            if (_deps.BigBulletPool == null)
            {
                _deps.Logger?.Log("BossBigBullet pool not found!");
                return false;
            }

            Vector2 center = _deps.EntityTransform.position;
            Vector2 targetPosition = _deps.MainTarget.position;
            Vector2 direction = (targetPosition - center).normalized;

            // Create a dummy target object in the direction of the main target
            GameObject tempTargetObject = new GameObject("DummyBigBulletTarget");
            tempTargetObject.transform.SetParent(_deps.DummyParentTransform);
            tempTargetObject.transform.position = center + (Vector2)(direction * 10f);

            var bullet = _deps.BigBulletPool.Get();
            bullet.Activate(new BulletInitData(
                WeaponType.BossBigBullet,
                tempTargetObject.transform,
                center,
                _stats.ProjectileSpeed,
                _stats.ProjectileBuffer,
                _deps.BigBulletPool
            ));

            Object.Destroy(tempTargetObject, 0.2f);

            // Reset attack flag after delay and call callback
            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate, () =>
            {
                _attack = false;
                onFinished?.Invoke();
            });

            return true;
        }
    }
} 