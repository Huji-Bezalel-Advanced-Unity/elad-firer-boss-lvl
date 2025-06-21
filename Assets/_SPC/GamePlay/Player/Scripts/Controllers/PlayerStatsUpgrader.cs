using System;
using System.Collections.Generic;
using System.Linq;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.InputSystem;
using _SPC.GamePlay.Managers;
using _SPC.GamePlay.UI.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public class PlayerStatsUpgrader
    {
        private enum UpgradeType
        {
            ExtraLife,
            ReplenishLife,
            ExtraShot,
            ShotSpeed
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
            
            _availableUpgrades = new List<UpgradeType>((UpgradeType[])Enum.GetValues(typeof(UpgradeType)));
            
            GameEvents.OnUpdateScore += OnScoreUpdated;
            GameEvents.OnGameFinished += ResetStats;
        }

        private void OnScoreUpdated(int newScore)
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
            
            // Select two unique random upgrades
            for (int i = 0; i < 2; i++)
            {
                if(upgradePool.Count == 0) break;
                
                int randIndex = Random.Range(0, upgradePool.Count);
                _currentChoices.Add(upgradePool[randIndex]);
                upgradePool.RemoveAt(randIndex);
            }
            
            if (_currentChoices.Count > 0)
            {
                var leftChoiceText = GetUpgradeDescription(_currentChoices[0]);
                var rightChoiceText = _currentChoices.Count > 1 ? GetUpgradeDescription(_currentChoices[1]) : "No other upgrades available";
                
                _renderer.RenderChoices(leftChoiceText, rightChoiceText, OnChoicesRendered);
            }
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
            
            _renderer.UnrenderChoices(() => GameEvents.GameResumed());
        }

        private void ApplyUpgrade(UpgradeType upgrade)
        {
            switch (upgrade)
            {
                case UpgradeType.ExtraLife:
                    var healthIncrease = _health.maxHealth * 0.25f;
                    _health.maxHealth += healthIncrease;
                    _health.currentHealth += healthIncrease;
                    _availableUpgrades.Remove(UpgradeType.ExtraLife); // Can only be chosen once
                    break;
                case UpgradeType.ReplenishLife:
                    _health.AddLife(_health.maxHealth); // Heals to full
                    break;
                case UpgradeType.ExtraShot:
                    _stats.NumberOfShots++;
                    break;
                case UpgradeType.ShotSpeed:
                    _stats.ProjectileSpeed *= 1.1f;
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
                default: return "Unknown Upgrade";
            }
        }

        public void ResetStats()
        {
            _stats.NumberOfShots = _initialNumberOfShots;
            _stats.ProjectileSpeed = _initialProjectileSpeed;
            _stats.ScoreThresholdUpgrade = _initialScoreThreshold;
            _stats.Health = _initialMaxHealth;
            
            GameEvents.OnUpdateScore -= OnScoreUpdated;
            GameEvents.OnGameFinished -= ResetStats;
            _inputSystem.Player.ChooseLeft.performed -= OnChooseLeft;
            _inputSystem.Player.ChooseRight.performed -= OnChooseRight;
        }
    }
} 