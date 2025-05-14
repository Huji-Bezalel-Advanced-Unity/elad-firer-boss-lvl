using System.Collections;
using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.GamePlay.Player.Scripts.Weapon;
using Core.Input_System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerAttacker: LBAttacker
    {
        private readonly BulletMonoPool _projectilePool;
        private readonly InputSystem_Actions _inputSystem;
        private bool _attack;


        public PlayerAttacker(BulletMonoPool projectilePool, LBStats stats, Transform target, Transform entityTransform, LBData entityData) : base(stats, target,entityTransform,entityData)
        {
            _projectilePool = projectilePool;
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.Attack.performed += OnAttackPerformed;
        }

        private void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            _attack = true;
        }

        public override void NormalAttack()
        {
            if (!_attack) return;
            var target = GetClosestTarget();
                        if (target != null)
                        {
                            var proj = _projectilePool.Get();
                            proj.Activate(
                                target.position,
                                EntityTransform.position,
                                Stats.ProjectileSpeed,
                                Stats.ProjectileBuffer,
                                this
                            );
                        }
            _attack = false;
        }

        
        public void ReturnToPool(Bullet bullet)
        {
            _projectilePool.Return(bullet);
        }

        private Transform GetClosestTarget()
        {
            Transform closest = null;
            float shortestDistance = float.MaxValue;

            foreach (Transform target in TargetTransform)
            {
                if (target == null) continue;

                float distance = Vector2.Distance(EntityTransform.position, target.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closest = target;
                }
            }

            return closest;
        }
    }
}