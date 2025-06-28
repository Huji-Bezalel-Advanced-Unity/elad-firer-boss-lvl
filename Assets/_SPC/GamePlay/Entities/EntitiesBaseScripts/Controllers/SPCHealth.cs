using System;
using System.Collections.Generic;
using System.Linq;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Entities
{
    /// <summary>
    /// Interface for health UI components that can be assigned to health systems.
    /// </summary>
    public interface IHealthUI
    {
        /// <summary>
        /// Assigns the health system to this UI component.
        /// </summary>
        /// <param name="health">The health system to assign.</param>
        void AssignEvent(SPCHealth health);
        
    }

    /// <summary>
    /// Holds dependencies for the SPCHealth system initialization.
    /// </summary>
    public struct HealthDependencies
    {
        public GameLogger logger;
        public IHealthUI healthUI;
        public Dictionary<float, List<SPCHealthAction>> OnHPReached;
        public Action OnDeathAction;
        public float maxHP;
        public float currentHP;
        public List<Action<float, float>> OnDamageActions;
        
        /// <summary>
        /// Initializes the health dependencies with all required parameters.
        /// </summary>
        public HealthDependencies(GameLogger logger, IHealthUI healthUI,
            Dictionary<float, List<SPCHealthAction>> onHPReached, Action onDeathAction, float maxHP, float currentHP, List<Action<float, float>> onDamageActions)
        {
            this.logger = logger;
            this.healthUI = healthUI;
            OnHPReached = onHPReached;
            OnDeathAction = onDeathAction;
            this.maxHP = maxHP;
            this.currentHP = currentHP;
            OnDamageActions = onDamageActions;
        }
    }
    
    /// <summary>
    /// Health system for entities, providing damage, healing, and health-based event triggers.
    /// Based on Health class from Health Classroom, modified for personal use.
    /// </summary>
    public class SPCHealth
    {
        public Action<float, float> OnDamageAction;
        public Action<float, float> OnHealAction;
        public Action<float, float> OnLifeStatsChangedAction;
        private Action OnDeathAction;

        public float maxHealth;
        public float currentHealth;

        private Dictionary<float, List<SPCHealthAction>> OnHPReached = new();

        private GameLogger logger;
        private HealthDependencies _deps;

        /// <summary>
        /// Initializes the health system with dependencies.
        /// </summary>
        /// <param name="dependencies">Dependencies required for health system initialization.</param>
        public SPCHealth(HealthDependencies dependencies)
        {
            InitializeHealthSystem(dependencies);
            AssignHealthUI(dependencies);
        }

        /// <summary>
        /// Initializes the health system with dependencies.
        /// </summary>
        /// <param name="dependencies">Dependencies to initialize from.</param>
        private void InitializeHealthSystem(HealthDependencies dependencies)
        {
            _deps = dependencies;
            logger = dependencies.logger;
            
            if (dependencies.OnHPReached != null) 
                OnHPReached = dependencies.OnHPReached;
                
            if (dependencies.OnDeathAction != null) 
                OnDeathAction = dependencies.OnDeathAction;
                
            if (dependencies.OnDamageActions != null)
            {
                foreach (var action in dependencies.OnDamageActions) 
                    OnDamageAction += action;
            }
            
            maxHealth = dependencies.maxHP;
            currentHealth = dependencies.currentHP;
        }
        
        /// <summary>
        /// Assigns the health UI component if provided.
        /// </summary>
        /// <param name="dependencies">Dependencies containing the health UI.</param>
        private void AssignHealthUI(HealthDependencies dependencies)
        {
            dependencies.healthUI?.AssignEvent(this);
        }
        
        /// <summary>
        /// Reduces the entity's health by the specified amount and triggers relevant events.
        /// </summary>
        /// <param name="amount">Amount of damage to apply.</param>
        public void ReduceLife(int amount)
        {
            if (currentHealth <= 0) return;
            
            ApplyDamage(amount);
            LogCurrentHealth();
            TriggerDamageEvents(amount);
            CheckDeath();
            TryInvokeReachedAction();
        }

        /// <summary>
        /// Adds health to the entity up to the maximum health.
        /// </summary>
        /// <param name="amount">Amount of health to add.</param>
        public void AddLife(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            OnHealAction?.Invoke(amount, currentHealth);
        }

        /// <summary>
        /// Gets the current health value.
        /// </summary>
        /// <returns>Current health value.</returns>
        public float GetHP()
        {
            return currentHealth;
        }

        /// <summary>
        /// Adds a health-based action that triggers when health reaches a specific threshold.
        /// </summary>
        /// <param name="amount">Health threshold to trigger the action.</param>
        /// <param name="action">Action to execute when threshold is reached.</param>
        public void AddHPAction(float amount, SPCHealthAction action)
        {
            if (OnHPReached.ContainsKey(amount))
            {
                OnHPReached[amount].Add(action);
            }
            else
            {
                OnHPReached.Add(amount, new List<SPCHealthAction> { action });
            }
        }

        /// <summary>
        /// Initializes health to a specific value.
        /// </summary>
        /// <param name="i">Health value to set.</param>
        public void InitializeHealth(int i)
        {
            currentHealth = maxHealth = i;
        }

        /// <summary>
        /// Gets the current health as a percentage of maximum health.
        /// </summary>
        /// <returns>Health percentage (0.0 to 1.0).</returns>
        public float GetPercentage()
        {
            var percentage = currentHealth / maxHealth;
            return percentage;
        }
        
        
        /// <summary>
        /// changes Life Stats.
        /// </summary>
        /// <param name="percentage">percentage to changed life stats.</param>
        public void UpdateLife(float percentage)
        {
            maxHealth *= percentage;
            currentHealth *= percentage;
            OnLifeStatsChangedAction.Invoke(maxHealth, currentHealth);
        }




        /// <summary>
        /// Applies damage to the current health, preventing it from going below 0.
        /// </summary>
        /// <param name="amount">Amount of damage to apply.</param>
        private void ApplyDamage(int amount)
        {
            currentHealth = Mathf.Max(currentHealth - amount, 0);
        }

        /// <summary>
        /// Logs the current health value.
        /// </summary>
        private void LogCurrentHealth()
        {
            logger?.Log("Current Health is " + currentHealth);
        }

        /// <summary>
        /// Triggers damage-related events.
        /// </summary>
        /// <param name="amount">Amount of damage applied.</param>
        private void TriggerDamageEvents(int amount)
        {
            OnDamageAction?.Invoke(amount, currentHealth);
        }

        /// <summary>
        /// Checks if the entity has died and triggers death event.
        /// </summary>
        private void CheckDeath()
        {
            if (currentHealth <= 0)
            {
                OnDeathAction?.Invoke();
            }
        }

        /// <summary>
        /// Tries to invoke health-based actions when health thresholds are reached.
        /// </summary>
        private void TryInvokeReachedAction()
        {
            var keys = OnHPReached.Keys.ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                var hp = keys[i];

                if (currentHealth <= hp)
                {
                    InvokeHealthActions(hp);
                    CleanupEmptyHealthActions(hp);
                }
            }
        }

        /// <summary>
        /// Invokes all actions for a specific health threshold.
        /// </summary>
        /// <param name="hp">Health threshold to check.</param>
        private void InvokeHealthActions(float hp)
        {
            for (var index = 0; index < OnHPReached[hp].Count; index++)
            {
                var action = OnHPReached[hp][index];
                action.Invoke();

                if (action.IsOnce)
                {
                    OnHPReached[hp].Remove(action);
                }
            }
        }

        /// <summary>
        /// Removes empty health action lists to prevent memory leaks.
        /// </summary>
        /// <param name="hp">Health threshold to cleanup.</param>
        private void CleanupEmptyHealthActions(float hp)
        {
            if (!OnHPReached[hp].Any())
            {
                OnHPReached.Remove(hp);
            }
        }
    }
    
    /// <summary>
    /// Represents a health-based action that can be triggered when health reaches a specific threshold.
    /// </summary>
    public struct SPCHealthAction : IEquatable<SPCHealthAction>
    {
        /// <summary>
        /// Initializes a new health action.
        /// </summary>
        /// <param name="action">Action to execute when health threshold is reached.</param>
        /// <param name="isOnce">Whether the action should only execute once.</param>
        public SPCHealthAction(Action action, bool isOnce = true)
        {
            OnHealthReachedAction = action;
            IsOnce = isOnce;
        }

        private Action OnHealthReachedAction;
        public readonly bool IsOnce;

        /// <summary>
        /// Invokes the health action.
        /// </summary>
        public void Invoke()
        {
            OnHealthReachedAction?.Invoke();
        }
        
        public bool Equals(SPCHealthAction other)
        {
            return Equals(OnHealthReachedAction, other.OnHealthReachedAction) && IsOnce == other.IsOnce;
        }
        
        public override bool Equals(object obj)
        {
            return obj is SPCHealthAction other && Equals(other);
        }

        
        public override int GetHashCode()
        {
            return HashCode.Combine(OnHealthReachedAction, IsOnce);
        }
    }
}