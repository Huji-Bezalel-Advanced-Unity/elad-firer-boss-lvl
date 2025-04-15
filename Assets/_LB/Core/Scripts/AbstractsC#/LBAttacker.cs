using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsC_
{
    public abstract class LBAttacker
    {
        protected LBMonoPool<LBBaseProjectile> ProjectilePool;
        protected LBStats Stats;
        protected List<Transform> TargetTransform = new List<Transform>();
        protected Transform EntityTransform;
        
        public LBAttacker(LBMonoPool<LBBaseProjectile> projectilePool, LBStats stats, Transform target,
            Transform entityTransform)
        {
            EntityTransform = entityTransform;
            TargetTransform.Add(target);
            ProjectilePool = projectilePool;
            Stats = stats;
        }

        protected void AddTarget(Transform target)
        {
            TargetTransform.Add(target);
        }

        public abstract void NormalAttack();

        public abstract void StopAttack();
    }
}