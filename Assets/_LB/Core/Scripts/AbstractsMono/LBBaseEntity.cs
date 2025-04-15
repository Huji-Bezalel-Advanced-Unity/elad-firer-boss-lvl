using _LB.Core.Scripts.Abstracts;
using System.Collections.Generic;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.BaseMono
{
    public class LBBaseEntity: LBBaseMono
    {
        [Header("Movement")]
        [SerializeField] protected Rigidbody2D rb2D;
        
        [Header("Data")]
        [SerializeField] protected Collider2D entityCollider;
        
        [Header("Animations")]
        [SerializeField] protected Animator animator;

        [Header("Stats")] 
        [SerializeField] protected ScriptableObject stats;
        protected ILBStats Stats
        {
            get
            {
                if (stats is ILBStats statsFormula)
                    return statsFormula;

                throw new System.InvalidCastException(
                    $"{nameof(stats)} does not implement {nameof(ILBStats)} in {this.GetType().Name}"
                );
            }
        }
        
        protected LBMovement Movement;
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