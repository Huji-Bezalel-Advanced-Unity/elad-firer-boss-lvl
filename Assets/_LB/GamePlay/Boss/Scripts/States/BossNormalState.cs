using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;

namespace _LB.GamePlay.Boss.Scripts.States
{
    public class BossNormalState: LBState
    {
        public BossNormalState(LBAnimator animator, LBMovement movement, LBData data, LBAttacker attacker, LBStats stats) : base(animator, movement, data, attacker, stats)
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