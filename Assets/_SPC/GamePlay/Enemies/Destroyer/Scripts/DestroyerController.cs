using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Enemies.BaseEnemy;
using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;
using _SPC.GamePlay.Enemies.Destroyer.Scripts.Controllers;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Destroyer.Scripts
{
    public class DestroyerController: SPCBaseEnemy
    {
        [SerializeField] private DestroyerStats stats;
        private DestroyerAttacker _attacker;
        private Transform _explosionsFather;
        private bool _initialized = false;


        public override void Init(Transform mainTarget, List<Transform> targets, GameObject explosionPrefab, Transform explosionsFather, BulletMonoPool pool)
        {
            base.Init(mainTarget, targets, explosionPrefab, explosionsFather, pool);
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<WeaponType, BulletMonoPool> { { WeaponType.DestroyerBullet, bulletPool } },
                TargetTransforms = transformTargets,
                Logger = enemyLogger
            };
            _attacker = new DestroyerAttacker(stats, deps);
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized) return;
            _attacker.NormalAttack();
        }
        
        
    }
}