using Heroic.Systems;
using UnityEngine;
using System;

namespace Heroic.Player
{
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField] private int level = 1;
        [SerializeField] private int currentExperience;
        [SerializeField] private int baseExperienceToLevel = 10;
        [SerializeField] private float thresholdGrowth = 1.35f;
        [SerializeField] private UpgradeManager upgradeManager;

        public event Action<int> LevelChanged;
        public event Action<int, int> ExperienceChanged;

        public int Level => level;
        public int CurrentExperience => currentExperience;
        public int ExperienceToNextLevel => CalculateExperienceThreshold(level);

        public void ConfigureLeveling(int newBaseExperienceToLevel, float newThresholdGrowth)
        {
            baseExperienceToLevel = Mathf.Max(1, newBaseExperienceToLevel);
            thresholdGrowth = Mathf.Max(1f, newThresholdGrowth);
            ExperienceChanged?.Invoke(currentExperience, ExperienceToNextLevel);
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            currentExperience += amount;

            while (currentExperience >= ExperienceToNextLevel)
            {
                currentExperience -= ExperienceToNextLevel;
                LevelUp();
            }

            ExperienceChanged?.Invoke(currentExperience, ExperienceToNextLevel);
        }

        private void LevelUp()
        {
            level++;
            LevelChanged?.Invoke(level);

            if (upgradeManager != null)
            {
                bool includeMovementChoice = level % 2 == 0;
                upgradeManager.OpenDraft(level, includeMovementChoice);
            }
        }

        private int CalculateExperienceThreshold(int targetLevel)
        {
            return Mathf.Max(1, Mathf.RoundToInt(baseExperienceToLevel * Mathf.Pow(thresholdGrowth, targetLevel - 1)));
        }
    }
}
