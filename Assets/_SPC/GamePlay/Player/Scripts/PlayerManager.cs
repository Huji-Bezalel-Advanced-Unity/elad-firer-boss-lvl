using System.Collections.Generic;
using _LB.Core.Scripts.Interfaces;
using _LB.Core.Scripts.Utils;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Player.Scripts.Controllers;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Player.Scripts
{
    public sealed class PlayerManager: LBBaseMono, IHitable
    {
        private static readonly int Flame = Animator.StringToHash("Flame");

        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb2D;
        
        [Header("Data")]
        [SerializeField] private Collider2D entityCollider;
        
        [Header("Animations")]
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject flame;

        [Header("Stats")] 
        [SerializeField] private PlayerStats stats;
        
        [Header("Attacker")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private BulletMonoPool projectilePool;
        [SerializeField] private List<Transform> transformTargets = new List<Transform>();
        private PlayerMovement _movement;
        private PlayerAttacker _attacker;

        void Start()
        {
            _movement = new PlayerMovement(rb2D,stats);
            _attacker = new PlayerAttacker(stats,targetTransform,transform, projectilePool,transformTargets);
        }
        
        
        public void Update()
        {
            _attacker.NormalAttack();
            _movement.UpdateMovement();
            RotateTowardsNearestTarget();
            HandleFlame();
        }

        private void HandleFlame()
        {
            if (!_movement.IsMoving)
            {
                animator.SetBool(Flame, false);
            }
            else
            {
                animator.SetBool(Flame, true);
                flame.SetActive(true);
            }
        }


        private void RotateTowardsNearestTarget()
        {
            if (transformTargets == null || transformTargets.Count == 0) return;
            Transform closest = UsedAlgorithms.GetClosestTarget(transformTargets, transform);
            if (closest == null) return;

            Vector3 dir = (closest.position - transform.position).normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            // Make the sprite's "up" face the target
            transform.up = -dir;
        }
        
        
    }
}