using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.InputSystem;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerAttacker: Attacker
    {
        private readonly InputSystem_Actions _inputSystem;
        private bool _attack;
        private PlayerStats Stats;


        public PlayerAttacker(PlayerStats stats, AttackerDependencies dependencies) : base(dependencies) 
        {
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.Attack.performed += OnAttackPerformed;
            Stats = stats;
        }

        private void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            _attack = true;
        }

        public void NormalAttack()
        {
            if (!_attack) return;
            OneBulletShot();
            _attack = false;
        }

        private void OneBulletShot()
        {
            var target = UsedAlgorithms.GetClosestTarget(TargetTransforms, EntityTransform);
            if (target != null)
            {
                var proj = ProjectilePools[BulletType.PlayerBullet].Get();
                proj.Activate(
                    target.position,
                    EntityTransform.position,
                    Stats.ProjectileSpeed,
                    Stats.ProjectileBuffer,
                    ProjectilePools[BulletType.PlayerBullet]
                );
            }
        }
    }
}