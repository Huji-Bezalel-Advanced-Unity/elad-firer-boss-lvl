using System;
using System.Collections.Generic;
using System.Linq;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.InputSystem;
using _SPC.Core.Scripts.Managers;
using _SPC.GamePlay.UI.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public class PlayerStatsUpgrader : SPCStatsUpgrader
    {
        public enum UpgradeType
        {
            ExtraLife,
            ReplenishLife,
            ExtraShot,
            ShotSpeed,
            IncreaseAcceleration,
            DecreaseScoreThresholdMultiplier
        }
        
        private readonly PlayerStats _stats;
        private readonly SPCHealth _health;
        private readonly PlayerUpgradeChooseRenderer _renderer;
        private readonly InputSystem_Actions _inputSystem;

        private readonly List<UpgradeType> _availableUpgrades;
        private readonly List<UpgradeType> _currentChoices = new List<UpgradeType>();

        // Fields to store initial stat values for resetting
        private readonly int _initialNumberOfShots;
        private readonly float _initialProjectileSpeed;
        private readonly int _initialScoreThreshold;
        private readonly float _initialMaxHealth;
        private readonly float _initialAcceleration;
        private readonly float _initialScoreThresholdMultiplier;


        public PlayerStatsUpgrader(PlayerStats stats, SPCHealth health, PlayerUpgradeChooseRenderer renderer)
        {
            _stats = stats;
            _health = health;
            _renderer = renderer;
            _inputSystem = InputSystemBuffer.Instance.InputSystem;

            // Store initial values
            _initialNumberOfShots = stats.NumberOfShots;
            _initialProjectileSpeed = stats.ProjectileSpeed;
            _initialScoreThreshold = stats.ScoreThresholdUpgrade;
            _initialMaxHealth = stats.Health;
            _initialAcceleration = stats.Acceleration;
            _initialScoreThresholdMultiplier = stats.ScoreThresholdMultiplier;
            
            _availableUpgrades = new List<UpgradeType>((UpgradeType[])Enum.GetValues(typeof(UpgradeType)));
            
            GameEvents.OnUpdateScore += OnScoreUpdated;
            GameEvents.OnGameFinished += ResetStats;
        }

        private void OnScoreUpdated(long newScore)
        {
            if (newScore >= _stats.ScoreThresholdUpgrade)
            {
                _stats.ScoreThresholdUpgrade = (int)(_stats.ScoreThresholdUpgrade * _stats.ScoreThresholdMultiplier);
                PauseAndShowUpgrades();
            }
        }

        private void PauseAndShowUpgrades()
        {
            GameEvents.GamePaused();
            
            _currentChoices.Clear();
            var upgradePool = new List<UpgradeType>(_availableUpgrades);
            
            // Select two unique random upgrades using a weighted algorithm
            for (int i = 0; i < 2; i++)
            {
                if(upgradePool.Count == 0) break;

                var chosenUpgrade = GetWeightedRandomUpgrade(upgradePool);
                _currentChoices.Add(chosenUpgrade);
                upgradePool.Remove(chosenUpgrade);
            }
            
            if (_currentChoices.Count > 0)
            {
                var leftChoiceText = GetUpgradeDescription(_currentChoices[0]);
                var rightChoiceText = _currentChoices.Count > 1 ? GetUpgradeDescription(_currentChoices[1]) : "No other upgrades available";
                
                _renderer.RenderChoices(leftChoiceText, rightChoiceText, OnChoicesRendered);
            }
        }

        private UpgradeType GetWeightedRandomUpgrade(List<UpgradeType> availableUpgrades)
        {
            bool isHealthLow = _health.currentHealth / _health.maxHealth < 0.5f;
            var weightedUpgrades = new Dictionary<UpgradeType, int>();
            int totalWeight = 0;

            foreach (var upgrade in availableUpgrades)
            {
                int weight = 100; 
                if (isHealthLow && (upgrade == UpgradeType.ExtraLife || upgrade == UpgradeType.ReplenishLife))
                {
                    weight += 10; 
                }
                weightedUpgrades.Add(upgrade, weight);
                totalWeight += weight;
            }

            int randomValue = Random.Range(0, totalWeight);

            foreach (var item in weightedUpgrades)
            {
                if (randomValue < item.Value)
                {
                    return item.Key;
                }
                randomValue -= item.Value;
            }

            return availableUpgrades.Last(); // Fallback, should not be reached
        }

        private void OnChoicesRendered()
        {
            _inputSystem.Player.ChooseLeft.performed += OnChooseLeft;
            _inputSystem.Player.ChooseRight.performed += OnChooseRight;
        }

        private void OnChooseLeft(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            HandleChoice(0);
        }

        private void OnChooseRight(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            HandleChoice(1);
        }

        private void HandleChoice(int choiceIndex)
        {
            _inputSystem.Player.ChooseLeft.performed -= OnChooseLeft;
            _inputSystem.Player.ChooseRight.performed -= OnChooseRight;

            if (choiceIndex < _currentChoices.Count)
            {
                ApplyUpgrade(_currentChoices[choiceIndex]);
            }
            
            _renderer.UnrenderChoices(GameEvents.GameResumed);
        }

        private void ApplyUpgrade(UpgradeType upgrade)
        {
            PlayerUpgradeCounts.TryAdd(upgrade, 0);
            PlayerUpgradeCounts[upgrade]++;
            switch (upgrade)
            {
                case UpgradeType.ExtraLife:
                    var healthIncrease = _health.maxHealth * 0.25f;
                    _health.maxHealth += healthIncrease;
                    _health.currentHealth += _health.currentHealth * 0.25f;
                    break;
                case UpgradeType.ReplenishLife:
                    _health.AddLife(_health.maxHealth); 
                    break;
                case UpgradeType.ExtraShot:
                    _stats.NumberOfShots++;
                    break;
                case UpgradeType.ShotSpeed:
                    _stats.ProjectileSpeed *= 1.1f;
                    break;
                case UpgradeType.IncreaseAcceleration:
                    _stats.Acceleration *= 1.02f;
                    break;
                case UpgradeType.DecreaseScoreThresholdMultiplier:
                    _stats.ScoreThresholdMultiplier *= 0.9f;
                    break;
            }
        }
        
        private string GetUpgradeDescription(UpgradeType upgrade)
        {
            switch (upgrade)
            {
                case UpgradeType.ExtraLife: return "Extra Life: Increase Max HP by 25%";
                case UpgradeType.ReplenishLife: return "Replenish Life: Restore to full HP";
                case UpgradeType.ExtraShot: return "Extra Shot: Fire an additional projectile";
                case UpgradeType.ShotSpeed: return "Shot Speed: Increase projectile speed by 10%";
                case UpgradeType.IncreaseAcceleration: return "Maneuverability: Increase acceleration by 2%";
                case UpgradeType.DecreaseScoreThresholdMultiplier: return "Fast Learner: Get upgrades 10% faster";
                default: return "Unknown Upgrade";
            }
        }

        public override void ResetStats()
        {
            _stats.NumberOfShots = _initialNumberOfShots;
            _stats.ProjectileSpeed = _initialProjectileSpeed;
            _stats.ScoreThresholdUpgrade = _initialScoreThreshold;
            _stats.Health = _initialMaxHealth;
            _stats.Acceleration = _initialAcceleration;
            _stats.ScoreThresholdMultiplier = _initialScoreThresholdMultiplier;
            
            GameEvents.OnUpdateScore -= OnScoreUpdated;
            GameEvents.OnGameFinished -= ResetStats;
            _inputSystem.Player.ChooseLeft.performed -= OnChooseLeft;
            _inputSystem.Player.ChooseRight.performed -= OnChooseRight;
        }
    }
} 