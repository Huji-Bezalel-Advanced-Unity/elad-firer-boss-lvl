using System;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Boss.Scripts
{
    public class BossController: LBBaseEnemy.LBBaseEnemy
    {
        [Header("Stats")] 
        [SerializeField] private BossStats stats;

        private BossSrcAttacker _srcAttacker;


        private void Start()
        {
            var deps = new AttackerDependencies
            {
                MainTarget = targetTransform,
                EntityTransform = transform,
                ProjectilePools = new Dictionary<BulletType, BulletMonoPool> { { BulletType.EnemyBullet, bulletPool } },
                TargetTransforms = transformTargets,
                Logger = enemyLogger
            };
            _srcAttacker = new BossSrcAttacker(stats, deps);
        }

        private void Update()
        {
            _srcAttacker.NormalAttack();
        }
    }
}