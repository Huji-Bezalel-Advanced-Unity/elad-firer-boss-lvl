using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsMono
{
    public abstract class LBBaseEntity: LBBaseMono
    {
        [Header("Movement")]
        [SerializeField] protected Rigidbody2D rb2D;
        
        [Header("Data")]
        [SerializeField] protected Collider2D entityCollider;
        
        [Header("Animations")]
        [SerializeField] protected Animator animator;

        [Header("Stats")] 
        [SerializeField] protected LBStats Stats;
        
        [Header("Attacker")]
        [SerializeField] protected LBMonoPool<LBBaseProjectile> projectilePool;
        [SerializeField] protected Transform targetTransform;
        
        
        protected LBMovement Movement;
        protected LBAttacker Attacker;
        protected LBData Data;
        protected LBAnimator Animator;
        protected LBContext Context;
        protected LBStateFactory StateFactory;
        
        protected virtual void Update()
        {
            Context.UpdateState();
        }
        
    }

   
}