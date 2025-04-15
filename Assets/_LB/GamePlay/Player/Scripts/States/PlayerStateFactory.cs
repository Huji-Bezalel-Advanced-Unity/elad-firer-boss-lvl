using _LB.Core.Scripts.Abstracts;

namespace _LB.GamePlay.Player.Scripts.States
{
    public class PlayerStatesFactory : LBStateFactory
    {
        private const string SpecialAttack = "SpecialAttack";
        private const string Hurt = "Hurt";
        private const string Die = "Die";


        public PlayerStatesFactory(LBAnimator animator, LBData data, LBMovement movement) : base(animator, data, movement)
        {
        }

        public override LBState CreateNormalState()
        {
            return new PlayerNormalState(Animator, Movement, Data);
        }

        protected override LBState CreateState(string stateId)
        {
            return stateId switch
            {
                Normal => CreateNormalState(),
                SpecialAttack => new PlayerSpecialAttackState(Animator, Movement, Data),
                Hurt => new PlayerHurtState(Animator, Movement, Data),
                Die => new PlayerDieState(Animator, Movement, Data),
                _ => throw new System.ArgumentException($"Unknown stateId: {stateId}")
            };
        }
    }
}