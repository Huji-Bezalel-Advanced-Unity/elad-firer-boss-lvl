namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBContext
    {
        protected LBAnimator Animator;
        protected LBMovement Movement;
        protected LBData Data;
        protected LBState State;

        protected LBContext(LBAnimator animator,LBMovement movement, LBData data)
        {
            Animator = animator;
            Data = data;
            Movement = movement;
        }
        void ChangeState(LBState newState)
        {
            State = newState;
        }
        
    }
}