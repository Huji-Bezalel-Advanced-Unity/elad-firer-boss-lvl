using _LB.Core.Scripts.Abstracts;

namespace _LB.GamePlay.Boss.Scripts.States
{
    public class BossStatesFactory : LBStateFactory
    {
        private const string SpecialAttack1 = "SpecialAttack1";
        private const string SpecialAttack2 = "SpecialAttack2";
        private const string SpecialAttack3 = "SpecialAttack3";

        public BossStatesFactory(LBAnimator animator, LBData data, LBMovement movement)
            : base(animator, data, movement)
        {
        }

        public override LBState CreateNormalState()
        {
            return new BossNormalState(Animator, Movement, Data);
        }

        protected override LBState CreateState(string stateId)
        {
            return stateId switch
            {
                Normal => CreateNormalState(),
                SpecialAttack1 => new BossSpecialAttack1State(Animator, Movement, Data),
                SpecialAttack2 => new BossSpecialAttack2State(Animator, Movement, Data),
                SpecialAttack3 => new BossSpecialAttack3State(Animator, Movement, Data),
                _ => throw new System.ArgumentException($"Unknown stateId: {stateId}")
            };
        }
    }
}