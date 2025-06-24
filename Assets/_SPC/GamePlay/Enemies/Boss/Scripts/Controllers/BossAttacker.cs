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
        private Coroutine _currentCoroutine;
        private bool _isSpecialAttackActive = false;
        private float _lastBulletAttackTime = 0f;

        // Phase-specific attack lists
        private readonly List<BossAttacks> _phase1Attacks = new List<BossAttacks> { BossAttacks.BigBulletShot, BossAttacks.SpawnDestroyers };
        private readonly List<BossAttacks> _phase2Attacks = new List<BossAttacks> { BossAttacks.BigBulletShot, BossAttacks.SpawnDestroyers };
        private readonly List<BossAttacks> _phase3Attacks = new List<BossAttacks> { BossAttacks.BigBulletShot, BossAttacks.SpawnDestroyers, BossAttacks.Rage };
        private float _lastTime;

        public BossAttacker(BossStats stats, AttackerDependencies deps, BossAttackerDependencies bossDeps) : base(deps)
        {
            _stats = stats;
            _bossDeps = bossDeps;

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

            // Set up health-based phase transitions
            _bossDeps.Health.AddHPAction(_bossDeps.Health.maxHealth * 2 / 3,
                new SPCHealthAction(() =>
                {
                    if(_currentCoroutine != null)
                    {
                        AttackerMono.StopCoroutine(_currentCoroutine);
                    }
                    _currentCoroutine = AttackerMono.StartCoroutine(PhaseAttack(_phase2Attacks, _stats.phase2SpecialAttackInterval));
                }));
            _bossDeps.Health.AddHPAction(_bossDeps.Health.maxHealth * 1 / 3,
                new SPCHealthAction(() =>
                {
                    if(_currentCoroutine != null)
                    {
                        AttackerMono.StopCoroutine(_currentCoroutine);
                    }
                    _currentCoroutine = AttackerMono.StartCoroutine(PhaseAttack(_phase3Attacks, _stats.phase3SpecialAttackInterval));
                }));

            // Start phase 1
            _currentCoroutine = AttackerMono.StartCoroutine(PhaseAttack(_phase1Attacks, _stats.phase1SpecialAttackInterval));

            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        private IEnumerator PhaseAttack(List<BossAttacks> availableAttacks, float attackInterval)
        {
            
            
            while (true)
            {
                while(_isPaused){
                    yield return null;
                }
                
                yield return AttackerMono.StartCoroutine(WaitForSecondsPauseAware(attackInterval));
                
                while(_isPaused){
                    yield return null;
                }
                
                // Choose and execute random special attack
                ExecuteRandomSpecialAttack(availableAttacks);
            }
        }

        private IEnumerator WaitForSecondsPauseAware(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                if (!_isPaused)
                {
                    elapsed += Time.deltaTime;
                }
                yield return null;
            }
        }

        private void ExecuteRandomSpecialAttack(List<BossAttacks> availableAttacks)
        {
            if (_isSpecialAttackActive) return;
            
            _isSpecialAttackActive = true;
            
            // Stop bullet attacks during special attack
            
            // Choose random attack
            BossAttacks chosenAttack = availableAttacks[UnityEngine.Random.Range(0, availableAttacks.Count)];
            
            // Execute the attack with callback to resume bullet attacks
            Attack(chosenAttack, () =>
            {
                _isSpecialAttackActive = false;
                _lastBulletAttackTime = Time.time;
            });
        }

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
            if (_isPaused)
            {
                _lastBulletAttackTime = Time.time - _lastTime;
                return;
            }
            if (_isSpecialAttackActive) return;
            _lastTime = Time.time - _lastBulletAttackTime;
            // Check if enough time has passed since last bullet attack
            if (_lastTime >= _stats.ProjectileSpawnRate)
            {
                _bulletAttack.Attack();
                _lastBulletAttackTime = Time.time;
            }
        }

        public override void CleanUp()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
            
            if (_currentCoroutine != null)
            {
                AttackerMono.StopCoroutine(_currentCoroutine);
            }
            _rageAttack?.Cleanup();
        }

        private bool Attack(BossAttacks attackType, Action onFinish = null)
        {
            if (_isPaused) return false;

            switch (attackType)
            {
                case BossAttacks.BulletsShot:
                    return _bulletAttack.Attack(onFinish);
                case BossAttacks.BigBulletShot:
                    return _bigBulletAttack.Attack(onFinish);
                case BossAttacks.SpawnDestroyers:
                    return _spawnDestroyersAttack.Attack(onFinish);
                case BossAttacks.Rage:
                    return _rageAttack.Attack(onFinish);
                default:
                    return false;
            }
        }
        
    }
}