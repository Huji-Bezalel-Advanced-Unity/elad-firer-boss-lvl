using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsC_
{
    public abstract class LBAnimator
    {
        protected Animator animator;

        protected LBStats Stats;

        protected LBAnimator(Animator animator, LBStats stats)
        {
            this.animator = animator;
            Stats = stats;
        }
    }
}