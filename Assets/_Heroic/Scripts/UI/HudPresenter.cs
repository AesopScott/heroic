using Heroic.Core;
using Heroic.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroic.UI
{
    public class HudPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private RunManager runManager;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider experienceSlider;
        [SerializeField] private Image healthFillImage;
        [SerializeField] private Image experienceFillImage;
        [SerializeField] private RectTransform healthFillRect;
        [SerializeField] private RectTransform experienceFillRect;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text experienceText;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void OnEnable()
        {
            if (playerExperience != null)
            {
                playerExperience.ExperienceChanged += HandleExperienceChanged;
                playerExperience.LevelChanged += HandleLevelChanged;
            }

            if (runManager != null)
            {
                runManager.RunTimeChanged += HandleRunTimeChanged;
            }

            RefreshAll();
        }

        private void OnDisable()
        {
            if (playerExperience != null)
            {
                playerExperience.ExperienceChanged -= HandleExperienceChanged;
                playerExperience.LevelChanged -= HandleLevelChanged;
            }

            if (runManager != null)
            {
                runManager.RunTimeChanged -= HandleRunTimeChanged;
            }
        }

        private void Update()
        {
            RefreshHealth();
        }

        private void RefreshAll()
        {
            RefreshHealth();

            if (playerExperience != null)
            {
                HandleExperienceChanged(playerExperience.CurrentExperience, playerExperience.ExperienceToNextLevel);
                HandleLevelChanged(playerExperience.Level);
            }
        }

        private void RefreshHealth()
        {
            if (playerHealth == null)
            {
                return;
            }

            if (healthSlider != null)
            {
                healthSlider.maxValue = playerHealth.MaxHealth;
                healthSlider.value = playerHealth.CurrentHealth;
            }

            if (healthFillImage != null)
            {
                ApplyBarFill(healthFillRect, healthFillImage, playerHealth.MaxHealth > 0 ? Mathf.Clamp01(playerHealth.CurrentHealth / (float)playerHealth.MaxHealth) : 0f);
            }

            if (healthText != null)
            {
                healthText.text = $"HP {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
            }
        }

        private void HandleExperienceChanged(int current, int required)
        {
            if (experienceSlider != null)
            {
                experienceSlider.maxValue = required;
                experienceSlider.value = current;
            }

            if (experienceFillImage != null)
            {
                ApplyBarFill(experienceFillRect, experienceFillImage, required > 0 ? Mathf.Clamp01(current / (float)required) : 0f);
            }

            if (experienceText != null)
            {
                experienceText.text = $"XP {current}/{required}";
            }
        }

        private void HandleLevelChanged(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"Level {level}";
            }
        }

        private void HandleRunTimeChanged(float seconds)
        {
            if (timerText == null)
            {
                return;
            }

            int minutes = Mathf.FloorToInt(seconds / 60f);
            int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
            timerText.text = $"{minutes:00}:{remainingSeconds:00}";
        }

        private static void ApplyBarFill(RectTransform fillRect, Image fillImage, float percent)
        {
            if (fillRect == null)
            {
                fillImage.fillAmount = percent;
                return;
            }

            float fullWidth = fillRect.sizeDelta.x;
            if (fillRect.parent is RectTransform parentRect)
            {
                fullWidth = parentRect.rect.width > 0f ? parentRect.rect.width : parentRect.sizeDelta.x;
            }

            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullWidth * percent);
            fillImage.fillAmount = 1f;
        }
    }
}
