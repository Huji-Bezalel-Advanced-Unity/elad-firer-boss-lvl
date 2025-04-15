using _LB.Core.Scripts.AbstractsC_;

namespace _LB.GamePlay.Player.Scripts.States
{
    public sealed class PlayerDieState: LBState
    {
        public PlayerDieState(LBAnimator animator, LBMovement movement, LBData data, LBAttacker attacker) : base(animator, movement, data, attacker)
        {
        }

        public override void Enter()
        {
            throw new System.NotImplementedException();
        }

        public override string Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}