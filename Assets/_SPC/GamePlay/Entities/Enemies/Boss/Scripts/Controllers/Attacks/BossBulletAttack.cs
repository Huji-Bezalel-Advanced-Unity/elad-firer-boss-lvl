using System;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies for the BossBulletAttack, including transforms, pool, and logger.
    /// </summary>
    public struct BossBulletAttackDependencies
    {
        public Transform EntityTransform;
        public Transform DummyParentTransform;
        public BulletMonoPool BossBulletPool;
        public GameLogger Logger;
    }

    /// <summary>
    /// Handles the boss's circular bullet attack pattern.
    /// </summary>
    public class BossBulletAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossBulletAttackDependencies _deps;
        private bool _isAttacking = false;
        private Action _onAttackFinished;

        /// <summary>
        /// Initializes the BossBulletAttack with stats and dependencies.
        /// </summary>
        public BossBulletAttack(BossStats stats, BossBulletAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
        }

        /// <summary>
        /// Executes the bullet attack in a circular pattern around the boss.
        /// </summary>
        public override bool Attack(Action onFinished = null)
        {
            if (!ValidateBulletPool())
            {
                return false;
            }
            
            PerformAttack();
            return true;
        }

        /// <summary>
        /// Validates that the bullet pool is available.
        /// </summary>
        private bool ValidateBulletPool()
        {
            if (_deps.BossBulletPool == null)
            {
                _deps.Logger?.Log("BossBullet pool not found!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Performs the circular bullet attack pattern.
        /// </summary>
        private void PerformAttack()
        {
            Vector2 center = _deps.EntityTransform.position;
            float randomStartAngle = Random.Range(0f, 360f);
            float degreesBetween = CalculateDegreesBetweenBullets();

            for (int i = 0; i < _stats.bulletCount; i++)
            {
                float angle = randomStartAngle + i * degreesBetween;
                Vector2 direction = CalculateBulletDirection(angle);
                Vector2 dummyPosition = center + direction * 10f;

                GameObject tempTarget = CreateTemporaryTarget(dummyPosition);
                FireBullet(center, tempTarget);
            }
        }

        /// <summary>
        /// Calculates the angle between each bullet in the circular pattern.
        /// </summary>
        private float CalculateDegreesBetweenBullets()
        {
            return 360f / _stats.bulletCount;
        }

        /// <summary>
        /// Calculates the direction vector for a bullet at the given angle.
        /// </summary>
        private Vector2 CalculateBulletDirection(float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
        }

        /// <summary>
        /// Creates a temporary target object for the bullet.
        /// </summary>
        private GameObject CreateTemporaryTarget(Vector2 position)
        {
            GameObject tempTargetObject = new GameObject("DummyBulletTarget");
            tempTargetObject.transform.SetParent(_deps.DummyParentTransform);
            tempTargetObject.transform.position = position;
            
            // Clean up after a short delay
            Object.Destroy(tempTargetObject, 0.2f);
            
            return tempTargetObject;
        }

        /// <summary>
        /// Fires a bullet from the center towards the target.
        /// </summary>
        private void FireBullet(Vector2 center, GameObject target)
        {
            var bullet = _deps.BossBulletPool.Get();
            bullet.Activate(new BulletInitData(
                WeaponType.BossBullet,
                target.transform,
                center,
                _stats.ProjectileSpeed,
                _stats.ProjectileBuffer,
                _deps.BossBulletPool
            ));
        }
    }
} 