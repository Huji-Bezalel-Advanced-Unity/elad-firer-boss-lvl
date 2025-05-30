using System.Collections;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Player.Scripts.Controllers;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Player.Scripts
{
    public sealed class PlayerController: SPCBaseMono, IHitable
    {
        private static readonly int Flame = Animator.StringToHash("Flame");

        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb2D;
        
        [Header("Data")]
        [SerializeField] private Collider2D entityCollider;
        
        [Header("Animations")]
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject flame;
        [SerializeField] private SpriteRenderer spriteRenderer;


        [Header("Stats")] 
        [SerializeField] private PlayerStats stats;
        [SerializeField] private GameLogger  playerLogger;
        
        [Header("Attacker")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private BulletMonoPool playerPool;
        [SerializeField] private List<Transform> transformTargets = new List<Transform>();
        private PlayerMovement _movement;
        private PlayerAttacker _attacker;

        void Start()
        {
            _movement = new PlayerMovement(rb2D,stats,playerLogger);
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<BulletType, BulletMonoPool> { { BulletType.PlayerBullet, playerPool } },
                TargetTransforms = transformTargets,
                Logger = playerLogger
            };
            _attacker = new PlayerAttacker(stats, deps);
            _health = new PlayerHealth(playerLogger);
        }
        
        
        public void Update()
        {
            _attacker.NormalAttack();
            _movement.UpdateMovement();
            RotateTowardsNearestTarget();
            HandleFlame();
        }
        private Coroutine _flashCoroutine;
        private PlayerHealth _health;

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


        public void GotHit(Vector3 projectileTransform)
        {
            if (_flashCoroutine != null)
                return;

            _flashCoroutine = StartCoroutine(FlashRed());
        }

        public Type GetTypeOfEntity()
        {
            return Type.Player;
        }


        private IEnumerator FlashRed()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            _flashCoroutine = null;
        }
    }
}