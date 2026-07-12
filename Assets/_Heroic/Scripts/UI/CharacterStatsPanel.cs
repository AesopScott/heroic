using Heroic.Player;
using Heroic.Systems;
using System.Text;
using TMPro;
using UnityEngine;

namespace Heroic.UI
{
    public class CharacterStatsPanel : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private RunBuildState buildState;
        [SerializeField] private MovementCaster movementCaster;
        [SerializeField] private PlayerTemporaryBuffs temporaryBuffs;
        [SerializeField] private SpellStatModifier spellStats;
        [SerializeField] private TerritoryCastingController territoryCasting;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text experienceText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text skillListText;
        [SerializeField] private TMP_Text bonusListText;
        [SerializeField] private float refreshInterval = 0.2f;

        private readonly StringBuilder builder = new StringBuilder(768);
        private float nextRefreshTime;

        private void Awake()
        {
            playerHealth ??= FindAnyObjectByType<PlayerHealth>();
            playerExperience ??= FindAnyObjectByType<PlayerExperience>();
            buildState ??= FindAnyObjectByType<RunBuildState>();
            movementCaster ??= FindAnyObjectByType<MovementCaster>();
            temporaryBuffs ??= FindAnyObjectByType<PlayerTemporaryBuffs>();
            spellStats ??= FindAnyObjectByType<SpellStatModifier>();
            territoryCasting ??= FindAnyObjectByType<TerritoryCastingController>();
        }

