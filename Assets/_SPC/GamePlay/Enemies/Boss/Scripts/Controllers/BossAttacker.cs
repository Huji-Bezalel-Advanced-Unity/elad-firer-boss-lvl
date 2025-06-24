using System;
using System.Collections;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using System.Collections.Generic;
using _SPC.Core.Scripts.Managers;
using _SPC.GamePlay.Enemies.Destroyer.Scripts;
using Unity.VisualScripting;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
{
    public struct BossAttackerDependencies
    {
        public BoxCollider2D ArenaCollider;
        public BulletMonoPool DestroyerPool;
        public GameObject ExplosionPrefab;
        public Transform ExplosionsFather;
        public Collider2D BossCollider;
        public Transform DummyParentTransform;
        public SPCHealth Health;
    }

    public enum BossAttacks
    {
        BulletsShot,
        BigBulletShot,
        SpawnDestroyers,
        Rage
    }
    
    public class BossAttacker : SPCAttacker
    {
        private readonly BossStats _stats;
        private readonly BossAttackerDependencies _bossDeps;
        private readonly BossBulletAttack _bulletAttack;
        private readonly BossBigBulletAttack _bigBulletAttack;
        private readonly BossSpawnDestroyersAttack _spawnDestroyersAttack;
        private readonly BossRageAttack _rageAttack;
        private bool _isPaused = false;
        private Coroutine _currentCourtine;


        public BossAttacker(BossStats stats, AttackerDependencies deps, BossAttackerDependencies bossDeps) : base(deps)
        {
            _stats = stats;
            _bossDeps = bossDeps;

            _bossDeps.Health.AddHPAction(_bossDeps.Health.maxHealth * 2 / 3,
                new SPCHealthAction(() =>
                {
                    if(_currentCourtine != null)
                    {
                        AttackerMono.StopCoroutine(_currentCourtine);
                    }
                    _currentCourtine = AttackerMono.StartCoroutine(Phase2Attack());
                }));
            _bossDeps.Health.AddHPAction(_bossDeps.Health.maxHealth * 1 / 3,
                new SPCHealthAction(() =>
                {
                    if(_currentCourtine != null)
                    {
                        AttackerMono.StopCoroutine(_currentCourtine);
                    }
                    _currentCourtine = AttackerMono.StartCoroutine(Phase3Attack());
                }));

            _currentCourtine = AttackerMono.StartCoroutine(Phase1Attack());
            // Initialize bullet attack
            var bulletAttackDeps = new BossBulletAttackDependencies
            {
                EntityTransform = EntityTransform,
                DummyParentTransform = _bossDeps.DummyParentTransform,
                BossBulletPool = ProjectilePools[WeaponType.BossBullet],
                Logger = Logger
            };
            _bulletAttack = new BossBulletAttack(_stats, bulletAttackDeps);

            // Initialize big bullet attack
            var bigBulletAttackDeps = new BossBigBulletAttackDependencies
            {
                EntityTransform = EntityTransform,
                DummyParentTransform = _bossDeps.DummyParentTransform,
                MainTarget = MainTarget,
                BigBulletPool = ProjectilePools[WeaponType.BossBigBullet],
                Logger = Logger
            };
            _bigBulletAttack = new BossBigBulletAttack(_stats, bigBulletAttackDeps);

            // Initialize spawn destroyers attack
            var spawnDestroyersDeps = new BossSpawnDestroyersAttackDependencies
            {
                ArenaCollider = _bossDeps.ArenaCollider,
                DestroyerPool = _bossDeps.DestroyerPool,
                ExplosionPrefab = _bossDeps.ExplosionPrefab,
                ExplosionsFather = _bossDeps.ExplosionsFather,
                BossCollider = _bossDeps.BossCollider,
                MainTarget = MainTarget,
                TargetTransforms = new List<Transform>(TargetTransforms)
            };
            _spawnDestroyersAttack = new BossSpawnDestroyersAttack(_stats, spawnDestroyersDeps);

            // Initialize rage attack
            var rageAttackDeps = new BossRageAttackDependencies
            {
                EntityTransform = EntityTransform,
                MainTarget = MainTarget,
                Logger = Logger
            };
            _rageAttack = new BossRageAttack(_stats, rageAttackDeps);

            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        #region Phase 1
        
        private IEnumerator Phase1Attack()
        {
            while (true)
            {
                if(_isPaused) continue;
                yield return new WaitForSeconds(5f);
                Attack(BossAttacks.Rage);
            }
            yield break;
        }
        
        #endregion
        
        #region Phase 2
        private IEnumerator Phase2Attack()
        {
            while (true)
            {
                if(_isPaused) continue;
                yield return null;
            }
            yield break;
        }
        
        #endregion
        
        #region Phase 3
        private IEnumerator Phase3Attack()
        {
            while (true)
            {
                if(_isPaused) continue;
                yield return null;
            }
            yield break;
        }
        
        #endregion



        private void OnGamePaused()
        {
            _isPaused = true;
        }

        private void OnGameResumed()
        {
            _isPaused = false;
        }

        public override void Attack()
        {
            if (_isPaused) return;
            // Attack(BossAttacks.BigBulletShot);
        }

        public override void CleanUp()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
            
            _rageAttack?.Cleanup();
        }

        private bool Attack(BossAttacks attackType, Action onFinish = null)
        {
            if (_isPaused) return false;

            switch (attackType)
            {
                case BossAttacks.BulletsShot:
                    return _bulletAttack.Attack();
                    break;
                case BossAttacks.BigBulletShot:
                    return _bigBulletAttack.Attack();
                    break;
                case BossAttacks.SpawnDestroyers:
                    return _spawnDestroyersAttack.Attack();
                    break;
                case BossAttacks.Rage:
                    return _rageAttack.Attack();
                    break;
            }
            return false;
        }

        

        public bool IsRageAttacking => _rageAttack.IsAttacking;
    }
}