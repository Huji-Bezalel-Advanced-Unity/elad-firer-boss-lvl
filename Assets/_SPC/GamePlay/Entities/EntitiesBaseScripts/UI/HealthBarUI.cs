using _SPC.Core.BaseScripts.BaseMono;
using UnityEngine;
using UnityEngine.UI;

namespace _SPC.GamePlay.Entities
{
    /// <summary>
    /// UI component for displaying health bars with visual feedback for damage and healing.
    /// </summary>
    public class HealthBarUI : SPCBaseMono, IHealthUI
    {
        [Tooltip("Slider component for the health bar.")]
        [SerializeField] private Slider healthSlider;
        
        [Tooltip("Image component for the health bar fill.")]
        [SerializeField] private Image fillImage;
        
        [Tooltip("Color of the health bar fill.")]
        [SerializeField] private Color healthColor = Color.green;

        /// <summary>
        /// Initializes the health bar UI components.
        /// </summary>
        private void Awake()
        {
            InitializeFillImage();
            SetHealthBarColor();
        }

        /// <summary>
        /// Assigns the health system to this UI component and sets up event subscriptions.
        /// </summary>
        /// <param name="health">The health system to assign.</param>
        public void AssignEvent(SPCHealth health)
        {
            if (!ValidateHealthAssignment(health))
            {
                return;
            }

            UpdateHealthBarStats(health.maxHealth,health.currentHealth);
            SubscribeToHealthEvents(health);
        }

        /// <summary>
        /// Initializes the fill image component if not already assigned.
        /// </summary>
        private void InitializeFillImage()
        {
            if (healthSlider != null && fillImage == null)
            {
                fillImage = healthSlider.fillRect.GetComponent<Image>();
            }
        }

        /// <summary>
        /// Sets the color of the health bar fill.
        /// </summary>
        private void SetHealthBarColor()
        {
            if (fillImage != null)
            {
                fillImage.color = healthColor;
            }
        }

        /// <summary>
        /// Validates that the health assignment is valid.
        /// </summary>
        /// <param name="health">Health system to validate.</param>
        /// <returns>True if the assignment is valid, false otherwise.</returns>
        private bool ValidateHealthAssignment(SPCHealth health)
        {
            return health != null && healthSlider != null;
        }

        /// <summary>
        /// Updates the health bar with the health system's values.
        /// </summary>
        /// <param name="maxHealth">Max health of entity.</param>
        /// <param name="currentHealth">Current health of entity.</param>
        private void UpdateHealthBarStats(float maxHealth, float currentHealth)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        /// <summary>
        /// Subscribes to health system events for damage and healing.
        /// </summary>
        /// <param name="health">Health system to subscribe to.</param>
        private void SubscribeToHealthEvents(SPCHealth health)
        {
            health.OnDamageAction += UpdateHealthBarOnDamage;
            health.OnHealAction += UpdateHealthBarOnHeal;
            health.OnLifeStatsChangedAction += UpdateHealthBarStats;
        }

        /// <summary>
        /// Updates the health bar when damage is taken.
        /// </summary>
        /// <param name="amount">Amount of damage taken.</param>
        /// <param name="current">Current health value.</param>
        private void UpdateHealthBarOnDamage(float amount, float current)
        {
            healthSlider.value = current;
        }

        /// <summary>
        /// Updates the health bar when health is restored.
        /// </summary>
        /// <param name="amount">Amount of health restored.</param>
        /// <param name="current">Current health value.</param>
        private void UpdateHealthBarOnHeal(float amount, float current)
        {
            healthSlider.value = current;
        }
    }
} 