        private void OnEnable()
        {
            if (playerExperience != null)
            {
                playerExperience.ExperienceChanged += HandleExperienceChanged;
                playerExperience.LevelChanged += HandleLevelChanged;
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
        }

        private void Update()
        {
            if (Time.unscaledTime < nextRefreshTime)
            {
                return;
            }

            nextRefreshTime = Time.unscaledTime + refreshInterval;
            RefreshAll();
        }

        private void RefreshAll()
        {
            RefreshCoreStats();
            RefreshSkills();
            RefreshBonuses();
        }

        private void RefreshCoreStats()
        {
            if (playerHealth != null && healthText != null)
            {
                healthText.text = $"Health: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
            }

            if (playerExperience != null && experienceText != null)
            {
                experienceText.text = $"Experience: {playerExperience.CurrentExperience}/{playerExperience.ExperienceToNextLevel}";
            }

            if (playerExperience != null && levelText != null)
            {
                levelText.text = $"Level: {playerExperience.Level}";
            }
        }

        private void RefreshSkills()
        {
            if (skillListText == null)
            {
                return;
            }

            builder.Clear();
            builder.AppendLine("Skills");
            AppendLearnedSkills("Abilities", false, false);
            AppendMovementSkills();
            AppendLearnedSkills("Systems", true, false);
            AppendUpgradeTiers();
            skillListText.text = builder.ToString();
        }

        private void AppendLearnedSkills(string header, bool systemsOnly, bool movementsOnly)
        {
            if (buildState == null)
            {
                return;
            }

            bool wroteHeader = false;
            foreach (string skillId in buildState.LearnedSkillIds)
            {
                bool isSystem = skillId.StartsWith("system_");
                bool isMovement = skillId.StartsWith("movement_");
                if (systemsOnly != isSystem || movementsOnly != isMovement || (!systemsOnly && !movementsOnly && (isSystem || isMovement)))
                {
                    continue;
                }

                if (!wroteHeader)
                {
                    builder.AppendLine(header + ":");
                    wroteHeader = true;
                }

                builder.Append("- ");
                builder.AppendLine(FormatId(skillId));
            }
        }

        private void AppendMovementSkills()
        {
            if (movementCaster == null)
            {
                return;
            }

            builder.AppendLine("Movement:");
            for (int i = 0; i < 3; i++)
            {
                MovementCaster.MovementSkillId skill = movementCaster.GetEquippedSkill(i);
                builder.Append(i + 1);
                builder.Append(". ");
                builder.AppendLine(skill == MovementCaster.MovementSkillId.None ? "-" : FormatCamelCase(skill.ToString()));
            }
        }

        private void AppendUpgradeTiers()
        {
            if (buildState == null || buildState.SkillUpgrades.Count == 0)
            {
                return;
            }

            bool wroteHeader = false;
            foreach (RunBuildState.SkillUpgradeState upgrade in buildState.SkillUpgrades)
            {
                if (upgrade == null || upgrade.Tier <= 0)
                {
                    continue;
                }

                if (!wroteHeader)
                {
                    builder.AppendLine("Boosts:");
                    wroteHeader = true;
                }

                builder.Append("- ");
                builder.Append(FormatUpgradeId(upgrade.UpgradePathId));
                builder.Append(" T");
                builder.AppendLine(upgrade.Tier.ToString());
            }
        }

        private void RefreshBonuses()
        {
            if (bonusListText == null)
            {
                return;
            }

            builder.Clear();
            builder.AppendLine("Current Bonuses");

            bool hasBonus = false;
            if (temporaryBuffs != null)
            {
                if (temporaryBuffs.HasActiveSpeedBoost)
                {
                    builder.Append("- Speed x");
                    builder.Append(temporaryBuffs.ActiveSpeedMultiplier.ToString("0.00"));
                    builder.Append(" for ");
                    builder.Append(temporaryBuffs.SpeedBoostRemaining.ToString("0.0"));
                    builder.AppendLine("s");
                    hasBonus = true;
                }

                if (temporaryBuffs.HasActiveExperienceBoost)
                {
                    builder.Append("- XP x");
                    builder.Append(temporaryBuffs.ActiveExperienceMultiplier.ToString("0.00"));
                    builder.Append(" for ");
                    builder.Append(temporaryBuffs.ExperienceBoostRemaining.ToString("0.0"));
                    builder.AppendLine("s");
                    hasBonus = true;
                }

                if (temporaryBuffs.HasActiveInvulnerability)
                {
                    builder.Append("- Invulnerable for ");
                    builder.Append(temporaryBuffs.InvulnerabilityRemaining.ToString("0.0"));
                    builder.AppendLine("s");
                    hasBonus = true;
                }
            }

            if (territoryCasting != null && territoryCasting.HasActiveTerritoryBonus)
            {
                builder.AppendLine(territoryCasting.ActiveBonusSummary);
                hasBonus = true;
            }

            if (!hasBonus && spellStats != null)
            {
                AppendMultiplierBonus("Damage", spellStats.DamageMultiplier, ref hasBonus);
                AppendMultiplierBonus("Range", spellStats.RangeMultiplier, ref hasBonus);
                AppendMultiplierBonus("Recovery", spellStats.RecoveryMultiplier, ref hasBonus);
            }

            if (!hasBonus)
            {
                builder.AppendLine("- None");
            }

            bonusListText.text = builder.ToString();
        }

        private void AppendMultiplierBonus(string label, float multiplier, ref bool hasBonus)
        {
            if (multiplier <= 1.01f)
            {
                return;
            }

            builder.Append("- ");
            builder.Append(label);
            builder.Append(" x");
            builder.AppendLine(multiplier.ToString("0.00"));
            hasBonus = true;
        }

        private void HandleExperienceChanged(int current, int required)
        {
            RefreshAll();
        }

        private void HandleLevelChanged(int level)
        {
            RefreshAll();
        }

        private static string FormatUpgradeId(string id)
        {
            if (id.StartsWith("upgrade_"))
            {
                id = id.Substring("upgrade_".Length);
            }

            return FormatId(id);
        }

        private static string FormatId(string id)
        {
            string[] parts = id.Split('_');
            builderScratch.Clear();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "arcane" || parts[i] == "fire" || parts[i] == "system" || parts[i] == "movement")
                {
                    continue;
                }

                if (builderScratch.Length > 0)
                {
                    builderScratch.Append(' ');
                }

                builderScratch.Append(ToTitleCase(parts[i]));
            }

            return builderScratch.Length == 0 ? id : builderScratch.ToString();
        }

        private static string FormatCamelCase(string text)
        {
            builderScratch.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && char.IsUpper(text[i]) && !char.IsWhiteSpace(text[i - 1]))
                {
                    builderScratch.Append(' ');
                }

                builderScratch.Append(text[i]);
            }

            return builderScratch.ToString();
        }

        private static string ToTitleCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return char.ToUpperInvariant(text[0]) + (text.Length > 1 ? text.Substring(1) : string.Empty);
        }

        private static readonly StringBuilder builderScratch = new StringBuilder(96);
    }
}
