namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBState
    {
        protected LBContext Context;
        protected LBAnimator Animator;
        protected LBMovement Movement;
        protected LBData Data;

        protected LBState(LBContext context, LBAnimator animator,LBMovement movement, LBData data)
        {
            Context = context;
            Animator = animator;
            Data = data;
            Movement = movement;
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}