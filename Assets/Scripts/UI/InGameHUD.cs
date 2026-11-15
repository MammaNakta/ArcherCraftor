using UnityEngine;
using UnityEngine.UI;
using ArcherCraftor.Combat;

namespace ArcherCraftor.UI
{
    public class InGameHUD : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Health playerHealth;

        [Header("Controls Guide Panel")]
        [SerializeField] private GameObject guidePanel;

        private void Start()
        {
            if (playerHealth == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerHealth = player.GetComponent<Health>();
            }

            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthBar;
                UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthBar;
            }
        }

        private void UpdateHealthBar(float current, float max)
        {
            if (healthSlider != null && max > 0f)
            {
                healthSlider.value = current / max;
            }
        }
    }
}
