using _LB.Core.Scripts.Abstracts;

namespace _LB.GamePlay.Boss.Scripts.States
{
    public class BossContext: LBContext
    {
        public BossContext(LBAnimator animator, LBMovement movement, LBData data, LBStateFactory stateFactory) : base(animator, movement, data, stateFactory)
        {
        }
    }
}