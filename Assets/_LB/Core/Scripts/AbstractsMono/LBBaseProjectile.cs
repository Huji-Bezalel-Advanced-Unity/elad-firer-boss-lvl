using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.Generics;
using _LB.Core.Scripts.Interfaces;
using _LB.GamePlay.Player.Scripts.Controllers;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsMono
{
    public abstract class LBBaseProjectile : LBBaseMono, ILBPoolable
    {
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected BoxCollider2D collider;


        public void Reset()
        {
            rb2D.linearVelocity = Vector2.zero;
        }
        
    }
}