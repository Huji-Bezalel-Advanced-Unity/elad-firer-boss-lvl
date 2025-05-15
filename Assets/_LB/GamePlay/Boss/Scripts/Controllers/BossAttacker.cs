using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossAttacker : LBAttacker
    {
        public BossAttacker(LBStats stats, Transform target, Transform entityTransform, List<Transform> targetTransform) : base(stats, target, entityTransform, targetTransform)
        {
        }

        public override void NormalAttack()
        {
            throw new System.NotImplementedException();
        }

        
    }
}