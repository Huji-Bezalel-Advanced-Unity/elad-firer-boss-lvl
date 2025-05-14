using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;

namespace _LB.GamePlay.Player.Scripts.States
{
    public class PlayerStatesFactory : LBStateFactory
    {
        private const string SpecialAttack = "SpecialAttack";
        private const string Hurt = "Hurt";
        private const string Die = "Die";


        public PlayerStatesFactory(LBAnimator animator, LBData data, LBMovement movement, LBAttacker attacker, LBStats stats) : base(animator, data, movement, attacker, stats)
        {
        }

        public override LBState CreateNormalState()
        {
            return new PlayerNormalState(Animator, Movement, Data,Attacker,Stats);
        }

        protected override LBState CreateState(string stateId)
        {
            return stateId switch
            {
                Normal => CreateNormalState(),
                SpecialAttack => new PlayerSpecialAttackState(Animator, Movement, Data,Attacker,Stats),
                Hurt => new PlayerHurtState(Animator, Movement, Data,Attacker,Stats),
                Die => new PlayerDieState(Animator, Movement, Data,Attacker,Stats),
                _ => throw new System.ArgumentException($"Unknown stateId: {stateId}")
            };
        }
    }
}