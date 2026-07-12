using Heroic.Core;
using Heroic.Enemies;
using Heroic.Player;
using TMPro;
using UnityEngine;

namespace Heroic.UI
{
    public class ObjectivePresenter : MonoBehaviour
    {
        [SerializeField] private RunManager runManager;
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private BossSpawner bossSpawner;
        [SerializeField] private TMP_Text goalText;
        [SerializeField] private TMP_Text bossText;
        [SerializeField] private TMP_Text upgradeText;

        private void Awake()
        {
            if (runManager == null)
            {
                runManager = FindAnyObjectByType<RunManager>();
            }
        }

        private void OnEnable()
        {
            if (runManager != null)
            {
                runManager.RunTimeChanged += HandleRunTimeChanged;
                runManager.StateChanged += HandleStateChanged;
            }

            if (playerExperience != null)
            {
                playerExperience.ExperienceChanged += HandleExperienceChanged;
                playerExperience.LevelChanged += HandleLevelChanged;
            }

            RefreshAll();
        }

        private void OnDisable()
        {
            if (runManager != null)
            {
                runManager.RunTimeChanged -= HandleRunTimeChanged;
                runManager.StateChanged -= HandleStateChanged;
            }

            if (playerExperience != null)
            {
                playerExperience.ExperienceChanged -= HandleExperienceChanged;
                playerExperience.LevelChanged -= HandleLevelChanged;
            }
        }

        private void RefreshAll()
        {
            RefreshGoal();
            RefreshBoss();
            RefreshUpgrade();
        }

        private void RefreshGoal()
        {
            if (goalText != null)
            {
                goalText.text = "DEMO GOAL\nSurvive. Build the spellbook.\nKill the Arcane Warden.";
            }
        }

        private void RefreshBoss()
        {
            if (bossText == null || bossSpawner == null || runManager == null)
            {
                return;
            }

            if (bossSpawner.HasSpawned)
            {
                bossText.text = "Boss active: burn it down.";
                return;
            }

            float seconds = Mathf.Max(0f, bossSpawner.SpawnAtSeconds - runManager.RunTimeSeconds);
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
            bossText.text = $"Boss in {minutes:00}:{remainingSeconds:00}";
        }

        private void RefreshUpgrade()
        {
            if (upgradeText == null || playerExperience == null)
            {
                return;
            }

            upgradeText.text = $"Next draft: {playerExperience.CurrentExperience}/{playerExperience.ExperienceToNextLevel} XP";
        }

        private void HandleRunTimeChanged(float seconds)
        {
            RefreshBoss();
        }

        private void HandleStateChanged(RunManager.RunState state)
        {
            RefreshAll();
        }

        private void HandleExperienceChanged(int current, int required)
        {
            RefreshUpgrade();
        }

        private void HandleLevelChanged(int level)
        {
            RefreshUpgrade();
        }
    }
}
