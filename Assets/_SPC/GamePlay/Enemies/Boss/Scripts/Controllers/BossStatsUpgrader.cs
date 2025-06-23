using System;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Utils;
using _SPC.GamePlay.Enemies.Destroyer.Scripts;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
{
    public struct BossStatsUpgraderDependencies
    {
        public BossStats Stats;
        public DestroyerStats DestroyerStats;
        public GameLogger Logger;
        public Action[] OnBossUpgradedActions;
        public SpcBasicAiModule AiModule;
    }

    public class BossStatsUpgrader : SPCStatsUpgrader
    {
        public enum UpgradeType
        {
            IncreaseDestroyerCount,
            IncreaseBulletCount,
            IncreaseProjectileSpeed,
            IncreaseDestroyerSmoothFactor,
            IncreaseDestroyerMovementSpeed,
            DecreaseWanderDelay,
            IncreaseDestroyerHealth,
            DecreaseProjectileSpawnRate,
            DecreaseDestroyerSpawnTime
        }

        private readonly BossStats _bossStats;
        private readonly DestroyerStats _destroyerStats;
        private readonly SpcBasicAiModule _aiModule;
        private readonly GameLogger _enemyLogger;
        
        private readonly List<UpgradeType> _availableUpgrades;
        private bool _isFirstUpgrade = true;

        // Boss initial stats
        private readonly int _initialBulletCount;
        private readonly int _initialNumberOfEnemiesToSpawn;
        private readonly float _initialBossProjectileSpeed;
        private readonly float _initialBossProjectileSpawnRate;
        private readonly float _initialDestroyerSpawnTime;
        private readonly int _initialScoreThreshold;

        // Destroyer initial stats
        private readonly float _initialDestroyerProjectileSpeed;
        private readonly float _initialDestroyerProjectileSpawnRate;
        private readonly float _initialSmoothFactor;
        private readonly float _initialMovementSpeed;
        private readonly float _initialMinWanderDelay;
        private readonly float _initialMaxWanderDelay;
        private readonly float _initialDestroyerHealth;
        private readonly Action[] _OnBossUpgraded;


        private event Action OnBossStatsUpgraded;
        public BossStatsUpgrader(BossStatsUpgraderDependencies deps)
        {
            _bossStats = deps.Stats;
            _destroyerStats = deps.DestroyerStats;
            _aiModule = deps.AiModule;
            _enemyLogger = deps.Logger;
            _OnBossUpgraded = deps.OnBossUpgradedActions;
            if (_OnBossUpgraded != null)
            {
                foreach (var action in _OnBossUpgraded)
                {
                    OnBossStatsUpgraded += action;
                }
            }
            // Store initial BossStats
            _initialBulletCount = _bossStats.bulletCount;
            _initialNumberOfEnemiesToSpawn = _bossStats.numberOfEnemiesToSpawn;
            _initialBossProjectileSpeed = _bossStats.ProjectileSpeed;
            _initialBossProjectileSpawnRate = _bossStats.ProjectileSpawnRate;
            _initialDestroyerSpawnTime = _bossStats.destroyerSpawnTime;
            _initialScoreThreshold = _bossStats.scoreThresholdUpgrade;

            // Store initial DestroyerStats
            _initialDestroyerProjectileSpeed = _destroyerStats.ProjectileSpeed;
            _initialDestroyerProjectileSpawnRate = _destroyerStats.ProjectileSpawnRate;
            _initialSmoothFactor = _destroyerStats.SmoothFactor;
            _initialMovementSpeed = _destroyerStats.MovementSpeed;
            _initialMinWanderDelay = _destroyerStats.minWanderDelay;
            _initialMaxWanderDelay = _destroyerStats.maxWanderDelay;
            _initialDestroyerHealth = _destroyerStats.Health;
            _availableUpgrades = new List<UpgradeType>((UpgradeType[])Enum.GetValues(typeof(UpgradeType)));
            GameEvents.OnGameFinished += ResetStats;
            GameEvents.OnUpdateScore += OnScoreUpdated;
        }

        private void OnScoreUpdated(int newScore)
        {
            if (newScore >= _bossStats.scoreThresholdUpgrade)
            {
                _bossStats.scoreThresholdUpgrade = (int)(_bossStats.scoreThresholdUpgrade * _bossStats.scoreThresholdMultiplier);
                ApplyAiUpgrade();
                OnBossStatsUpgraded?.Invoke();
            }
        }

        private void ApplyAiUpgrade()
        {
            UpgradeType chosenUpgrade;
            if (_isFirstUpgrade)
            {
                chosenUpgrade = UpgradeType.IncreaseBulletCount;
                _isFirstUpgrade = false;
            }
            else if (_aiModule != null)
            {
                chosenUpgrade = (UpgradeType)_aiModule.Predict();
            }
            else
            {
                if (_availableUpgrades.Count == 0) return;
                int randomIndex = Random.Range(0, _availableUpgrades.Count);
                chosenUpgrade = _availableUpgrades[randomIndex];
            }
            BossUpgradeCounts.TryAdd(chosenUpgrade, 0);
            BossUpgradeCounts[chosenUpgrade]++;
            if (_enemyLogger != null)
            {
                _enemyLogger.Log($"Boss chose upgrade: {chosenUpgrade} (Total: {BossUpgradeCounts[chosenUpgrade]})");
            }
            switch (chosenUpgrade)
            {
                case UpgradeType.IncreaseDestroyerCount:
                    _bossStats.numberOfEnemiesToSpawn++;
                    break;
                case UpgradeType.IncreaseBulletCount:
                    _bossStats.bulletCount++;
                    break;
                case UpgradeType.IncreaseProjectileSpeed:
                    _bossStats.ProjectileSpeed *= 1.1f;
                    _destroyerStats.ProjectileSpeed *= 1.1f;
                    break;
                case UpgradeType.IncreaseDestroyerSmoothFactor:
                    _destroyerStats.SmoothFactor *= 1.2f; 
                    break;
                case UpgradeType.IncreaseDestroyerMovementSpeed:
                    _destroyerStats.MovementSpeed *= 1.15f;
                    break;
                case UpgradeType.DecreaseWanderDelay:
                    _destroyerStats.minWanderDelay *= 0.8f;
                    _destroyerStats.maxWanderDelay *= 0.8f;
                    break;
                case UpgradeType.IncreaseDestroyerHealth:
                    _destroyerStats.Health += 10;
                    break;
                case UpgradeType.DecreaseProjectileSpawnRate:
                    _bossStats.ProjectileSpawnRate *= 0.9f;
                    _destroyerStats.ProjectileSpawnRate *= 0.9f;
                    break;
                case UpgradeType.DecreaseDestroyerSpawnTime:
                    _bossStats.destroyerSpawnTime *= 0.925f; // 7.5% decrease
                    break;
            }
        }
        
        public override void ResetStats()
        {
            _aiModule?.Clear();
            PlayerUpgradeCounts.Clear();
            BossUpgradeCounts.Clear();
            // Reset BossStats
            _bossStats.bulletCount = _initialBulletCount;
            _bossStats.numberOfEnemiesToSpawn = _initialNumberOfEnemiesToSpawn;
            _bossStats.ProjectileSpeed = _initialBossProjectileSpeed;
            _bossStats.ProjectileSpawnRate = _initialBossProjectileSpawnRate;
            _bossStats.destroyerSpawnTime = _initialDestroyerSpawnTime;
            _bossStats.scoreThresholdUpgrade = _initialScoreThreshold;
            
            // Reset DestroyerStats
            _destroyerStats.ProjectileSpeed = _initialDestroyerProjectileSpeed;
            _destroyerStats.ProjectileSpawnRate = _initialDestroyerProjectileSpawnRate;
            _destroyerStats.SmoothFactor = _initialSmoothFactor;
            _destroyerStats.MovementSpeed = _initialMovementSpeed;
            _destroyerStats.minWanderDelay = _initialMinWanderDelay;
            _destroyerStats.maxWanderDelay = _initialMaxWanderDelay;
            _destroyerStats.Health = _initialDestroyerHealth;
            
            GameEvents.OnGameFinished -= ResetStats;
            GameEvents.OnUpdateScore -= OnScoreUpdated;
            if (_OnBossUpgraded != null)
            {
                foreach (var action in _OnBossUpgraded)
                {
                    OnBossStatsUpgraded -= action;
                }
            }
        }
    }
} 