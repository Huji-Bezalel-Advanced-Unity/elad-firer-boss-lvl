using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBMovement
    {
        public Vector2 Direction { get; protected set; }
        protected Rigidbody2D Rigidbody;
        protected ILBStats Stats;
        
        protected LBMovement(Rigidbody2D rb, ILBStats stats)
        {
            Rigidbody = rb;
            Stats = stats;
        }
        
        public abstract void UpdateMovement();
    }
}