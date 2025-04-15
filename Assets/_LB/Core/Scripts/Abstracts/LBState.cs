namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBState
    {
        protected LBAnimator Animator;
        protected LBMovement Movement;
        protected LBData Data;

        protected LBState(LBAnimator animator,LBMovement movement, LBData data)
        {
            Animator = animator;
            Data = data;
            Movement = movement;
        }

        public abstract void Enter();
        public abstract string Update();
        public abstract void Exit();
    }
}