using Heroic.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Heroic.UI
{
    public class DraftPresenter : MonoBehaviour
    {
        [SerializeField] private UpgradeManager upgradeManager;
        [SerializeField] private RunBuildState buildState;
        [SerializeField] private Button[] choiceButtons = new Button[0];
        [SerializeField] private TMP_Text[] choiceLabels = new TMP_Text[0];
        [SerializeField] private Image[] choiceBars = new Image[0];
        [SerializeField] private Image[] categoryIconBackdrops = new Image[0];
        [SerializeField] private TMP_Text[] categoryIconLabels = new TMP_Text[0];
        [SerializeField] private Image[] skillIconBackdrops = new Image[0];
        [SerializeField] private TMP_Text[] skillIconLabels = new TMP_Text[0];
        [SerializeField] private TMP_Text headerText;

        private IReadOnlyList<UpgradeManager.DraftChoice> currentChoices;

        private void Awake()
        {
            if (upgradeManager == null)
            {
                upgradeManager = FindAnyObjectByType<UpgradeManager>();
            }

            if (buildState == null)
            {
                buildState = FindAnyObjectByType<RunBuildState>();
            }
        }

        private void OnEnable()
        {
            if (upgradeManager != null)
            {
                upgradeManager.DraftOpened += HandleDraftOpened;
                upgradeManager.DraftClosed += HandleDraftClosed;
            }
        }

        private void OnDisable()
        {
            if (upgradeManager != null)
            {
                upgradeManager.DraftOpened -= HandleDraftOpened;
                upgradeManager.DraftClosed -= HandleDraftClosed;
            }
        }

        private void HandleDraftOpened(IReadOnlyList<UpgradeManager.DraftChoice> choices, bool includesMovement)
        {
            currentChoices = choices;

            if (headerText != null)
            {
                headerText.text = includesMovement ? "Choose an upgrade or movement" : "Choose an upgrade";
            }

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (choiceButtons[i] == null)
                {
                    continue;
                }

                bool hasChoice = choices != null && i < choices.Count;
                choiceButtons[i].gameObject.SetActive(hasChoice);

                if (!hasChoice)
                {
                    continue;
                }

                int capturedIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => SelectChoice(capturedIndex));

                if (i < choiceLabels.Length && choiceLabels[i] != null)
                {
                    choiceLabels[i].text = FormatChoiceLabel(choices[i]);
                }

                ApplyChoiceVisuals(i, choices[i]);
            }
        }

        private void HandleDraftClosed()
        {
            currentChoices = null;
        }

        private void SelectChoice(int index)
        {
            if (upgradeManager == null || currentChoices == null || index < 0 || index >= currentChoices.Count)
            {
                return;
            }

            upgradeManager.ApplyChoice(currentChoices[index].Id);
        }

        private static string FormatChoiceLabel(UpgradeManager.DraftChoice choice)
        {
            string category = choice.Category.ToString().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(choice.Description))
            {
                return $"<size=82%><color=#87C8FF>{category}</color></size>\n<b>{choice.DisplayName}</b>";
            }

            return $"<size=82%><color=#87C8FF>{category}</color></size>\n<b>{choice.DisplayName}</b>\n<size=86%><color=#C7E6F5>{choice.Description}</color></size>";
        }

        private void ApplyChoiceVisuals(int index, UpgradeManager.DraftChoice choice)
        {
            Color barColor = ResolveBarColor(choice);
            Color tierColor = ResolveTierColor(choice);

            if (index < choiceBars.Length && choiceBars[index] != null)
            {
                choiceBars[index].color = barColor;
            }

            if (index < categoryIconBackdrops.Length && categoryIconBackdrops[index] != null)
            {
                categoryIconBackdrops[index].color = Darken(barColor, 0.42f);
            }

            if (index < categoryIconLabels.Length && categoryIconLabels[index] != null)
            {
                categoryIconLabels[index].text = ResolveCategoryIcon(choice.Category);
            }

            if (index < skillIconBackdrops.Length && skillIconBackdrops[index] != null)
            {
                skillIconBackdrops[index].color = tierColor;
            }

            if (index < skillIconLabels.Length && skillIconLabels[index] != null)
            {
                skillIconLabels[index].text = ResolveSkillIcon(choice.Id);
                skillIconLabels[index].color = IsBright(tierColor) ? new Color(0.03f, 0.04f, 0.05f) : Color.white;
            }
        }

        private Color ResolveBarColor(UpgradeManager.DraftChoice choice)
        {
            string id = choice.Id.ToLowerInvariant();

            if (choice.Category == UpgradeManager.UpgradeCategory.Movement || id.StartsWith("movement_"))
            {
                return Hex("88F7B0");
            }

            if (choice.Category == UpgradeManager.UpgradeCategory.System || id.StartsWith("system_"))
            {
                return Hex("C8C3FF");
            }

            if (id.Contains("_fire_") || id.StartsWith("fire_"))
            {
                return Hex("FF6A2A");
            }

            if (id.Contains("_cold_") || id.StartsWith("cold_"))
            {
                return Hex("7FE7FF");
            }

            if (id.Contains("_lightning_") || id.StartsWith("lightning_"))
            {
                return Hex("F5E84B");
            }

            if (id.Contains("_earth_") || id.StartsWith("earth_"))
            {
                return Hex("A8743D");
            }

            if (id.Contains("_mind_") || id.StartsWith("mind_"))
            {
                return Hex("D889FF");
            }

            if (id.Contains("_blood_") || id.StartsWith("blood_"))
            {
                return Hex("C0263E");
            }

            if (id.Contains("_poison_") || id.StartsWith("poison_"))
            {
                return Hex("76D94E");
            }

            return Hex("78D7FF");
        }

        private Color ResolveTierColor(UpgradeManager.DraftChoice choice)
        {
            int tier = ResolveDisplayedTier(choice);
            switch (Mathf.Clamp(tier, 1, 5))
            {
                case 1:
                    return Hex("9BA3AA");
                case 2:
                    return Hex("54D36B");
                case 3:
                    return Hex("4FA3FF");
                case 4:
                    return Hex("B066FF");
                default:
                    return Hex("FFD45A");
            }
        }

        private int ResolveDisplayedTier(UpgradeManager.DraftChoice choice)
        {
            if (choice.Category != UpgradeManager.UpgradeCategory.Boost || buildState == null)
            {
                return 1;
            }

            string skillId = ResolveSkillId(choice.Id);
            int currentTier = buildState.GetSkillPathTier(skillId, choice.Id);
            return Mathf.Clamp(currentTier + 1, 1, 5);
        }

        private static string ResolveCategoryIcon(UpgradeManager.UpgradeCategory category)
        {
            switch (category)
            {
                case UpgradeManager.UpgradeCategory.Attack:
                    return "ATK";
                case UpgradeManager.UpgradeCategory.Defense:
                    return "DEF";
                case UpgradeManager.UpgradeCategory.Movement:
                    return "MOV";
                case UpgradeManager.UpgradeCategory.System:
                    return "SYS";
                default:
                    return "UP";
            }
        }

        private static string ResolveSkillIcon(string choiceId)
        {
            string id = choiceId.ToLowerInvariant();
            if (id.Contains("magic_missile"))
            {
                return "MM";
            }

            if (id.Contains("arcane_blast"))
            {
                return "AB";
            }

            if (id.Contains("warp_pulse"))
            {
                return "WP";
            }

            if (id.Contains("spell_echo"))
            {
                return "SE";
            }

            if (id.Contains("arcane_orbit"))
            {
                return "AO";
            }

            if (id.Contains("blink"))
            {
                return "BL";
            }

            if (id.Contains("lunge"))
            {
                return "LG";
            }

            if (id.Contains("teleport"))
            {
                return "TP";
            }

            if (id.Contains("whirlwind"))
            {
                return "WH";
            }

            if (id.Contains("fire_bolt"))
            {
                return "FB";
            }

            if (id.Contains("flame_wave"))
            {
                return "FW";
            }

            if (id.Contains("burning_ground"))
            {
                return "BG";
            }

            return "??";
        }

        private static string ResolveSkillId(string choiceId)
        {
            if (choiceId.StartsWith("upgrade_arcane_magic_missile"))
            {
                return "arcane_magic_missile";
            }

            if (choiceId.StartsWith("upgrade_arcane_arcane_blast"))
            {
                return "arcane_arcane_blast";
            }

            if (choiceId.StartsWith("upgrade_arcane_warp_pulse"))
            {
                return "arcane_warp_pulse";
            }

            if (choiceId.StartsWith("upgrade_arcane_spell_echo"))
            {
                return "arcane_spell_echo";
            }

            if (choiceId.StartsWith("upgrade_arcane_arcane_orbit"))
            {
                return "arcane_arcane_orbit";
            }

            if (choiceId.StartsWith("upgrade_fire_fire_bolt"))
            {
                return "fire_fire_bolt";
            }

            if (choiceId.StartsWith("upgrade_fire_flame_wave"))
            {
                return "fire_flame_wave";
            }

            if (choiceId.StartsWith("upgrade_fire_burning_ground"))
            {
                return "fire_burning_ground";
            }

            return choiceId;
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color color);
            return color;
        }

        private static Color Darken(Color color, float multiplier)
        {
            return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, 0.96f);
        }

        private static bool IsBright(Color color)
        {
            return color.r * 0.299f + color.g * 0.587f + color.b * 0.114f > 0.62f;
        }
    }
}
