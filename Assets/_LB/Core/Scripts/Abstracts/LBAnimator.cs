using UnityEngine;

namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBAnimator
    {
        protected Animator animator;

        protected LBAnimator(Animator animator)
        {
            this.animator = animator;
        }
    }
}