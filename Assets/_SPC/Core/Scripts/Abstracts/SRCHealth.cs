using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _SPC.Core.Scripts.Abstracts
{
    public class SRCHealth
    {
        public Action<float, float> OnDamageAction; 
        public Action<float, float> OnHealAction;
        public Action OnDeathAction;
            
        public float maxHealth = 100;
        public float currentHealth = 100;
        
        public Dictionary<float, List<SRCHealthAction>> OnHPReached = new();

        public void ReduceLife(float amount)
        {
            currentHealth -= amount;
            
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

        public void AddHPAction(float amount, SRCHealthAction action)
        {
            if (OnHPReached.ContainsKey(amount))
            {
                OnHPReached[amount].Add(action);
            }
            else
            {
                OnHPReached.Add(amount, new List<SRCHealthAction> { action });
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

    public struct SRCHealthAction : IEquatable<SRCHealthAction>
    {
        public SRCHealthAction(Action action, bool isOnce = true)
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

        public bool Equals(SRCHealthAction other)
        {
            return Equals(OnDamageAction, other.OnDamageAction) && IsOnce == other.IsOnce;
        }

        public override bool Equals(object obj)
        {
            return obj is SRCHealthAction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OnDamageAction, IsOnce);
        }
    }
}