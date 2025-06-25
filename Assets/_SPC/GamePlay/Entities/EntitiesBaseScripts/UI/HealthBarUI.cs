using _SPC.Core.BaseScripts.BaseMono;
using UnityEngine;
using UnityEngine.UI;

namespace _SPC.GamePlay.Entities
{
    public class HealthBarUI : SPCBaseMono, IHealthUI
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image fillImage;
        [SerializeField] private Color healthColor = Color.green;

        private void Awake()
        {
            if (healthSlider != null && fillImage == null)
            {
                fillImage = healthSlider.fillRect.GetComponent<Image>();
            }
            if (fillImage != null)
                fillImage.color = healthColor;
        }

        public void AssignEvent(SPCHealth health)
        {
            if (health == null || healthSlider == null) return;
            healthSlider.maxValue = health.maxHealth;
            healthSlider.value = health.currentHealth;

            health.OnDamageAction += (amount, current) =>
            {
                healthSlider.value = current;
            };
            health.OnHealAction += (amount, current) =>
            {
                healthSlider.value = current;
            };
        }


    }
} 