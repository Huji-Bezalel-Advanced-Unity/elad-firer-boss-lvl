using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossAttacker : LBAttacker
    {
        public BossAttacker(LBMonoPool<LBBaseProjectile> projectilePool, LBStats stats, Transform target, Transform entityTransform) : base(projectilePool, stats, target, entityTransform)
        {
        }

        public override void NormalAttack()
        {
            throw new System.NotImplementedException();
        }

        public override void StopAttack()
        {
            throw new System.NotImplementedException();
        }
        
    }
}