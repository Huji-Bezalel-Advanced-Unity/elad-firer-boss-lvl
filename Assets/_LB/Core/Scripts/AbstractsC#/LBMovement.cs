using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsC_
{
    public abstract class LBMovement
    {
        public Vector2 Direction { get; protected set; }
        protected Rigidbody2D Rigidbody;
        protected LBStats Stats;
        
        protected LBMovement(Rigidbody2D rb, LBStats stats)
        {
            Rigidbody = rb;
            Stats = stats;
        }
        
        public abstract void UpdateMovement();
    }
}