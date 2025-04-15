using _LB.Core.Scripts.Abstracts;

namespace _LB.GamePlay.Player.Scripts.States
{
    public sealed class PlayerNormalState: LBState
    {
        public PlayerNormalState(LBAnimator animator, LBMovement movement, LBData data) : base(animator, movement, data)
        {
        }

        public override void Enter()
        {
            
        }

        public override string Update()
        {
            Movement.UpdateMovement();
            return "Normal";
        }

        public override void Exit()
        {
            
        }
    }
}