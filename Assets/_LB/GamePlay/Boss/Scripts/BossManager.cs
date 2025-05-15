using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.GamePlay.Boss.Scripts.Controllers;
using _LB.GamePlay.Boss.Scripts.States;
using UnityEngine;


namespace _LB.GamePlay.Boss.Scripts
{
    public class BossManager: LBBaseEntity
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
        [SerializeField] protected Transform targetTransform;
        [SerializeField] private List<Transform> transformTargets = new List<Transform>();
        void Start()
        {
            // Movement = new BossMovement(rb2D,Stats);
            // Data = new BossData(entityCollider,Stats);
            // Attacker = new BossAttacker(Stats,targetTransform,transform,transformTargets);
            // Animator = new BossAnimator(animator,Stats,transformTargets);
            // StateFactory = new BossStatesFactory(Animator, Data,Movement,Attacker,Stats);
            // Context = new BossContext(Animator, Movement, Data,StateFactory);
        }

        protected override void Update()
        {
        }

        public override void GotHit(int i)
        {
        }
    }
}