using System;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Entities.Enemies.Destroyer;
using _SPC.GamePlay.Utils;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies for the BossStatsUpgrader, including stats, logger, and upgrade actions.
    /// </summary>
    public struct BossStatsUpgraderDependencies
    {
        public BossStats Stats;
        public DestroyerStats DestroyerStats;
        public GameLogger Logger;
        public Action[] OnBossUpgradedActions;
    }

    /// <summary>
    /// Handles boss stat upgrades and resets based on score and game events.
    /// </summary>
    public class BossStatsUpgrader : SPCStatsUpgrader
    {
        public enum UpgradeType
        {
            IncreaseDestroyerCount,
            IncreaseBulletCount,
            IncreaseDestroyerSmoothFactor,
            IncreaseDestroyerMovementSpeed,
            DecreaseWanderDelay,
            IncreaseDestroyerHealth,
            DecreaseProjectileSpawnRate,
            IncreaseAttacksSpeed
        }

        private readonly BossStats _bossStats;
        private readonly DestroyerStats _destroyerStats;
        private readonly GameLogger _enemyLogger;
        
        private List<UpgradeType> _availableUpgrades;
        private bool _isFirstUpgrade = true;
        private readonly Action[] _OnBossUpgraded;

        // Boss initial stats
        private int _initialBulletCount;
        private int _initialNumberOfEnemiesToSpawn;
        private float _initialBossProjectileSpeed;
        private float _initialBossProjectileSpawnRate;
        private float _initialDestroyerSpawnTime;
        private long _initialScoreThreshold;
        private float _initialRageChargeTime;
        private float _laserMoveSpeed;
        private float _initialLaserStretchTime;

        // Destroyer initial stats
        private float _initialDestroyerProjectileSpeed;
        private float _initialDestroyerProjectileSpawnRate;
        private float _initialSmoothFactor;
        private float _initialMovementSpeed;
        private float _initialMinWanderDelay;
        private float _initialMaxWanderDelay;
        private float _initialDestroyerHealth;

        private event Action OnBossStatsUpgraded;

        /// <summary>
        /// Initializes the BossStatsUpgrader and subscribes to relevant game events.
        /// </summary>
        public BossStatsUpgrader(BossStatsUpgraderDependencies deps)
        {
            _bossStats = deps.Stats;
            _destroyerStats = deps.DestroyerStats;
            _enemyLogger = deps.Logger;
            _OnBossUpgraded = deps.OnBossUpgradedActions;
            
            SubscribeToUpgradeActions();
            StoreInitialStats();
            InitializeUpgrades();
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Handles score updates and applies upgrades if the threshold is reached.
        /// </summary>
        private void OnScoreUpdated(long newScore)
        {
            if (newScore >= _bossStats.scoreThresholdUpgrade)
            {
                UpdateScoreThreshold();
                ApplyBossUpgrade();
                OnBossStatsUpgraded?.Invoke();
            }
        }

        /// <summary>
        /// Applies a random or first-time upgrade to the boss and logs the result.
        /// </summary>
        private void ApplyBossUpgrade() 
        {
            UpgradeType chosenUpgrade = ChooseUpgrade();
            UpdateUpgradeCount(chosenUpgrade);
            LogUpgrade(chosenUpgrade);
            ApplyUpgradeEffect(chosenUpgrade);
        }
        
        /// <summary>
        /// Resets all boss and destroyer stats to their initial values and unsubscribes from events.
        /// </summary>
        public override void ResetStats()
        {
            ClearUpgradeCounts();
            ResetBossStats();
            ResetDestroyerStats();
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Subscribes to the provided upgrade actions.
        /// </summary>
        private void SubscribeToUpgradeActions()
        {
            if (_OnBossUpgraded != null)
            {
                foreach (var action in _OnBossUpgraded)
                {
                    OnBossStatsUpgraded += action;
                }
            }
        }

        /// <summary>
        /// Stores initial values of all stats for later reset.
        /// </summary>
        private void StoreInitialStats()
        {
            StoreInitialBossStats();
            StoreInitialDestroyerStats();
        }

        /// <summary>
        /// Stores initial boss stats values.
        /// </summary>
        private void StoreInitialBossStats()
        {
            _initialBulletCount = _bossStats.bulletCount;
            _initialNumberOfEnemiesToSpawn = _bossStats.numberOfEnemiesToSpawn;
            _initialBossProjectileSpeed = _bossStats.ProjectileSpeed;
            _initialBossProjectileSpawnRate = _bossStats.ProjectileSpawnRate;
            _initialScoreThreshold = _bossStats.scoreThresholdUpgrade;
            _initialRageChargeTime = _bossStats.rageChargeTime;
            _laserMoveSpeed = _bossStats.laserMoveSpeed;
            _initialLaserStretchTime = _bossStats.laserStretchTime;
        }

        /// <summary>
        /// Stores initial destroyer stats values.
        /// </summary>
        private void StoreInitialDestroyerStats()
        {
            _initialDestroyerProjectileSpeed = _destroyerStats.ProjectileSpeed;
            _initialDestroyerProjectileSpawnRate = _destroyerStats.ProjectileSpawnRate;
            _initialSmoothFactor = _destroyerStats.SmoothFactor;
            _initialMovementSpeed = _destroyerStats.MovementSpeed;
            _initialMinWanderDelay = _destroyerStats.minWanderDelay;
            _initialMaxWanderDelay = _destroyerStats.maxWanderDelay;
            _initialDestroyerHealth = _destroyerStats.Health;
        }

        /// <summary>
        /// Initializes the available upgrades list.
        /// </summary>
        private void InitializeUpgrades()
        {
            _availableUpgrades = new List<UpgradeType>((UpgradeType[])Enum.GetValues(typeof(UpgradeType)));
        }

        /// <summary>
        /// Subscribes to game events for score updates and game state changes.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            GameEvents.OnGameFinished += ResetStats;
            GameEvents.OnUpdateScore += OnScoreUpdated;
            GameEvents.OnGameLoss += ResetStats;
        }

        /// <summary>
        /// Updates the score threshold for the next upgrade.
        /// </summary>
        private void UpdateScoreThreshold()
        {
            _bossStats.scoreThresholdUpgrade = (long)(_bossStats.scoreThresholdUpgrade * _bossStats.scoreThresholdMultiplier);
        }

        /// <summary>
        /// Chooses the appropriate upgrade type based on game state.
        /// </summary>
        private UpgradeType ChooseUpgrade()
        {
            if (_isFirstUpgrade)
            {
                _isFirstUpgrade = false;
                return UpgradeType.IncreaseBulletCount;
            }
            
            if (_availableUpgrades.Count == 0) 
                return UpgradeType.IncreaseBulletCount; // Fallback
            
            int randomIndex = Random.Range(0, _availableUpgrades.Count);
            return _availableUpgrades[randomIndex];
        }

        /// <summary>
        /// Updates the count for the chosen upgrade type.
        /// </summary>
        private void UpdateUpgradeCount(UpgradeType upgradeType)
        {
            BossUpgradeCounts.TryAdd(upgradeType, 0);
            BossUpgradeCounts[upgradeType]++;
        }

        /// <summary>
        /// Logs the chosen upgrade if logger is available.
        /// </summary>
        private void LogUpgrade(UpgradeType upgradeType)
        {
            if (_enemyLogger != null)
            {
                _enemyLogger.Log($"Boss chose upgrade: {upgradeType} (Total: {BossUpgradeCounts[upgradeType]})");
            }
        }

        /// <summary>
        /// Applies the effect of the chosen upgrade type.
        /// </summary>
        private void ApplyUpgradeEffect(UpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case UpgradeType.IncreaseDestroyerCount:
                    _bossStats.numberOfEnemiesToSpawn++;
                    break;
                case UpgradeType.IncreaseBulletCount:
                    _bossStats.bulletCount++;
                    break;
                case UpgradeType.IncreaseAttacksSpeed:
                    ApplyAttackSpeedUpgrade();
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
                    _destroyerStats.Health += 20;
                    break;
                case UpgradeType.DecreaseProjectileSpawnRate:
                    ApplyProjectileSpawnRateUpgrade();
                    break;
            }
        }

        /// <summary>
        /// Applies attack speed upgrade to both boss and destroyer.
        /// </summary>
        private void ApplyAttackSpeedUpgrade()
        {
            _bossStats.ProjectileSpeed *= 1.1f;
            _destroyerStats.ProjectileSpeed *= 1.1f;
            _bossStats.rageChargeTime *= 0.9f;
            _bossStats.laserMoveSpeed *= 1.1f;
            _bossStats.laserStretchTime *= 0.9f;
        }

        /// <summary>
        /// Applies projectile spawn rate upgrade to both boss and destroyer.
        /// </summary>
        private void ApplyProjectileSpawnRateUpgrade()
        {
            _bossStats.ProjectileSpawnRate *= 0.9f;
            _destroyerStats.ProjectileSpawnRate *= 0.9f;
        }

        /// <summary>
        /// Clears all upgrade count dictionaries.
        /// </summary>
        private void ClearUpgradeCounts()
        {
            PlayerUpgradeCounts.Clear();
            BossUpgradeCounts.Clear();
        }

        /// <summary>
        /// Resets boss stats to their initial values.
        /// </summary>
        private void ResetBossStats()
        {
            _bossStats.bulletCount = _initialBulletCount;
            _bossStats.numberOfEnemiesToSpawn = _initialNumberOfEnemiesToSpawn;
            _bossStats.ProjectileSpeed = _initialBossProjectileSpeed;
            _bossStats.ProjectileSpawnRate = _initialBossProjectileSpawnRate;
            _bossStats.scoreThresholdUpgrade = _initialScoreThreshold;
            _bossStats.rageChargeTime = _initialRageChargeTime;
            _bossStats.laserMoveSpeed = _laserMoveSpeed;
            _bossStats.laserStretchTime = _initialLaserStretchTime;
        }

        /// <summary>
        /// Resets destroyer stats to their initial values.
        /// </summary>
        private void ResetDestroyerStats()
        {
            _destroyerStats.ProjectileSpeed = _initialDestroyerProjectileSpeed;
            _destroyerStats.ProjectileSpawnRate = _initialDestroyerProjectileSpawnRate;
            _destroyerStats.SmoothFactor = _initialSmoothFactor;
            _destroyerStats.MovementSpeed = _initialMovementSpeed;
            _destroyerStats.minWanderDelay = _initialMinWanderDelay;
            _destroyerStats.maxWanderDelay = _initialMaxWanderDelay;
            _destroyerStats.Health = _initialDestroyerHealth;
        }

        /// <summary>
        /// Unsubscribes from all game events and upgrade actions.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnGameFinished -= ResetStats;
            GameEvents.OnGameLoss -= ResetStats;
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