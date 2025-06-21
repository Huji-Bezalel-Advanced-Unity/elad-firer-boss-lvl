using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.InputSystem;
using _SPC.Core.Scripts.Interfaces;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerAttacker: SPCAttacker
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

            for (int i = 0; i < Stats.NumberOfShots; i++)
            {
                OneBulletShot(i);
            }
            
            _attack = false;
        }

        private void OneBulletShot(int shotIndex)
        {
            var target = UsedAlgorithms.GetClosestTarget(TargetTransforms, EntityTransform);
            if (target != null)
            {
                var proj = ProjectilePools[WeaponType.PlayerBullet].Get();

                Vector2 fireDirection = (target.position - EntityTransform.position).normalized;
                Vector2 perpendicular = new Vector2(-fireDirection.y, fireDirection.x);
                float totalWidth = (Stats.NumberOfShots - 1) * Stats.ShotSpacing;
                float offset = (Stats.NumberOfShots > 1) ? (shotIndex * Stats.ShotSpacing) - (totalWidth / 2f) : 0;
                Vector2 spawnPosition = (Vector2)EntityTransform.position + perpendicular * offset;

                proj.Activate(new BulletInitData(
                    WeaponType.PlayerBullet,
                    target,
                    spawnPosition,
                    Stats.ProjectileSpeed,
                    Stats.ProjectileBuffer,
                    ProjectilePools[WeaponType.PlayerBullet]
                ));
            }
        }
    }
}