using System;
using System.Linq;
using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;
using _SPC.GamePlay.Player.Scripts.Controllers;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Boss.Scripts.Controllers
{
    public class BossAiStatsChooser : SpcBasicAiModule
    {
        private readonly GameLogger _enemyLogger;
        private int _currentScore;
        private static SPCHealth _playerHealth; // we have one player health so its static

        public BossAiStatsChooser(GameLogger enemyLogger = null)
        {
            _enemyLogger = enemyLogger;
            GameEvents.OnUpdateScore += OnUpdateScore;
        }

        private void OnUpdateScore(int score)
        {
            _currentScore = score;
        }

        public override void Fit()
        {
            // No-op for now
        }

        public override object Predict()
        {
            var availableUpgrades = Enum.GetValues(typeof(BossStatsUpgrader.UpgradeType)).Cast<BossStatsUpgrader.UpgradeType>().ToList();
            var weights = availableUpgrades.ToDictionary(upg => upg, upg => 100);

            // --- Expanded rule-based weighting logic ---
            // At the start of the game, all upgrades have equal probability for extra weight
            if (_currentScore < 4000)
            {
                foreach (var upg in availableUpgrades)
                {
                    weights[upg] += 10;
                }
            }

            // Player upgrade counts
            int playerExtraShots = SPCStatsUpgrader.PlayerUpgradeCounts.TryGetValue(PlayerStatsUpgrader.UpgradeType.ExtraShot, out var shots) ? shots : 0;
            int playerShotSpeed = SPCStatsUpgrader.PlayerUpgradeCounts.TryGetValue(PlayerStatsUpgrader.UpgradeType.ShotSpeed, out var shotSpeed) ? shotSpeed : 0;
            int playerAcceleration = SPCStatsUpgrader.PlayerUpgradeCounts.TryGetValue(PlayerStatsUpgrader.UpgradeType.IncreaseAcceleration, out var acc) ? acc : 0;
            int playerReplenishLife = SPCStatsUpgrader.PlayerUpgradeCounts.TryGetValue(PlayerStatsUpgrader.UpgradeType.ReplenishLife, out var rep) ? rep : 0;
            int playerExtraLife = SPCStatsUpgrader.PlayerUpgradeCounts.TryGetValue(PlayerStatsUpgrader.UpgradeType.ExtraLife, out var exLife) ? exLife : 0;
            int playerFastLearner = SPCStatsUpgrader.PlayerUpgradeCounts.TryGetValue(PlayerStatsUpgrader.UpgradeType.DecreaseScoreThresholdMultiplier, out var fastLearner) ? fastLearner : 0;

            // Boss upgrade counts
            int bossBulletUpgrades = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.IncreaseBulletCount, out var bcount) ? bcount : 0;
            int bossHealthUpgrades = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.IncreaseDestroyerHealth, out var bhealth) ? bhealth : 0;
            int bossProjectileSpeed = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed, out var bps) ? bps : 0;
            int bossSmoothFactor = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.IncreaseDestroyerSmoothFactor, out var bsf) ? bsf : 0;
            int bossMovementSpeed = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.IncreaseDestroyerMovementSpeed, out var bms) ? bms : 0;
            int bossWanderDelay = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.DecreaseWanderDelay, out var bwd) ? bwd : 0;
            int bossProjectileSpawnRate = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.DecreaseProjectileSpawnRate, out var bpsr) ? bpsr : 0;
            int bossDestroyerSpawnTime = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.DecreaseDestroyerSpawnTime, out var bdst) ? bdst : 0;
            int bossDestroyerCount = SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(BossStatsUpgrader.UpgradeType.IncreaseDestroyerCount, out var bdc) ? bdc : 0;

            // Player is stacking extra shots, boss should increase health
            if (playerExtraShots > 2)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerHealth] += 50;
            // Player is stacking shot speed, boss should increase movement speed
            if (playerShotSpeed > 2)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerMovementSpeed] += 40;
            // Player is stacking acceleration, boss should decrease wander delay
            if (playerAcceleration > 2)
                weights[BossStatsUpgrader.UpgradeType.DecreaseWanderDelay] += 30;
            // Player is stacking replenish life, boss should increase bullet count
            if (playerReplenishLife > 1)
                weights[BossStatsUpgrader.UpgradeType.IncreaseBulletCount] += 25;
            // Player is stacking extra life, boss should increase projectile speed
            if (playerExtraLife > 1)
                weights[BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed] += 25;
            // Player is stacking fast learner, boss should decrease destroyer spawn time
            if (playerFastLearner > 1)
                weights[BossStatsUpgrader.UpgradeType.DecreaseDestroyerSpawnTime] += 20;

            // Boss is behind in bullet upgrades, prioritize more bullets
            if (bossBulletUpgrades < playerExtraShots)
                weights[BossStatsUpgrader.UpgradeType.IncreaseBulletCount] += 30;
            // Boss is behind in health upgrades, prioritize more health
            if (bossHealthUpgrades < playerExtraLife)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerHealth] += 30;
            // Boss is behind in projectile speed, prioritize more speed
            if (bossProjectileSpeed < playerShotSpeed)
                weights[BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed] += 30;
            // Boss is behind in movement speed, prioritize more speed
            if (bossMovementSpeed < playerAcceleration)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerMovementSpeed] += 30;

            // If player health is low, boss gets more aggressive
            if (_playerHealth != null && _playerHealth.currentHealth / _playerHealth.maxHealth < 0.5f)
            {
                weights[BossStatsUpgrader.UpgradeType.IncreaseBulletCount] += 40;
                weights[BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed] += 40;
            }

            // If boss has many destroyers, prioritize decreasing spawn time
            if (bossDestroyerCount > 3)
                weights[BossStatsUpgrader.UpgradeType.DecreaseDestroyerSpawnTime] += 30;
            // If boss has many health upgrades, prioritize projectile spawn rate
            if (bossHealthUpgrades > 3)
                weights[BossStatsUpgrader.UpgradeType.DecreaseProjectileSpawnRate] += 20;

            // If score is high, boss gets more aggressive
            if (_currentScore > 5000)
            {
                weights[BossStatsUpgrader.UpgradeType.IncreaseBulletCount] += 50;
                weights[BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed] += 50;
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerMovementSpeed] += 50;
            }

            // If player has not upgraded much, boss picks random upgrades
            int totalPlayerUpgrades = playerExtraShots + playerShotSpeed + playerAcceleration + playerReplenishLife + playerExtraLife + playerFastLearner;
            if (totalPlayerUpgrades < 2)
            {
                foreach (var upg in availableUpgrades)
                {
                    weights[upg] += 10;
                }
            }

            // New: If boss has a lot of one upgrade, diversify
            foreach (var upg in availableUpgrades)
            {
                if (SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(upg, out var count) && count > 5)
                {
                    foreach (var other in availableUpgrades)
                    {
                        if (other != upg)
                            weights[other] += 5;
                    }
                }
            }

            // New: If player is balanced, boss picks random
            int maxPlayerUpgrade = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(playerExtraShots, playerShotSpeed), playerAcceleration), playerReplenishLife), playerExtraLife), playerFastLearner);
            if (maxPlayerUpgrade <= 2)
            {
                foreach (var upg in availableUpgrades)
                {
                    weights[upg] += 15;
                }
            }

            // New: If boss is losing (score is much higher than upgrades), prioritize health and bullet count
            if (_currentScore > 10000 && (bossHealthUpgrades + bossBulletUpgrades) < 5)
            {
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerHealth] += 60;
                weights[BossStatsUpgrader.UpgradeType.IncreaseBulletCount] += 60;
            }

            // New: If player has many upgrades in one stat, boss counters
            if (playerExtraShots > 4)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerCount] += 40;
            if (playerShotSpeed > 4)
                weights[BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed] += 40;
            if (playerAcceleration > 4)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerMovementSpeed] += 40;
            if (playerReplenishLife > 3)
                weights[BossStatsUpgrader.UpgradeType.DecreaseProjectileSpawnRate] += 30;
            if (playerExtraLife > 3)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerHealth] += 30;
            if (playerFastLearner > 3)
                weights[BossStatsUpgrader.UpgradeType.DecreaseDestroyerSpawnTime] += 30;

            // New: If boss has many upgrades in one stat, reduce its own weight for that stat
            foreach (var upg in availableUpgrades)
            {
                if (SPCStatsUpgrader.BossUpgradeCounts.TryGetValue(upg, out var count) && count > 8)
                {
                    weights[upg] -= 20;
                }
            }

            // New: If player health is very high, boss prioritizes damage
            if (_playerHealth != null && _playerHealth.currentHealth / _playerHealth.maxHealth > 0.9f)
            {
                weights[BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed] += 30;
                weights[BossStatsUpgrader.UpgradeType.IncreaseBulletCount] += 30;
            }

            // New: If boss has not used a certain upgrade, give it a bonus
            foreach (var upg in availableUpgrades)
            {
                if (!SPCStatsUpgrader.BossUpgradeCounts.ContainsKey(upg))
                    weights[upg] += 20;
            }

            // New: If player has not used a certain upgrade, boss deprioritizes countering it
            if (playerExtraShots == 0)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerHealth] -= 10;
            if (playerShotSpeed == 0)
                weights[BossStatsUpgrader.UpgradeType.IncreaseDestroyerMovementSpeed] -= 10;
            if (playerAcceleration == 0)
                weights[BossStatsUpgrader.UpgradeType.DecreaseWanderDelay] -= 10;
            if (playerReplenishLife == 0)
                weights[BossStatsUpgrader.UpgradeType.IncreaseBulletCount] -= 10;
            if (playerExtraLife == 0)
                weights[BossStatsUpgrader.UpgradeType.IncreaseProjectileSpeed] -= 10;
            if (playerFastLearner == 0)
                weights[BossStatsUpgrader.UpgradeType.DecreaseDestroyerSpawnTime] -= 10;
            

            // --- End of expanded rule-based weighting logic ---

            // Log weights in ascending order
            if (_enemyLogger != null)
            {
                var sortedWeights = weights.OrderBy(x => x.Value).ToList();
                foreach (var kvp in sortedWeights)
                {
                    _enemyLogger.Log($"Upgrade: {kvp.Key}, Weight: {kvp.Value}");
                }
            }

            // Weighted random selection
            int totalWeight = weights.Values.Sum();
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            foreach (var kvp in weights)
            {
                if (randomValue < kvp.Value)
                    return kvp.Key;
                randomValue -= kvp.Value;
            }
            // Fallback
            return availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];
        }

        public override void Clear()
        {
            GameEvents.OnUpdateScore -= OnUpdateScore;
        }


        public static void SetPlayerHealth(SPCHealth health)
        {
            _playerHealth = health;
        }
    }
} 