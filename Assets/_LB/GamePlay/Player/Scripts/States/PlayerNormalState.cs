using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace _LB.GamePlay.Player.Scripts.States
{
    public sealed class PlayerNormalState: LBState
    {
        public PlayerNormalState(LBAnimator animator, LBMovement movement, LBData data, LBAttacker attacker, LBStats stats) : base(animator, movement, data, attacker, stats)
        {
        }

        public override void Enter()
        {
        }

        public override string Update()
        {
            Attacker.NormalAttack();
            Movement.UpdateMovement();
            Animator.RotateTowardsNearestTarget();
            Animator.UpdateMovementAnimations(Movement.IsMoving);
            return "Normal";
        }

        public override void Exit()
        {
        }
    }
}