using System.Collections.Generic;
using _LB.Core.Scripts.Utils;
using _SPC.Core.Scripts.InputSystem;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerAttacker
    {
        private readonly BulletMonoPool _projectilePool;
        private readonly InputSystem_Actions _inputSystem;
        private bool _attack;
        private PlayerStats Stats;
        private List<Transform> _targetTransforms;
        private Transform _playerTransform;


        public PlayerAttacker(PlayerStats stats, Transform target, Transform playerTransform, BulletMonoPool projectilePool, List<Transform> targetTransforms) 
        {
            _projectilePool = projectilePool;
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.Attack.performed += OnAttackPerformed;
            _playerTransform = playerTransform;
            _targetTransforms = targetTransforms;
            _targetTransforms.Add(target);
            Stats = stats;
        }

        private void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            _attack = true;
        }

        public void NormalAttack()
        {
            if (!_attack) return;
            var target = UsedAlgorithms.GetClosestTarget(_targetTransforms, _playerTransform);
                        if (target != null)
                        {
                            var proj = _projectilePool.Get();
                            proj.Activate(
                                target.position,
                                _playerTransform.position,
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