using System.Collections;
using System.Collections.Generic;
using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Utils;
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


        public PlayerAttacker(LBStats stats, Transform target, Transform entityTransform, BulletMonoPool projectilePool, List<Transform> targetTransforms) : base(stats, target, entityTransform,targetTransforms)
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
            var target = UsedAlgorithms.GetClosestTarget(TargetTransform, EntityTransform);
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

        
    }
}