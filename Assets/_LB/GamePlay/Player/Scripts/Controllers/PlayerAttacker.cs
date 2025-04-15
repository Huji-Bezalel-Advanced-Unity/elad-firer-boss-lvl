using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public class PlayerAttacker: LBAttacker
    {
        public PlayerAttacker(LBMonoPool<LBBaseProjectile> projectilePool, LBStats stats, Transform target, Transform entityTransform) : base(projectilePool, stats, target,entityTransform)
        {
        }

        public override void NormalAttack()
        {
            Transform closestTarget = GetClosestTarget();
            if (closestTarget == null) return;

            LBBaseProjectile projectile = ProjectilePool.Get();
            projectile.Activate(
                closestTarget.position,
                EntityTransform.position,
                Stats.ProjectileSpeed,
                Stats.ProjectileBuffer,
                ProjectilePool
            );
        }

        public override void StopAttack()
        {
            throw new System.NotImplementedException();
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