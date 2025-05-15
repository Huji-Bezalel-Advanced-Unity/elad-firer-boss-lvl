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
        protected LBStats Stats;
        protected List<Transform> TargetTransform;
        protected Transform EntityTransform;

        protected LBAttacker(LBStats stats, Transform target,
            Transform entityTransform, List<Transform> targetTransform)
        {
            EntityTransform = entityTransform;
            TargetTransform = targetTransform;
            TargetTransform.Add(target);
            Stats = stats;
        }

        protected void AddTarget(Transform target)
        {
            TargetTransform.Add(target);
        }

        public abstract void NormalAttack();
        



    }
}