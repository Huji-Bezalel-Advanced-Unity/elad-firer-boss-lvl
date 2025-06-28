using System;
using System.Collections;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies required by the BossAttacker for attacks and phase management.
    /// </summary>
    public struct BossAttackerDependencies
    {
        public BoxCollider2D ArenaCollider;
        public BulletMonoPool DestroyerPool;
        public GameObject ExplosionPrefab;
        public Transform ExplosionsFather;
        public Collider2D BossCollider;
        public Transform DummyParentTransform;
        public SPCHealth Health;
        public BossFaceChanger FaceChanger;
        public GameObject LaserPrefab;
        public Transform LaserTransform;
    }

    /// <summary>
    /// Enum of all possible boss attacks.
    /// </summary>
    public enum BossAttacks
    {
        None,
        BulletsShot,
        BigBulletShot,
        SpawnDestroyers,
        Rage,
        Laser
    }
    
    /// <summary>
    /// Controls the boss's attack logic, phase transitions, and special attack selection.
    /// </summary>
    public class BossAttacker : SPCAttacker
    {
        private readonly BossStats _stats;
        private readonly BossAttackerDependencies _bossDeps;
        private readonly BossBulletAttack _bulletAttack;
        private readonly BossBigBulletAttack _bigBulletAttack;
        private readonly BossSpawnDestroyersAttack _spawnDestroyersAttack;
        private readonly BossRageAttack _rageAttack;
        private readonly BossLaserAttack _laserAttack;
        private bool _isPaused = false;
        private Coroutine _currentCoroutine;
        
        private float _lastBulletAttackTime = 0f;
        private BossAttacks _lastAttack = BossAttacks.None;

        private readonly List<BossAttacks> _phase1Attacks = new List<BossAttacks> { BossAttacks.BigBulletShot, BossAttacks.SpawnDestroyers };
        private readonly List<BossAttacks> _phase2Attacks = new List<BossAttacks> { BossAttacks.BigBulletShot, BossAttacks.SpawnDestroyers, BossAttacks.Laser };
        private readonly List<BossAttacks> _phase3Attacks = new List<BossAttacks> { BossAttacks.BigBulletShot, BossAttacks.SpawnDestroyers, BossAttacks.Rage, BossAttacks.Laser };
        private float _lastTime;

        /// <summary>
        /// Initializes the boss attacker, its attacks, and phase transitions.
        /// </summary>
        public BossAttacker(BossStats stats, AttackerDependencies deps, BossAttackerDependencies bossDeps) : base(deps)
        {
            _stats = stats;
            _bossDeps = bossDeps;

            // Initialize attacks
            _bulletAttack = new BossBulletAttack(_stats, new BossBulletAttackDependencies
            {
                EntityTransform = EntityTransform,
                DummyParentTransform = _bossDeps.DummyParentTransform,
                BossBulletPool = ProjectilePools[WeaponType.BossBullet],
                Logger = Logger
            });
            _bigBulletAttack = new BossBigBulletAttack(_stats, new BossBigBulletAttackDependencies
            {
                EntityTransform = EntityTransform,
                DummyParentTransform = _bossDeps.DummyParentTransform,
                MainTarget = MainTarget,
                BigBulletPool = ProjectilePools[WeaponType.BossBigBullet],
                Logger = Logger
            });
            _spawnDestroyersAttack = new BossSpawnDestroyersAttack(_stats, new BossSpawnDestroyersAttackDependencies
            {
                ArenaCollider = _bossDeps.ArenaCollider,
                DestroyerPool = _bossDeps.DestroyerPool,
                ExplosionPrefab = _bossDeps.ExplosionPrefab,
                ExplosionsFather = _bossDeps.ExplosionsFather,
                BossCollider = _bossDeps.BossCollider,
                MainTarget = MainTarget,
                TargetTransforms = new List<Transform>(TargetTransforms),
                AttackerMono = AttackerMono
            });
            _rageAttack = new BossRageAttack(_stats, new BossRageAttackDependencies
            {
                EntityTransform = EntityTransform,
                MainTarget = MainTarget,
                Logger = Logger
            });
            _laserAttack = new BossLaserAttack(_stats, new BossLaserAttackDependencies
            {
                EntityTransform = EntityTransform,
                ArenaCollider = _bossDeps.ArenaCollider,
                LaserPrefab = _bossDeps.LaserPrefab,
                Logger = Logger,
                AttackerMono = AttackerMono,
                LaserTransform = _bossDeps.LaserTransform,
            });

            // Set up health-based phase transitions
            _bossDeps.Health.AddHPAction(_bossDeps.Health.maxHealth * 2 / 3,
                new SPCHealthAction(() => SwitchPhase(_phase2Attacks, _stats.phase2SpecialAttackInterval)));
            _bossDeps.Health.AddHPAction(_bossDeps.Health.maxHealth * 1 / 3,
                new SPCHealthAction(() => SwitchPhase(_phase3Attacks, _stats.phase3SpecialAttackInterval)));

            // Start phase 1
            _currentCoroutine = AttackerMono.StartCoroutine(PhaseAttack(_phase1Attacks, _stats.phase1SpecialAttackInterval));
            Attack();
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        /// <summary>
        /// Switches to a new attack phase with the given attacks and interval.
        /// </summary>
        private void SwitchPhase(List<BossAttacks> attacks, float interval)
        {
            if (_currentCoroutine != null)
            {
                AttackerMono.StopCoroutine(_currentCoroutine);
            }
            _currentCoroutine = AttackerMono.StartCoroutine(PhaseAttack(attacks, interval));
        }

        /// <summary>
        /// Coroutine for handling phase-based special attacks.
        /// </summary>
        private IEnumerator PhaseAttack(List<BossAttacks> availableAttacks, float attackInterval)
        {
            while (true)
            {
                while (_isPaused) yield return null;
                yield return AttackerMono.StartCoroutine(WaitForSeconds(attackInterval));
                while (_isPaused) yield return null;
                ExecuteRandomSpecialAttack(availableAttacks);
            }
        }

        /// <summary>
        /// Waits for the specified number of seconds, pausing if the game is paused.
        /// </summary>
        private IEnumerator WaitForSeconds(float seconds)
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

        /// <summary>
        /// Selects and executes a random special attack from the available list.
        /// </summary>
        private void ExecuteRandomSpecialAttack(List<BossAttacks> availableAttacks)
        {
            _bossDeps.FaceChanger?.SetAngryFace();
            BossAttacks chosenAttack;
            do
            {
                chosenAttack = availableAttacks[UnityEngine.Random.Range(0, availableAttacks.Count)];
            } while (_lastAttack == chosenAttack);
            _lastAttack = chosenAttack;
            Attack(chosenAttack, () =>
            {
                _bossDeps.FaceChanger?.SetNormalFace();
            });
        }

        /// <summary>
        /// Pauses the boss attack logic.
        /// </summary>
        private void OnGamePaused() => _isPaused = true;

        /// <summary>
        /// Resumes the boss attack logic.
        /// </summary>
        private void OnGameResumed() => _isPaused = false;

        /// <summary>
        /// Handles the boss's regular bullet attack and cooldown logic.
        /// </summary>
        public override void Attack()
        {
            AttackerMono.StartCoroutine(AttackerCourtine());
        }

        private IEnumerator AttackerCourtine()
        {
            while (true)
            {
                while(_isPaused)
                {
                    yield return null;
                    _lastBulletAttackTime = Time.time - _lastTime;
                }
                _lastTime = Time.time - _lastBulletAttackTime;
                if (_lastTime >= _stats.ProjectileSpawnRate)
                {
                    _bulletAttack.Attack();
                    _lastBulletAttackTime = Time.time;
                }
                yield return null;
            }
            
        }

        /// <summary>
        /// Cleans up event subscriptions and coroutines.
        /// </summary>
        public override void CleanUp()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
            if (_currentCoroutine != null)
            {
                AttackerMono.StopCoroutine(_currentCoroutine);
            }
            _rageAttack?.Cleanup();
            _laserAttack?.Cleanup();
        }

        /// <summary>
        /// Executes the specified attack type, invoking the callback when finished.
        /// </summary>
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
                case BossAttacks.Laser:
                    return _laserAttack.Attack(onFinish);
                default:
                    return false;
            }
        }
    }
}