using _LB.Core.Scripts.Abstracts;

namespace _LB.GamePlay.Player.Scripts.States
{
    public class PlayerContext: LBContext
    {
        public PlayerContext(LBAnimator animator, LBMovement movement, LBData data) : base(animator, movement, data)
        {
        }
    }
}