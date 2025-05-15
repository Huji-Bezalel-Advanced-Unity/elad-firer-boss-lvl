using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public class PlayerAnimator: LBAnimator
    {
        private readonly GameObject _flame;
        private static readonly int Flame = Animator.StringToHash("Flame");

        public PlayerAnimator(Animator animator, LBStats stats, List<Transform> targetTransforms, GameObject flame) : base(animator, stats, targetTransforms)
        {
            _flame = flame;
        }

        public override void UpdateMovementAnimations(bool movementIsMoving)
        {
            if (!movementIsMoving)
            {
                Animator.SetBool(Flame, false);
                return;
            }
            Animator.SetBool(Flame, true);
            _flame.SetActive(true);
        }
    }
}