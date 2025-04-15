using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBAnimator
    {
        protected Animator animator;

        protected ILBStats Stats;

        protected LBAnimator(Animator animator, ILBStats stats)
        {
            this.animator = animator;
            Stats = stats;
        }
    }
}