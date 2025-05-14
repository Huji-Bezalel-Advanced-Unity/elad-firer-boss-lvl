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
        protected List<Transform> TargetTransform = new List<Transform>();
        protected Transform EntityTransform;
        protected readonly LBData _entityData;

        public LBAttacker(LBStats stats, Transform target,
            Transform entityTransform, LBData entityData)
        {
            EntityTransform = entityTransform;
            _entityData = entityData;
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