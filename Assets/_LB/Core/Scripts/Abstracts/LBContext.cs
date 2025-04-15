using UnityEngine;

namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBContext
    {
        private const string Normal = "Normal";
        protected LBAnimator Animator;
        protected LBMovement Movement;
        protected LBData Data;
        protected LBStateFactory StateFactory;
        protected LBState State;
        

        protected LBContext(LBAnimator animator, LBMovement movement, LBData data, LBStateFactory stateFactory)
        {
            Animator = animator;
            Data = data;
            Movement = movement;
            StateFactory = stateFactory;
            State = StateFactory.GetState(Normal);
        }

        public void UpdateState()
        {
           string state = State.Update();
           if (StateFactory.GetState(state) != State)
           {
               State.Exit();
               State = StateFactory.GetState(state);
               State.Enter();
           }
        }
        
    }
}