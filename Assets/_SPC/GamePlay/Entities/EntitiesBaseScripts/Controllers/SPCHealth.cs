using System;
using System.Collections.Generic;
using System.Linq;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Entities
{
    public interface IHealthUI
    {
        void AssignEvent(SPCHealth health);
    }

    public struct HealthDependencies
    {
        public GameLogger logger;
        public IHealthUI healthUI;
        public Dictionary<float, List<SPCHealthAction>> OnHPReached;
        public Action OnDeathAction;
        public float maxHP;
        public float currentHP;
        public List<Action<float, float>> OnDamageActions;
        

        public HealthDependencies(GameLogger logger, IHealthUI healthUI,
            Dictionary<float, List<SPCHealthAction>> onHPReached, Action onDeathAction, float maxHP, float currentHP , List<Action<float, float>> onDamageActions)
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
    
    // Based on Health class from Health Classroom, modified for personal use
    public class SPCHealth
    {
        public Action<float, float> OnDamageAction;
        public Action<float, float> OnHealAction;
        public Action OnDeathAction;

        public float maxHealth;
        public float currentHealth;

        public readonly Dictionary<float, List<SPCHealthAction>> OnHPReached = new();

        protected GameLogger logger;

        public SPCHealth(HealthDependencies dependencies)
        {
            logger = dependencies.logger;
            if (dependencies.OnHPReached != null) OnHPReached = dependencies.OnHPReached;
            if (dependencies.OnDeathAction != null) OnDeathAction = dependencies.OnDeathAction;
            if (dependencies.OnDamageActions != null)
            {
                foreach (var action in dependencies.OnDamageActions) OnDamageAction += action;
            }
            maxHealth = dependencies.maxHP;
            currentHealth = dependencies.currentHP;
            dependencies.healthUI?.AssignEvent(this);
        }

        public void ReduceLife(int amount)
        {
            if(currentHealth <= 0) return;
            currentHealth -= amount;
            logger?.Log("Current Heath is " + currentHealth);
            OnDamageAction?.Invoke(amount, currentHealth);

            CheckDeath();

            TryInvokeReachedAction();
        }

        private void CheckDeath()
        {
            if (currentHealth <= 0)
            {
                OnDeathAction?.Invoke();
            }
        }

        private void TryInvokeReachedAction()
        {
            var keys = OnHPReached.Keys.ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                var hp = keys[i];

                if (currentHealth <= hp)
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

                    if (!OnHPReached[hp].Any())
                    {
                        OnHPReached.Remove(hp);
                    }
                }
            }
        }

        public void AddLife(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            OnHealAction?.Invoke(amount, currentHealth);
        }

        public float GetHP()
        {
            return currentHealth;
        }

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

        public void InitializeHealth(int i)
        {
            currentHealth = maxHealth = i;
        }

        public float GetPercentage()
        {
            var percentage = currentHealth / maxHealth;
            return percentage;
        }
    }
    
    public struct SPCHealthAction : IEquatable<SPCHealthAction>
    {
        public SPCHealthAction(Action action, bool isOnce = true)
        {
            OnHelathReachedAction = action;
            IsOnce = isOnce;
        }

        public Action OnHelathReachedAction;
        public bool IsOnce;

        public void Invoke()
        {
            OnHelathReachedAction?.Invoke();
        }

        public bool Equals(SPCHealthAction other)
        {
            return Equals(OnHelathReachedAction, other.OnHelathReachedAction) && IsOnce == other.IsOnce;
        }

        public override bool Equals(object obj)
        {
            return obj is SPCHealthAction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OnHelathReachedAction, IsOnce);
        }
    }
}