using Heroic.Systems;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroic.UI
{
    public class SkillSideHudPresenter : MonoBehaviour
    {
        [SerializeField] private RunBuildState buildState;
        [SerializeField] private RectTransform abilityRoot;
        [SerializeField] private RectTransform systemRoot;
        [SerializeField] private int maxVisibleSlots = 12;
        [SerializeField] private float slotSize = 144f;
        [SerializeField] private float slotSpacing = 158f;
        [SerializeField] private float upgradeBadgeWidth = 180f;
        [SerializeField] private float upgradeBadgeHeight = 30f;
        [SerializeField] private float upgradeBadgeSpacing = 34f;

        private readonly List<IconSlot> abilitySlots = new List<IconSlot>();
        private readonly List<IconSlot> systemSlots = new List<IconSlot>();
        private readonly Dictionary<string, float> nextReadyAt = new Dictionary<string, float>();
        private readonly List<string> abilityIds = new List<string>();
        private readonly List<string> systemIds = new List<string>();
        private float nextRefreshAt;
        private string lastAbilitySignature = string.Empty;
        private string lastSystemSignature = string.Empty;

        private class IconSlot
        {
            public GameObject Root;
            public Image Icon;
            public Image CooldownOverlay;
            public TMP_Text CooldownText;
            public readonly List<UpgradeBadge> UpgradeBadges = new List<UpgradeBadge>();
        }

        private class UpgradeBadge
        {
            public GameObject Root;
            public Image Background;
            public Image ColorChip;
            public TMP_Text Label;
        }

        private void Awake()
        {
            if (buildState == null)
            {
                buildState = FindAnyObjectByType<RunBuildState>();
            }

            EnsureRoots();
            EnsureSlots(abilityRoot, abilitySlots, false);
            EnsureSystemHeader();
            EnsureSlots(systemRoot, systemSlots, true);
        }

        private void Update()
        {
            if (buildState == null)
            {
                return;
            }

            if (Time.unscaledTime >= nextRefreshAt)
            {
                RefreshLists();
                nextRefreshAt = Time.unscaledTime + 0.2f;
            }

            RefreshCooldownVisuals();
        }

        private void EnsureRoots()
        {
            RectTransform parent = transform as RectTransform;
            if (parent == null)
            {
                parent = gameObject.AddComponent<RectTransform>();
            }

            if (abilityRoot == null)
            {
                abilityRoot = CreateRail("AbilitySkillRail", parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(88f, -172f));
            }

            if (systemRoot == null)
            {
                systemRoot = CreateRail("SystemSkillRail", parent, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-88f, -132f));
            }
        }

        private RectTransform CreateRail(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(slotSize, slotSpacing * maxVisibleSlots);
            rect.anchoredPosition = anchoredPosition;
            return rect;
        }

        private void EnsureSlots(RectTransform root, List<IconSlot> slots, bool rightAligned)
        {
            while (slots.Count < maxVisibleSlots)
            {
                int index = slots.Count;
                IconSlot slot = CreateSlot(root, "SkillSlot" + (index + 1), new Vector2(0f, -index * slotSpacing));
                slots.Add(slot);
            }
        }

        private void EnsureSystemHeader()
        {
            IconSlot header = CreateSlot(systemRoot, "MagicSystemsHeader", Vector2.zero);
            header.Icon.sprite = SkillIconRegistry.GetIcon("system_magic_systems");
            header.Icon.color = Color.white;
            header.Root.SetActive(true);

            GameObject line = new GameObject("MagicSystemsDivider");
            line.transform.SetParent(systemRoot, false);
            RectTransform lineRect = line.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0.5f, 1f);
            lineRect.anchorMax = new Vector2(0.5f, 1f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            lineRect.sizeDelta = new Vector2(slotSize + 8f, 3f);
            lineRect.anchoredPosition = new Vector2(0f, -slotSize - 10f);
            Image image = line.AddComponent<Image>();
            image.color = new Color(0.78f, 0.77f, 1f, 0.75f);
        }

        private IconSlot CreateSlot(Transform parent, string name, Vector2 anchoredPosition)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            RectTransform rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(slotSize, slotSize);
            rect.anchoredPosition = anchoredPosition;

            Image icon = root.AddComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            GameObject overlayObject = new GameObject("CooldownOverlay");
            overlayObject.transform.SetParent(root.transform, false);
            RectTransform overlayRect = overlayObject.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            Image overlay = overlayObject.AddComponent<Image>();
            overlay.color = new Color(0.1f, 0.12f, 0.14f, 0.72f);
            overlay.raycastTarget = false;

            GameObject textObject = new GameObject("CooldownText");
            textObject.transform.SetParent(root.transform, false);
            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = string.Empty;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 24f;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.raycastTarget = false;

            root.SetActive(false);
            return new IconSlot
            {
                Root = root,
                Icon = icon,
                CooldownOverlay = overlay,
                CooldownText = text
            };
        }

        private void RefreshLists()
        {
            abilityIds.Clear();
            systemIds.Clear();

            foreach (string skillId in buildState.LearnedSkillIds)
            {
                if (string.IsNullOrEmpty(skillId) || skillId.StartsWith("movement_"))
                {
                    continue;
                }

                if (skillId.StartsWith("system_"))
                {
                    if (!skillId.StartsWith("system_synergy_"))
                    {
                        systemIds.Add(skillId);
                    }
                    continue;
                }

                abilityIds.Add(skillId);
            }

            ApplyList(abilityIds, abilitySlots, ref lastAbilitySignature, 0f, true);
            ApplyList(systemIds, systemSlots, ref lastSystemSignature, slotSpacing + 12f, false);
        }

        private void ApplyList(List<string> ids, List<IconSlot> slots, ref string signature, float yOffset, bool badgesToRight)
        {
            string newSignature = string.Join("|", ids) + "#" + UpgradeSignature(ids);
            if (newSignature != signature)
            {
                signature = newSignature;
                for (int i = 0; i < slots.Count; i++)
                {
                    bool hasSkill = i < ids.Count;
                    IconSlot slot = slots[i];
                    slot.Root.SetActive(hasSkill);
                    if (!hasSkill)
                    {
                        continue;
                    }

                    RectTransform rect = slot.Root.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(0f, -i * slotSpacing - yOffset);
                    string skillId = SkillIconRegistry.ResolveSkillId(ids[i]);
                    slot.Icon.sprite = SkillIconRegistry.GetIcon(skillId);
                    slot.Icon.color = Color.white;
                    ApplyUpgradeBadges(slot, skillId, badgesToRight);
                    if (!nextReadyAt.ContainsKey(skillId))
                    {
                        nextReadyAt[skillId] = Time.time + CooldownFor(skillId);
                    }
                }
            }
        }

        private string UpgradeSignature(List<string> ids)
        {
            if (buildState == null)
            {
                return string.Empty;
            }

            List<string> parts = new List<string>();
            foreach (RunBuildState.SkillUpgradeState upgrade in buildState.SkillUpgrades)
            {
                if (upgrade == null)
                {
                    continue;
                }

                string skillId = SkillIconRegistry.ResolveSkillId(upgrade.SkillId);
                if (!ids.Contains(skillId) && !ids.Contains(upgrade.SkillId))
                {
                    continue;
                }

                parts.Add(skillId + ":" + upgrade.UpgradePathId + ":" + upgrade.Tier);
            }

            return string.Join("|", parts);
        }

        private void ApplyUpgradeBadges(IconSlot slot, string skillId, bool badgesToRight)
        {
            List<RunBuildState.SkillUpgradeState> upgrades = UpgradesForSkill(skillId);
            EnsureUpgradeBadgeCount(slot, upgrades.Count);

            for (int i = 0; i < slot.UpgradeBadges.Count; i++)
            {
                UpgradeBadge badge = slot.UpgradeBadges[i];
                bool hasUpgrade = i < upgrades.Count;
                badge.Root.SetActive(hasUpgrade);
                if (!hasUpgrade)
                {
                    continue;
                }

                RunBuildState.SkillUpgradeState upgrade = upgrades[i];
                RectTransform rect = badge.Root.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = badgesToRight ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
                float x = badgesToRight ? (slotSize * 0.5f + 12f) : -(slotSize * 0.5f + 12f);
                float y = slotSize * 0.34f - i * upgradeBadgeSpacing;
                rect.anchoredPosition = new Vector2(x, y);

                Color color = SkillIconRegistry.GetColor(skillId);
                badge.Background.color = new Color(color.r * 0.25f, color.g * 0.25f, color.b * 0.25f, 0.84f);
                badge.ColorChip.color = color;
                badge.Label.text = FormatUpgradeLabel(upgrade.UpgradePathId, skillId) + " " + upgrade.Tier;
            }
        }

        private List<RunBuildState.SkillUpgradeState> UpgradesForSkill(string skillId)
        {
            List<RunBuildState.SkillUpgradeState> matches = new List<RunBuildState.SkillUpgradeState>();
            if (buildState == null)
            {
                return matches;
            }

            foreach (RunBuildState.SkillUpgradeState upgrade in buildState.SkillUpgrades)
            {
                if (upgrade == null)
                {
                    continue;
                }

                string resolvedSkillId = SkillIconRegistry.ResolveSkillId(upgrade.SkillId);
                if (resolvedSkillId == skillId || upgrade.SkillId == skillId)
                {
                    matches.Add(upgrade);
                }
            }

            return matches;
        }

        private void EnsureUpgradeBadgeCount(IconSlot slot, int count)
        {
            while (slot.UpgradeBadges.Count < count)
            {
                slot.UpgradeBadges.Add(CreateUpgradeBadge(slot.Root.transform, "UpgradeBadge" + (slot.UpgradeBadges.Count + 1)));
            }
        }

        private UpgradeBadge CreateUpgradeBadge(Transform parent, string name)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            RectTransform rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(upgradeBadgeWidth, upgradeBadgeHeight);

            Image background = root.AddComponent<Image>();
            background.raycastTarget = false;

            GameObject chipObject = new GameObject("ColorChip");
            chipObject.transform.SetParent(root.transform, false);
            RectTransform chipRect = chipObject.AddComponent<RectTransform>();
            chipRect.anchorMin = new Vector2(0f, 0.5f);
            chipRect.anchorMax = new Vector2(0f, 0.5f);
            chipRect.pivot = new Vector2(0f, 0.5f);
            chipRect.sizeDelta = new Vector2(8f, upgradeBadgeHeight);
            chipRect.anchoredPosition = Vector2.zero;
            Image chip = chipObject.AddComponent<Image>();
            chip.raycastTarget = false;

            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(root.transform, false);
            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(14f, 0f);
            labelRect.offsetMax = new Vector2(-6f, 0f);
            TMP_Text label = labelObject.AddComponent<TextMeshProUGUI>();
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.fontSize = 15f;
            label.fontStyle = FontStyles.Bold;
            label.color = Color.white;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.raycastTarget = false;

            root.SetActive(false);
            return new UpgradeBadge
            {
                Root = root,
                Background = background,
                ColorChip = chip,
                Label = label
            };
        }

        private static string FormatUpgradeLabel(string upgradePathId, string skillId)
        {
            string label = upgradePathId ?? string.Empty;
            if (label.StartsWith("upgrade_"))
            {
                label = label.Substring("upgrade_".Length);
            }

            if (!string.IsNullOrEmpty(skillId) && label.StartsWith(skillId + "_"))
            {
                label = label.Substring(skillId.Length + 1);
            }

            if (label.StartsWith("system_synergy_"))
            {
                label = label.Substring("system_synergy_".Length);
            }

            label = label.Replace('_', ' ');
            return ToTitleCase(label);
        }

        private static string ToTitleCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Upgrade";
            }

            string[] words = value.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }

                words[i] = char.ToUpperInvariant(word[0]) + (word.Length > 1 ? word.Substring(1) : string.Empty);
            }

            return string.Join(" ", words);
        }

        private void RefreshCooldownVisuals()
        {
            RefreshAbilityCooldowns();
            for (int i = 0; i < systemSlots.Count; i++)
            {
                systemSlots[i].CooldownOverlay.enabled = false;
                systemSlots[i].CooldownText.text = string.Empty;
            }
        }

        private void RefreshAbilityCooldowns()
        {
            for (int i = 0; i < abilityIds.Count && i < abilitySlots.Count; i++)
            {
                string skillId = abilityIds[i];
                float cooldown = CooldownFor(skillId);
                if (!nextReadyAt.TryGetValue(skillId, out float readyAt))
                {
                    readyAt = Time.time + cooldown;
                    nextReadyAt[skillId] = readyAt;
                }

                float remaining = readyAt - Time.time;
                if (remaining <= 0f)
                {
                    nextReadyAt[skillId] = Time.time + cooldown;
                    remaining = 0f;
                }

                IconSlot slot = abilitySlots[i];
                bool coolingDown = remaining > 0.05f;
                slot.CooldownOverlay.enabled = coolingDown;
                slot.CooldownText.text = coolingDown ? remaining.ToString(remaining < 1f ? "0.0" : "0") : string.Empty;
                slot.Icon.color = coolingDown ? new Color(0.58f, 0.58f, 0.62f, 1f) : Color.white;
            }
        }

        private static float CooldownFor(string skillId)
        {
            switch (skillId)
            {
                case "arcane_magic_missile":
                    return 0.75f;
                case "fire_fire_bolt":
                    return 1f;
                case "lightning_spark_surge":
                    return 1.1f;
                case "cold_frost_ring":
                case "fire_flame_wave":
                    return 3.2f;
                case "system_territory_casting":
                    return 0f;
                default:
                    return 2.4f;
            }
        }
    }
}
