using _LB.Core.Scripts.AbstractsC_;

namespace _LB.GamePlay.Player.Scripts.States
{
    public class PlayerStatesFactory : LBStateFactory
    {
        private const string SpecialAttack = "SpecialAttack";
        private const string Hurt = "Hurt";
        private const string Die = "Die";


        public PlayerStatesFactory(LBAnimator animator, LBData data, LBMovement movement, LBAttacker attacker) : base(animator, data, movement, attacker)
        {
        }

        public override LBState CreateNormalState()
        {
            return new PlayerNormalState(Animator, Movement, Data,Attacker);
        }

        protected override LBState CreateState(string stateId)
        {
            return stateId switch
            {
                Normal => CreateNormalState(),
                SpecialAttack => new PlayerSpecialAttackState(Animator, Movement, Data,Attacker),
                Hurt => new PlayerHurtState(Animator, Movement, Data,Attacker),
                Die => new PlayerDieState(Animator, Movement, Data,Attacker),
                _ => throw new System.ArgumentException($"Unknown stateId: {stateId}")
            };
        }
    }
}