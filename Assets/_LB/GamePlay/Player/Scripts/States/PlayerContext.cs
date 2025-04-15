using _LB.Core.Scripts.AbstractsC_;

namespace _LB.GamePlay.Player.Scripts.States
{
    public sealed class PlayerContext: LBContext
    {
        public PlayerContext(LBAnimator animator, LBMovement movement, LBData data, LBStateFactory stateFactory) : base(animator, movement, data, stateFactory)
        {
        }
    }
}