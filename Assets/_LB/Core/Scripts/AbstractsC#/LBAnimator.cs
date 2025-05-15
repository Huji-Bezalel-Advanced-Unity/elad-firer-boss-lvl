using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using _LB.Core.Scripts.Utils;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsC_
{
    public abstract class LBAnimator
    {
        protected readonly Animator Animator;

        protected readonly LBStats Stats;
        protected readonly List<Transform> TargetsTransforms;
        protected readonly Transform Transform;

        protected LBAnimator(Animator animator, LBStats stats,List<Transform> targetsTransforms)
        {
            Animator = animator;
            Stats = stats;
            TargetsTransforms = targetsTransforms;
            Transform = Animator.transform;
        }

        public virtual void RotateTowardsNearestTarget()
        {
            if (TargetsTransforms == null || TargetsTransforms.Count == 0) return;
            Transform closest = UsedAlgorithms.GetClosestTarget(TargetsTransforms, Animator.transform);
            if (closest == null) return;

            Vector3 dir = (closest.position - Animator.transform.position).normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            // Make the sprite's "up" face the target
            Animator.transform.up = -dir;
        }

        public abstract void UpdateMovementAnimations(bool movementIsMoving);

    }
}