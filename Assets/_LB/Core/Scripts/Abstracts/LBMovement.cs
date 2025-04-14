using UnityEngine;

namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBMovement
    {
        protected Rigidbody2D Rigidbody;
        
        protected LBMovement(Rigidbody2D rb)
        {
            Rigidbody = rb;
        }
        
        public abstract void UpdateMovement();
    }
}