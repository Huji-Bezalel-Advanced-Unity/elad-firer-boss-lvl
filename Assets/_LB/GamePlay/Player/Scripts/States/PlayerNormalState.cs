using _LB.Core.Scripts.AbstractsC_;

namespace _LB.GamePlay.Player.Scripts.States
{
    public sealed class PlayerNormalState: LBState
    {
        public PlayerNormalState(LBAnimator animator, LBMovement movement, LBData data, LBAttacker attacker) : base(animator, movement, data, attacker)
        {
        }

        public override void Enter()
        {
            Attacker.NormalAttack();
        }

        public override string Update()
        {
            Movement.UpdateMovement();
            return "Normal";
        }

        public override void Exit()
        {
            Attacker.StopAttack();
        }
    }
}