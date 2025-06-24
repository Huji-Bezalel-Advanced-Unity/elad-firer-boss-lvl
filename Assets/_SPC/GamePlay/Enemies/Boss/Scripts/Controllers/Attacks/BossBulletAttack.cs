using System;
using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Weapons.Bullet;

using UnityEngine;
using _SPC.Core.Scripts.Utils;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
{
    public struct BossBulletAttackDependencies
    {
        public Transform EntityTransform;
        public Transform DummyParentTransform;
        public BulletMonoPool BossBulletPool;
        public GameLogger Logger;
    }

    public class BossBulletAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossBulletAttackDependencies _deps;

        public BossBulletAttack(BossStats stats, BossBulletAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
        }

        public override bool Attack(Action onFinished = null)
        {
            if (_deps.BossBulletPool == null)
            {
                _deps.Logger?.Log("BossBullet pool not found!");
                return false;
            }

            int bulletCount = _stats.bulletCount;
            float degreesBetween = 360f / bulletCount;
            float randomStartAngle = Random.Range(0f, 360f);

            Vector2 center = _deps.EntityTransform.position; 

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = randomStartAngle + i * degreesBetween;
                float rad = angle * Mathf.Deg2Rad;
                
                Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
                Vector2 dummyPosition = center + direction * 10f;

                GameObject tempTargetObject = new GameObject("DummyBulletTarget");
                tempTargetObject.transform.SetParent(_deps.DummyParentTransform);
                tempTargetObject.transform.position = dummyPosition;

                var bullet = _deps.BossBulletPool.Get();
                bullet.Activate(new BulletInitData(
                    WeaponType.BossBullet,
                    tempTargetObject.transform,
                    center,
                    _stats.ProjectileSpeed,
                    _stats.ProjectileBuffer,
                    _deps.BossBulletPool
                ));

                Object.Destroy(tempTargetObject, 0.2f);
            }
            return true;
        }

       
       
       
    }
} 