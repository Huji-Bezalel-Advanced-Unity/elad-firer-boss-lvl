using System.Collections;
using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.GamePlay.Player.Scripts.Weapon;
using DG.Tweening;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerAttacker: LBAttacker
    {
        private readonly BulletMonoPool _projectilePool;


        public PlayerAttacker(BulletMonoPool projectilePool, LBStats stats, Transform target, Transform entityTransform, LBData entityData) : base(stats, target,entityTransform,entityData)
        {
            _projectilePool = projectilePool;
        }
        
        public override void NormalAttack(float projectileSpawnRate)
        {
            var target = GetClosestTarget();
                        if (target != null)
                        {
                            var proj = _projectilePool.Get();
                            proj.Activate(
                                target.position,
                                EntityTransform.position,
                                Stats.ProjectileSpeed,
                                Stats.ProjectileBuffer,
                                this
                            );
                        }
        }

        
        public void ReturnToPool(Bullet bullet)
        {
            _projectilePool.Return(bullet);
        }

        private Transform GetClosestTarget()
        {
            Transform closest = null;
            float shortestDistance = float.MaxValue;

            foreach (Transform target in TargetTransform)
            {
                if (target == null) continue;

                float distance = Vector2.Distance(EntityTransform.position, target.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closest = target;
                }
            }

            return closest;
        }
    }
}