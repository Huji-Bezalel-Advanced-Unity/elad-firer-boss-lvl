using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsMono
{
    public abstract class LBBaseEntity: LBBaseMono
    {
        protected LBMovement Movement;
        protected LBAttacker Attacker;
        protected LBData Data;
        protected LBAnimator Animator;
        protected LBContext Context;
        protected LBStateFactory StateFactory;
        
        protected virtual void Update()
        {
            Context.UpdateState();
        }

        public virtual void GotHit(int i)
        {
            Data.GotHit(i);
        }

    }

   
}