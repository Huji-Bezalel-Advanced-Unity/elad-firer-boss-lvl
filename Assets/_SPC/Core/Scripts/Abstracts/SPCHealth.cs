using System;
using System.Collections.Generic;
using System.Linq;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.Core.Scripts.Abstracts
{
    public class SPCHealth
    {
        public Action<float, float> OnDamageAction; 
        public Action<float, float> OnHealAction;
        public Action OnDeathAction;
            
        public float maxHealth = 100;
        public float currentHealth = 100;
        
        public Dictionary<float, List<SPCHealthAction>> OnHPReached = new();
        
        protected GameLogger logger;

        public void ReduceLife(int amount)
        {
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
            OnDamageAction = action;
            IsOnce = isOnce;
        }
        
        public Action OnDamageAction;
        public bool IsOnce;

        public void Invoke()
        {
            OnDamageAction?.Invoke();
        }

        public bool Equals(SPCHealthAction other)
        {
            return Equals(OnDamageAction, other.OnDamageAction) && IsOnce == other.IsOnce;
        }

        public override bool Equals(object obj)
        {
            return obj is SPCHealthAction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OnDamageAction, IsOnce);
        }
    }
}