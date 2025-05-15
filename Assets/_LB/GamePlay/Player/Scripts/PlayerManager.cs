using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.GamePlay.Boss.Scripts.States;
using _LB.GamePlay.Player.Scripts.Controllers;
using _LB.GamePlay.Player.Scripts.States;
using _LB.GamePlay.Player.Scripts.Weapon;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts
{
    public sealed class PlayerManager: LBBaseEntity
    {
        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb2D;
        
        [Header("Data")]
        [SerializeField] private Collider2D entityCollider;
        
        [Header("Animations")]
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject flame;

        [Header("Stats")] 
        [SerializeField] private LBStats stats;
        
        [Header("Attacker")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private BulletMonoPool projectilePool;
        [SerializeField] private List<Transform> transformTargets = new List<Transform>();
         void Start()
        {
            Movement = new PlayerMovement(rb2D,stats);
            Data = new PlayerData(entityCollider,stats);
            Attacker = new PlayerAttacker( stats,targetTransform,transform, projectilePool,transformTargets);
            Animator = new PlayerAnimator(animator,stats,transformTargets,flame);
            StateFactory = new PlayerStatesFactory(Animator, Data,Movement,Attacker,stats);
            Context = new PlayerContext(Animator, Movement, Data,StateFactory);
        }
        
        protected override void Update()
        {
            base.Update();
            
        }

        
        
        
    }
}