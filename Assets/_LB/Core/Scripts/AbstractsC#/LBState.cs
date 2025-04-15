namespace _LB.Core.Scripts.AbstractsC_
{
    public abstract class LBState
    {
        protected LBAnimator Animator;
        protected LBMovement Movement;
        protected LBData Data;
        protected LBAttacker Attacker;

        protected LBState(LBAnimator animator,LBMovement movement, LBData data,LBAttacker attacker)
        {
            Animator = animator;
            Data = data;
            Movement = movement;
            Attacker = attacker;
        }

        public abstract void Enter();
        public abstract string Update();
        public abstract void Exit();
    }
}