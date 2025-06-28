using System;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies for the BossBigBulletAttack, including transforms, pool, and logger.
    /// </summary>
    public struct BossBigBulletAttackDependencies
    {
        public Transform EntityTransform;
        public Transform DummyParentTransform;
        public Transform MainTarget;
        public BulletMonoPool BigBulletPool;
        public GameLogger Logger;
    }

    /// <summary>
    /// Handles the boss's big bullet attack that targets the player.
    /// </summary>
    public class BossBigBulletAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossBigBulletAttackDependencies _deps;
        private bool _attack = false;

        /// <summary>
        /// Initializes the BossBigBulletAttack with stats and dependencies.
        /// </summary>
        public BossBigBulletAttack(BossStats stats, BossBigBulletAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
        }

        /// <summary>
        /// Executes the big bullet attack towards the main target.
        /// </summary>
        public override bool Attack(Action onFinished = null)
        {
            if (_attack) return false;
            
            if (!ValidateBigBulletPool())
            {
                return false;
            }

            _attack = true;
            FireBigBullet();
            ScheduleAttackReset(onFinished);
            
            return true;
        }

        /// <summary>
        /// Validates that the big bullet pool is available.
        /// </summary>
        private bool ValidateBigBulletPool()
        {
            if (_deps.BigBulletPool == null)
            {
                _deps.Logger?.Log("BossBigBullet pool not found!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Fires the big bullet towards the main target.
        /// </summary>
        private void FireBigBullet()
        {
            Vector2 center = _deps.EntityTransform.position;
            Vector2 targetPosition = _deps.MainTarget.position;
            Vector2 direction = CalculateDirectionToTarget(center, targetPosition);
            Vector2 dummyPosition = center + (Vector2)(direction * 10f);

            GameObject tempTarget = CreateTemporaryTarget(dummyPosition);
            FireBullet(center, tempTarget);
        }

        /// <summary>
        /// Calculates the direction from center to target.
        /// </summary>
        private Vector2 CalculateDirectionToTarget(Vector2 center, Vector2 targetPosition)
        {
            return (targetPosition - center).normalized;
        }

        /// <summary>
        /// Creates a temporary target object for the big bullet.
        /// </summary>
        private GameObject CreateTemporaryTarget(Vector2 position)
        {
            GameObject tempTargetObject = new GameObject("DummyBigBulletTarget");
            tempTargetObject.transform.SetParent(_deps.DummyParentTransform);
            tempTargetObject.transform.position = position;
            
            // Clean up after a short delay
            Object.Destroy(tempTargetObject, 0.2f);
            
            return tempTargetObject;
        }

        /// <summary>
        /// Fires the big bullet from center towards the target.
        /// </summary>
        private void FireBullet(Vector2 center, GameObject target)
        {
            var bullet = _deps.BigBulletPool.Get();
            bullet.Activate(new BulletInitData(
                WeaponType.BossBigBullet,
                target.transform,
                center,
                _stats.ProjectileSpeed,
                _stats.ProjectileBuffer,
                _deps.BigBulletPool
            ));
        }

        /// <summary>
        /// Schedules the attack reset and callback after the spawn rate delay.
        /// </summary>
        private void ScheduleAttackReset(Action onFinished)
        {
            DOVirtual.DelayedCall(_stats.ProjectileSpawnRate, () =>
            {
                _attack = false;
                onFinished?.Invoke();
            });
        }
    }
} 