using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.BaseMono
{
    public class LBBaseProjectile : LBBaseMono, ILBPoolable
    {
        [SerializeField] Rigidbody2D rb2D;
        [SerializeField] Transform target;
        
        public void Reset()
        {
            rb2D.linearVelocity = Vector2.zero;
        }
    }
}