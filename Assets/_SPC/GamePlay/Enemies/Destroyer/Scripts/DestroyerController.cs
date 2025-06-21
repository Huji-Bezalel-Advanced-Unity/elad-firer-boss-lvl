using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
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
        private SPCHealth _health;
        private Transform _explosionsFather;
        private bool _initialized = false;
        private Coroutine _flashCoroutine;
        [Header("Sprite Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;


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

            var healthDeps = new HealthDependencies
            {
                healthUI = null,
                logger = enemyLogger,
                OnDeathAction = (() => Destroy(gameObject)),
                maxHP = stats.Health,
                currentHP = stats.Health,
                OnDamageActions = new List<Action<float, float>>{FlashCourtine}
            };
            _health = new SPCHealth(healthDeps);
        }

        private void Update()
        {
            if (!_initialized) return;
            _attacker.NormalAttack();
        }

        public override void GotHit(Vector3 projectileTransform, WeaponType weaponType)
        {
            base.GotHit(projectileTransform, weaponType);
            _health.ReduceLife(SPCAttacker.damage[weaponType]);
        }
        

        private void FlashCourtine(float health, float damage)
        {
            if (_flashCoroutine != null)
                return;
            _flashCoroutine = StartCoroutine(FlashRed());
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