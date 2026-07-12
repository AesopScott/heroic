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
            int[] laneTotals = CountLanes(choices);
            int[] laneIndices = new int[3];

            if (headerText != null)
            {
                headerText.text = "Choose your spellbook path";
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
                PositionChoiceButton(i, choices[i], laneTotals, laneIndices);
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

        private void PositionChoiceButton(int index, UpgradeManager.DraftChoice choice, int[] laneTotals, int[] laneIndices)
        {
            if (index >= choiceButtons.Length || choiceButtons[index] == null)
            {
                return;
            }

            int lane = ResolveLane(choice);
            int laneIndex = laneIndices[lane]++;
            int laneTotal = Mathf.Max(1, laneTotals[lane]);
            RectTransform rect = choiceButtons[index].GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            float x = lane == 0 ? -420f : lane == 2 ? 420f : 0f;
            float y = ((laneTotal - 1) * 68f) - laneIndex * 136f;
            rect.anchoredPosition = new Vector2(x, y);
        }

        private static int[] CountLanes(IReadOnlyList<UpgradeManager.DraftChoice> choices)
        {
            int[] totals = new int[3];
            if (choices == null)
            {
                return totals;
            }

            foreach (UpgradeManager.DraftChoice choice in choices)
            {
                if (choice != null)
                {
                    totals[ResolveLane(choice)]++;
                }
            }

            return totals;
        }

        private static int ResolveLane(UpgradeManager.DraftChoice choice)
        {
            if (choice.Category == UpgradeManager.UpgradeCategory.Movement || IsMovementBoost(choice))
            {
                return 0;
            }

            if (choice.Category == UpgradeManager.UpgradeCategory.System || IsSystemBoost(choice))
            {
                return 2;
            }

            return 1;
        }

        private static bool IsMovementBoost(UpgradeManager.DraftChoice choice)
        {
            return choice.Category == UpgradeManager.UpgradeCategory.Boost && ResolveSkillId(choice.Id).StartsWith("movement_");
        }

        private static bool IsSystemBoost(UpgradeManager.DraftChoice choice)
        {
            return choice.Category == UpgradeManager.UpgradeCategory.Boost && ResolveSkillId(choice.Id).StartsWith("system_");
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

            if (choice.Category == UpgradeManager.UpgradeCategory.System || id.StartsWith("system_") || id.StartsWith("upgrade_system_"))
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
                case UpgradeManager.UpgradeCategory.Boost:
                    return "UPG";
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

            if (id.Contains("cloud_walk"))
            {
                return "CW";
            }

            if (id.Contains("invisibility"))
            {
                return "IV";
            }

            if (id.Contains("stoneskin"))
            {
                return "SK";
            }

            if (id.Contains("tunnel"))
            {
                return "TN";
            }

            if (id.Contains("flight"))
            {
                return "FL";
            }

            if (id.Contains("territory_casting"))
            {
                return "TC";
            }

            if (id.Contains("component_boosts"))
            {
                return "CB";
            }

            if (id.Contains("sacrifice_casting"))
            {
                return "SC";
            }

            if (id.Contains("rhythm_casting"))
            {
                return "RC";
            }

            if (id.Contains("spell_tension"))
            {
                return "ST";
            }

            if (id.Contains("synergy_territory_components"))
            {
                return "TC";
            }

            if (id.Contains("synergy_territory_sacrifice"))
            {
                return "TS";
            }

            if (id.Contains("synergy_territory_rhythm"))
            {
                return "TR";
            }

            if (id.Contains("synergy_territory_tension"))
            {
                return "TT";
            }

            if (id.Contains("synergy_components_sacrifice"))
            {
                return "CS";
            }

            if (id.Contains("synergy_components_rhythm"))
            {
                return "CR";
            }

            if (id.Contains("synergy_components_tension"))
            {
                return "CT";
            }

            if (id.Contains("synergy_sacrifice_rhythm"))
            {
                return "SR";
            }

            if (id.Contains("synergy_sacrifice_tension"))
            {
                return "ST";
            }

            if (id.Contains("synergy_rhythm_tension"))
            {
                return "RT";
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

            if (id.Contains("flame_shield"))
            {
                return "FS";
            }

            if (id.Contains("flame_wall"))
            {
                return "FL";
            }

            if (id.Contains("frost_ring"))
            {
                return "FR";
            }

            if (id.Contains("ice_shard"))
            {
                return "IS";
            }

            if (id.Contains("glacial_field"))
            {
                return "GF";
            }

            if (id.Contains("crystal_prison"))
            {
                return "CP";
            }

            if (id.Contains("shatter_line"))
            {
                return "SL";
            }

            if (id.Contains("chain_bolt"))
            {
                return "CB";
            }

            if (id.Contains("static_field"))
            {
                return "SF";
            }

            if (id.Contains("thunder_lance"))
            {
                return "TL";
            }

            if (id.Contains("spark_surge"))
            {
                return "SS";
            }

            if (id.Contains("storm_call"))
            {
                return "SC";
            }

            if (id.Contains("stone_spike"))
            {
                return "SP";
            }

            if (id.Contains("boulder_toss"))
            {
                return "BT";
            }

            if (id.Contains("earth_wall"))
            {
                return "EW";
            }

            if (id.Contains("quake"))
            {
                return "QK";
            }

            if (id.Contains("mud_trap"))
            {
                return "MT";
            }

            if (id.Contains("psychic_lance"))
            {
                return "PL";
            }

            if (id.Contains("fear_wave"))
            {
                return "FE";
            }

            if (id.Contains("illusion_clone"))
            {
                return "IC";
            }

            if (id.Contains("confuse"))
            {
                return "CF";
            }

            if (id.Contains("mind_crush"))
            {
                return "MC";
            }

            if (id.Contains("blood_bolt"))
            {
                return "BB";
            }

            if (id.Contains("sanguine_pact"))
            {
                return "SG";
            }

            if (id.Contains("blood_nova"))
            {
                return "BN";
            }

            if (id.Contains("leech_bind"))
            {
                return "LB";
            }

            if (id.Contains("crimson_frenzy"))
            {
                return "CZ";
            }

            if (id.Contains("poison_dart"))
            {
                return "PD";
            }

            if (id.Contains("toxic_cloud"))
            {
                return "TX";
            }

            if (id.Contains("venom_trail"))
            {
                return "VT";
            }

            if (id.Contains("infection"))
            {
                return "IN";
            }

            if (id.Contains("rot_bloom"))
            {
                return "RB";
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

            if (choiceId.StartsWith("upgrade_fire_flame_shield"))
            {
                return "fire_flame_shield";
            }

            if (choiceId.StartsWith("upgrade_fire_flame_wall"))
            {
                return "fire_flame_wall";
            }

            if (choiceId.StartsWith("upgrade_cold_frost_ring"))
            {
                return "cold_frost_ring";
            }

            if (choiceId.StartsWith("upgrade_cold_ice_shard"))
            {
                return "cold_ice_shard";
            }

            if (choiceId.StartsWith("upgrade_cold_glacial_field"))
            {
                return "cold_glacial_field";
            }

            if (choiceId.StartsWith("upgrade_cold_crystal_prison"))
            {
                return "cold_crystal_prison";
            }

            if (choiceId.StartsWith("upgrade_cold_shatter_line"))
            {
                return "cold_shatter_line";
            }

            if (choiceId.StartsWith("upgrade_lightning_chain_bolt"))
            {
                return "lightning_chain_bolt";
            }

            if (choiceId.StartsWith("upgrade_lightning_static_field"))
            {
                return "lightning_static_field";
            }

            if (choiceId.StartsWith("upgrade_lightning_thunder_lance"))
            {
                return "lightning_thunder_lance";
            }

            if (choiceId.StartsWith("upgrade_lightning_spark_surge"))
            {
                return "lightning_spark_surge";
            }

            if (choiceId.StartsWith("upgrade_lightning_storm_call"))
            {
                return "lightning_storm_call";
            }

            if (choiceId.StartsWith("upgrade_earth_stone_spike"))
            {
                return "earth_stone_spike";
            }

            if (choiceId.StartsWith("upgrade_earth_boulder_toss"))
            {
                return "earth_boulder_toss";
            }

            if (choiceId.StartsWith("upgrade_earth_earth_wall"))
            {
                return "earth_earth_wall";
            }

            if (choiceId.StartsWith("upgrade_earth_quake"))
            {
                return "earth_quake";
            }

            if (choiceId.StartsWith("upgrade_earth_mud_trap"))
            {
                return "earth_mud_trap";
            }

            if (choiceId.StartsWith("upgrade_mind_psychic_lance"))
            {
                return "mind_psychic_lance";
            }

            if (choiceId.StartsWith("upgrade_mind_fear_wave"))
            {
                return "mind_fear_wave";
            }

            if (choiceId.StartsWith("upgrade_mind_illusion_clone"))
            {
                return "mind_illusion_clone";
            }

            if (choiceId.StartsWith("upgrade_mind_confuse"))
            {
                return "mind_confuse";
            }

            if (choiceId.StartsWith("upgrade_mind_mind_crush"))
            {
                return "mind_mind_crush";
            }

            if (choiceId.StartsWith("upgrade_blood_blood_bolt"))
            {
                return "blood_blood_bolt";
            }

            if (choiceId.StartsWith("upgrade_blood_sanguine_pact"))
            {
                return "blood_sanguine_pact";
            }

            if (choiceId.StartsWith("upgrade_blood_blood_nova"))
            {
                return "blood_blood_nova";
            }

            if (choiceId.StartsWith("upgrade_blood_leech_bind"))
            {
                return "blood_leech_bind";
            }

            if (choiceId.StartsWith("upgrade_blood_crimson_frenzy"))
            {
                return "blood_crimson_frenzy";
            }

            if (choiceId.StartsWith("upgrade_poison_poison_dart"))
            {
                return "poison_poison_dart";
            }

            if (choiceId.StartsWith("upgrade_poison_toxic_cloud"))
            {
                return "poison_toxic_cloud";
            }

            if (choiceId.StartsWith("upgrade_poison_venom_trail"))
            {
                return "poison_venom_trail";
            }

            if (choiceId.StartsWith("upgrade_poison_infection"))
            {
                return "poison_infection";
            }

            if (choiceId.StartsWith("upgrade_poison_rot_bloom"))
            {
                return "poison_rot_bloom";
            }

            if (choiceId.StartsWith("upgrade_system_territory_casting"))
            {
                return "system_territory_casting";
            }

            if (choiceId.StartsWith("upgrade_system_synergy_"))
            {
                return choiceId.Replace("upgrade_", string.Empty);
            }

            if (choiceId.StartsWith("upgrade_system_component_boosts"))
            {
                return "system_component_boosts";
            }

            if (choiceId.StartsWith("upgrade_system_sacrifice_casting"))
            {
                return "system_sacrifice_casting";
            }

            if (choiceId.StartsWith("upgrade_system_rhythm_casting"))
            {
                return "system_rhythm_casting";
            }

            if (choiceId.StartsWith("upgrade_system_spell_tension"))
            {
                return "system_spell_tension";
            }

            if (choiceId.StartsWith("upgrade_movement_blink"))
            {
                return "movement_blink";
            }

            if (choiceId.StartsWith("upgrade_movement_lunge"))
            {
                return "movement_lunge";
            }

            if (choiceId.StartsWith("upgrade_movement_teleport"))
            {
                return "movement_teleport";
            }

            if (choiceId.StartsWith("upgrade_movement_whirlwind"))
            {
                return "movement_whirlwind";
            }

            if (choiceId.StartsWith("upgrade_movement_cloud_walk"))
            {
                return "movement_cloud_walk";
            }

            if (choiceId.StartsWith("upgrade_movement_invisibility"))
            {
                return "movement_invisibility";
            }

            if (choiceId.StartsWith("upgrade_movement_stoneskin"))
            {
                return "movement_stoneskin";
            }

            if (choiceId.StartsWith("upgrade_movement_tunnel"))
            {
                return "movement_tunnel";
            }

            if (choiceId.StartsWith("upgrade_movement_flight"))
            {
                return "movement_flight";
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
