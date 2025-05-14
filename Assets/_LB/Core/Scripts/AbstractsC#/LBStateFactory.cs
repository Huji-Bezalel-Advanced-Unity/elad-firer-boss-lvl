using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsScriptable;

namespace _LB.Core.Scripts.AbstractsC_
{
    public abstract class LBStateFactory
    {
        protected readonly LBAnimator Animator;
        protected readonly LBData Data;
        protected readonly LBMovement Movement;
        protected readonly LBAttacker Attacker;
        protected readonly LBStats Stats;

        private readonly Dictionary<string, LBState> _stateCache = new();
        protected const string Normal = "Normal";

        protected LBStateFactory(LBAnimator animator, LBData data, LBMovement movement,LBAttacker attacker, LBStats stats)
        {
            Animator = animator;
            Data = data;
            Movement = movement;
            Attacker = attacker;
            Stats = stats;
        }
        public abstract LBState CreateNormalState();

        // Derived class must handle creation based on string key
        protected abstract LBState CreateState(string stateId);

        public LBState GetState(string stateId)
        {
            if (!_stateCache.ContainsKey(stateId))
            {
                _stateCache[stateId] = CreateState(stateId);
            }

            return _stateCache[stateId];
        }
    }
}