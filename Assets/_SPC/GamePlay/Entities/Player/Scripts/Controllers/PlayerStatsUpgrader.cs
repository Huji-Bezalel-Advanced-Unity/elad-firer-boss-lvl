using System;
using System.Collections.Generic;
using System.Linq;
using _SPC.Core.BaseScripts.InputSystem.Scripts;
using _SPC.Core.BaseScripts.Managers;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Entities.Player
{
    /// <summary>
    /// Handles player stat upgrades, including upgrade selection, application, and stat reset functionality.
    /// </summary>
    public class PlayerStatsUpgrader : SPCStatsUpgrader
    {
        /// <summary>
        /// Available upgrade types for the player.
        /// </summary>
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

        private List<UpgradeType> _availableUpgrades;
        private readonly List<UpgradeType> _currentChoices = new List<UpgradeType>();

        // Fields to store initial stat values for resetting
        private int _initialNumberOfShots;
        private float _initialProjectileSpeed;
        private int _initialScoreThreshold;
        private float _initialMaxHealth;
        private float _initialAcceleration;
        private float _initialScoreThresholdMultiplier;

        /// <summary>
        /// Initializes the player stats upgrader with dependencies.
        /// </summary>
        /// <param name="stats">Player statistics to upgrade.</param>
        /// <param name="health">Player health system.</param>
        /// <param name="renderer">UI renderer for upgrade choices.</param>
        public PlayerStatsUpgrader(PlayerStats stats, SPCHealth health, PlayerUpgradeChooseRenderer renderer)
        {
            _stats = stats;
            _health = health;
            _renderer = renderer;
            _inputSystem = InputSystemBuffer.Instance.InputSystem;

            StoreInitialValues();
            InitializeUpgradeSystem();
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Handles score updates and triggers upgrades when threshold is reached.
        /// </summary>
        /// <param name="newScore">The new score value.</param>
        private void OnScoreUpdated(long newScore)
        {
            if (newScore >= _stats.ScoreThresholdUpgrade)
            {
                UpdateScoreThreshold();
                PauseAndShowUpgrades();
            }
        }

        /// <summary>
        /// Pauses the game and displays upgrade choices to the player.
        /// </summary>
        private void PauseAndShowUpgrades()
        {
            GameEvents.GamePaused();
            
            _currentChoices.Clear();
            var upgradePool = new List<UpgradeType>(_availableUpgrades);
            
            SelectRandomUpgrades(upgradePool);
            RenderUpgradeChoices();
        }

        /// <summary>
        /// Selects random upgrades from the available pool.
        /// </summary>
        /// <param name="upgradePool">Pool of available upgrades.</param>
        private void SelectRandomUpgrades(List<UpgradeType> upgradePool)
        {
            for (int i = 0; i < 2; i++)
            {
                if (upgradePool.Count == 0) break;

                var chosenUpgrade = GetWeightedRandomUpgrade(upgradePool);
                _currentChoices.Add(chosenUpgrade);
                upgradePool.Remove(chosenUpgrade);
            }
        }

        /// <summary>
        /// Renders the upgrade choices in the UI.
        /// </summary>
        private void RenderUpgradeChoices()
        {
            if (_currentChoices.Count > 0)
            {
                var leftChoiceText = GetUpgradeDescription(_currentChoices[0]);
                var rightChoiceText = _currentChoices.Count > 1 ? GetUpgradeDescription(_currentChoices[1]) : "No other upgrades available";
                
                _renderer.RenderChoices(leftChoiceText, rightChoiceText, OnChoicesRendered);
            }
        }

        /// <summary>
        /// Gets a weighted random upgrade from the available upgrades.
        /// </summary>
        /// <param name="availableUpgrades">List of available upgrades.</param>
        /// <returns>The selected upgrade type.</returns>
        private UpgradeType GetWeightedRandomUpgrade(List<UpgradeType> availableUpgrades)
        {
            bool isHealthLow = IsHealthLow();
            var weightedUpgrades = CalculateUpgradeWeights(availableUpgrades, isHealthLow);
            
            return SelectUpgradeByWeight(weightedUpgrades, availableUpgrades);
        }

        /// <summary>
        /// Checks if the player's health is low (below 50%).
        /// </summary>
        /// <returns>True if health is low, false otherwise.</returns>
        private bool IsHealthLow()
        {
            return _health.currentHealth / _health.maxHealth < 0.5f;
        }

        /// <summary>
        /// Calculates weights for each available upgrade.
        /// </summary>
        /// <param name="availableUpgrades">List of available upgrades.</param>
        /// <param name="isHealthLow">Whether the player's health is low.</param>
        /// <returns>Dictionary mapping upgrades to their weights.</returns>
        private Dictionary<UpgradeType, int> CalculateUpgradeWeights(List<UpgradeType> availableUpgrades, bool isHealthLow)
        {
            var weightedUpgrades = new Dictionary<UpgradeType, int>();
            
            foreach (var upgrade in availableUpgrades)
            {
                int weight = 100;
                
                if (isHealthLow && IsHealthUpgrade(upgrade))
                {
                    weight += 10;
                }

                if (upgrade == UpgradeType.ExtraShot)
                {
                    weight -= 20; // Downgrade because it's very powerful
                }
                
                if (PlayerUpgradeCounts.ContainsKey(upgrade))
                {
                    weight -= 10 * PlayerUpgradeCounts[upgrade];
                } 
                weight = Math.Clamp(weight, 0, int.MaxValue);
                weightedUpgrades.Add(upgrade, weight);
            }
            
            return weightedUpgrades;
        }

        /// <summary>
        /// Checks if an upgrade is a health-related upgrade.
        /// </summary>
        /// <param name="upgrade">The upgrade to check.</param>
        /// <returns>True if it's a health upgrade, false otherwise.</returns>
        private bool IsHealthUpgrade(UpgradeType upgrade)
        {
            return upgrade == UpgradeType.ExtraLife || upgrade == UpgradeType.ReplenishLife;
        }

        /// <summary>
        /// Selects an upgrade based on calculated weights.
        /// </summary>
        /// <param name="weightedUpgrades">Dictionary of upgrades and their weights.</param>
        /// <param name="availableUpgrades">List of available upgrades for fallback.</param>
        /// <returns>The selected upgrade type.</returns>
        private UpgradeType SelectUpgradeByWeight(Dictionary<UpgradeType, int> weightedUpgrades, List<UpgradeType> availableUpgrades)
        {
            int totalWeight = weightedUpgrades.Values.Sum();
            int randomValue = Random.Range(0, totalWeight);

            foreach (var item in weightedUpgrades)
            {
                if (randomValue < item.Value)
                {
                    return item.Key;
                }
                randomValue -= item.Value;
            }

            return availableUpgrades.Last(); // Fallback
        }

        /// <summary>
        /// Handles the completion of choice rendering by subscribing to input events.
        /// </summary>
        private void OnChoicesRendered()
        {
            SubscribeToChoiceInput();
        }

        /// <summary>
        /// Handles left choice input.
        /// </summary>
        /// <param name="context">Input action callback context.</param>
        private void OnChooseLeft(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            HandleChoice(0);
        }

        /// <summary>
        /// Handles right choice input.
        /// </summary>
        /// <param name="context">Input action callback context.</param>
        private void OnChooseRight(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            HandleChoice(1);
        }

        /// <summary>
        /// Handles the player's choice selection.
        /// </summary>
        /// <param name="choiceIndex">Index of the chosen upgrade (0 or 1).</param>
        private void HandleChoice(int choiceIndex)
        {
            UnsubscribeFromChoiceInput();

            if (choiceIndex < _currentChoices.Count)
            {
                ApplyUpgrade(_currentChoices[choiceIndex]);
            }
            
            _renderer.UnrenderChoices(GameEvents.GameResumed);
        }

        /// <summary>
        /// Applies the selected upgrade to the player's stats.
        /// </summary>
        /// <param name="upgrade">The upgrade type to apply.</param>
        private void ApplyUpgrade(UpgradeType upgrade)
        {
            UpdateUpgradeCount(upgrade);
            
            switch (upgrade)
            {
                case UpgradeType.ExtraLife:
                    ApplyExtraLifeUpgrade();
                    break;
                case UpgradeType.ReplenishLife:
                    ApplyReplenishLifeUpgrade();
                    break;
                case UpgradeType.ExtraShot:
                    ApplyExtraShotUpgrade();
                    break;
                case UpgradeType.ShotSpeed:
                    ApplyShotSpeedUpgrade();
                    break;
                case UpgradeType.IncreaseAcceleration:
                    ApplyAccelerationUpgrade();
                    break;
                case UpgradeType.DecreaseScoreThresholdMultiplier:
                    ApplyScoreThresholdUpgrade();
                    break;
            }
        }

        /// <summary>
        /// Updates the count for the specified upgrade type.
        /// </summary>
        /// <param name="upgrade">The upgrade type to count.</param>
        private void UpdateUpgradeCount(UpgradeType upgrade)
        {
            PlayerUpgradeCounts.TryAdd(upgrade, 0);
            PlayerUpgradeCounts[upgrade]++;
        }

        /// <summary>
        /// Applies the extra life upgrade.
        /// </summary>
        private void ApplyExtraLifeUpgrade()
        {
            _health.UpdateLife(1.25f);
        }

        /// <summary>
        /// Applies the replenish life upgrade.
        /// </summary>
        private void ApplyReplenishLifeUpgrade()
        {
            _health.AddLife(_health.maxHealth);
        }

        /// <summary>
        /// Applies the extra shot upgrade.
        /// </summary>
        private void ApplyExtraShotUpgrade()
        {
            _stats.NumberOfShots++;
        }

        /// <summary>
        /// Applies the shot speed upgrade.
        /// </summary>
        private void ApplyShotSpeedUpgrade()
        {
            _stats.ProjectileSpeed *= 1.1f;
        }

        /// <summary>
        /// Applies the acceleration upgrade.
        /// </summary>
        private void ApplyAccelerationUpgrade()
        {
            _stats.Acceleration *= 1.02f;
        }

        /// <summary>
        /// Applies the score threshold multiplier upgrade.
        /// </summary>
        private void ApplyScoreThresholdUpgrade()
        {
            _stats.ScoreThresholdMultiplier *= 0.9f;
        }
        
        /// <summary>
        /// Gets the description text for a specific upgrade type.
        /// </summary>
        /// <param name="upgrade">The upgrade type to get description for.</param>
        /// <returns>The description text for the upgrade.</returns>
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

        /// <summary>
        /// Resets all player stats to their initial values and unsubscribes from events.
        /// </summary>
        public override void ResetStats()
        {
            RestoreInitialValues();
            UnsubscribeFromAllEvents();
        }

        /// <summary>
        /// Stores the initial values of all stats for later reset.
        /// </summary>
        private void StoreInitialValues()
        {
            _initialNumberOfShots = _stats.NumberOfShots;
            _initialProjectileSpeed = _stats.ProjectileSpeed;
            _initialScoreThreshold = _stats.ScoreThresholdUpgrade;
            _initialMaxHealth = _stats.Health;
            _initialAcceleration = _stats.Acceleration;
            _initialScoreThresholdMultiplier = _stats.ScoreThresholdMultiplier;
        }

        /// <summary>
        /// Initializes the upgrade system with available upgrades.
        /// </summary>
        private void InitializeUpgradeSystem()
        {
            _availableUpgrades = new List<UpgradeType>((UpgradeType[])Enum.GetValues(typeof(UpgradeType)));
        }

        /// <summary>
        /// Subscribes to game events for score updates and game state changes.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            GameEvents.OnUpdateScore += OnScoreUpdated;
            GameEvents.OnGameFinished += ResetStats;
            GameEvents.OnGameLoss += ResetStats;
        }

        /// <summary>
        /// Subscribes to input events for upgrade choice selection.
        /// </summary>
        private void SubscribeToChoiceInput()
        {
            _inputSystem.Player.ChooseLeft.performed += OnChooseLeft;
            _inputSystem.Player.ChooseRight.performed += OnChooseRight;
        }

        /// <summary>
        /// Unsubscribes from input events for upgrade choice selection.
        /// </summary>
        private void UnsubscribeFromChoiceInput()
        {
            _inputSystem.Player.ChooseLeft.performed -= OnChooseLeft;
            _inputSystem.Player.ChooseRight.performed -= OnChooseRight;
        }

        /// <summary>
        /// Updates the score threshold for the next upgrade.
        /// </summary>
        private void UpdateScoreThreshold()
        {
            _stats.ScoreThresholdUpgrade = (int)(_stats.ScoreThresholdUpgrade * _stats.ScoreThresholdMultiplier);
        }

        /// <summary>
        /// Restores all stats to their initial values.
        /// </summary>
        private void RestoreInitialValues()
        {
            _stats.NumberOfShots = _initialNumberOfShots;
            _stats.ProjectileSpeed = _initialProjectileSpeed;
            _stats.ScoreThresholdUpgrade = _initialScoreThreshold;
            _stats.Health = _initialMaxHealth;
            _stats.Acceleration = _initialAcceleration;
            _stats.ScoreThresholdMultiplier = _initialScoreThresholdMultiplier;
        }

        /// <summary>
        /// Unsubscribes from all game and input events.
        /// </summary>
        private void UnsubscribeFromAllEvents()
        {
            GameEvents.OnUpdateScore -= OnScoreUpdated;
            GameEvents.OnGameFinished -= ResetStats;
            GameEvents.OnGameLoss -= ResetStats;
            UnsubscribeFromChoiceInput();
        }
    }
} 