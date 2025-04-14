using _LB.Core.Scripts.Abstracts;
using UnityEngine;

namespace _LB.Core.Scripts.BaseMono
{
    public class LBBaseEntity: LBBaseMono
    {
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected Collider2D entityCollider;
        [SerializeField] protected UnityEngine.Animator animator;
        protected LBMovement Movement;
        protected LBData Data;
        protected LBAnimator Animator;
        protected LBContext Context;

        
    }
}