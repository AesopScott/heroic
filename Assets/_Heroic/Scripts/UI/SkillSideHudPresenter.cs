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
        [SerializeField] private int maxVisibleSlots = 24;
        [SerializeField] private float slotSize = 144f;
        [SerializeField] private float slotSpacing = 158f;
        [SerializeField] private float upgradeBadgeWidth = 54f;
        [SerializeField] private float upgradeBadgeHeight = 54f;
        [SerializeField] private float upgradeBadgeSpacing = 60f;
        [SerializeField] private Texture2D pairedSystemIconSheet;

        private readonly List<IconSlot> abilitySlots = new List<IconSlot>();
        private readonly List<IconSlot> systemSlots = new List<IconSlot>();
        private readonly Dictionary<string, float> nextReadyAt = new Dictionary<string, float>();
        private readonly Dictionary<string, Sprite> pairedSystemSprites = new Dictionary<string, Sprite>();
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
            public TMP_Text StatText;
            public readonly List<UpgradeBadge> UpgradeBadges = new List<UpgradeBadge>();
        }

        private class UpgradeBadge
        {
            public GameObject Root;
            public Image Background;
            public Image Icon;
            public TMP_Text TierText;
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
                systemRoot = CreateRail("SystemSkillRail", parent, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-88f, -172f));
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
            header.CooldownOverlay.enabled = false;
            header.CooldownText.text = string.Empty;
            header.Root.SetActive(true);
            ConfigureTooltip(header.Root, "system_magic_systems");

            GameObject line = new GameObject("MagicSystemsDivider");
            line.transform.SetParent(systemRoot, false);
            RectTransform lineRect = line.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0.5f, 1f);
            lineRect.anchorMax = new Vector2(0.5f, 1f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            lineRect.sizeDelta = new Vector2(slotSize + 8f, 3f);
            lineRect.anchoredPosition = new Vector2(0f, -slotSize * 0.5f - 12f);
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
            icon.raycastTarget = true;
            root.AddComponent<SkillTooltipTrigger>();

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

            GameObject statObject = new GameObject("SkillStats");
            statObject.transform.SetParent(root.transform, false);
            RectTransform statRect = statObject.AddComponent<RectTransform>();
            statRect.anchorMin = new Vector2(0f, 0f);
            statRect.anchorMax = new Vector2(1f, 0f);
            statRect.pivot = new Vector2(0.5f, 1f);
            statRect.sizeDelta = new Vector2(0f, 42f);
            statRect.anchoredPosition = new Vector2(0f, -4f);
            TMP_Text statText = statObject.AddComponent<TextMeshProUGUI>();
            statText.text = string.Empty;
            statText.alignment = TextAlignmentOptions.Top;
            statText.fontSize = 13f;
            statText.fontStyle = FontStyles.Bold;
            statText.color = new Color(0.88f, 0.94f, 1f, 0.94f);
            statText.enableWordWrapping = false;
            statText.overflowMode = TextOverflowModes.Ellipsis;
            statText.raycastTarget = false;

            root.SetActive(false);
            return new IconSlot
            {
                Root = root,
                Icon = icon,
                CooldownOverlay = overlay,
                CooldownText = text,
                StatText = statText
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
                    if (!skillId.StartsWith("system_pair_"))
                    {
                        systemIds.Add(skillId);
                    }
                    continue;
                }

                abilityIds.Add(skillId);
            }

            SystemPairDefinitions.AddActivePairs(buildState.LearnedSkillIds, systemIds);

            ApplyList(abilityIds, abilitySlots, ref lastAbilitySignature, 0f, true);
            ApplyList(systemIds, systemSlots, ref lastSystemSignature, slotSize + 30f, false);
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
                    slot.Icon.sprite = LoadSkillIconResource(skillId) ?? ResolveSystemPairIcon(skillId) ?? SkillIconRegistry.GetIcon(skillId);
                    slot.Icon.color = Color.white;
                    slot.StatText.text = FormatStats(skillId);
                    ConfigureTooltip(slot.Root, skillId);
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
                Color tierColor = SkillIconRegistry.GetTierColor(upgrade.Tier);
                badge.Background.color = new Color(tierColor.r, tierColor.g, tierColor.b, 0.92f);
                badge.Icon.sprite = SkillIconRegistry.GetUpgradeIcon(upgrade.UpgradePathId) ?? SkillIconRegistry.GetIcon(skillId);
                badge.Icon.color = Color.white;
                badge.TierText.text = upgrade.Tier.ToString();
                badge.TierText.color = Color.white;
                ConfigureTooltip(badge.Root, upgrade.SkillId, SkillTooltipText.UpgradeBody(upgrade.UpgradePathId, upgrade.Tier));
            }
        }

        private static void ConfigureTooltip(GameObject target, string skillId, string extra = "")
        {
            if (target == null)
            {
                return;
            }

            SkillTooltipTrigger tooltip = target.GetComponent<SkillTooltipTrigger>();
            if (tooltip == null)
            {
                tooltip = target.AddComponent<SkillTooltipTrigger>();
            }

            string resolvedSkillId = SkillIconRegistry.ResolveSkillId(skillId);
            tooltip.Configure(SkillTooltipText.TitleFor(resolvedSkillId), SkillTooltipText.BodyFor(resolvedSkillId, extra));
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
            background.raycastTarget = true;
            root.AddComponent<SkillTooltipTrigger>();

            GameObject iconObject = new GameObject("Icon");
            iconObject.transform.SetParent(root.transform, false);
            RectTransform iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(5f, 5f);
            iconRect.offsetMax = new Vector2(-5f, -5f);
            Image icon = iconObject.AddComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            GameObject tierObject = new GameObject("Tier");
            tierObject.transform.SetParent(root.transform, false);
            RectTransform tierRect = tierObject.AddComponent<RectTransform>();
            tierRect.anchorMin = new Vector2(1f, 0f);
            tierRect.anchorMax = new Vector2(1f, 0f);
            tierRect.pivot = new Vector2(1f, 0f);
            tierRect.sizeDelta = new Vector2(24f, 22f);
            tierRect.anchoredPosition = new Vector2(-3f, 2f);
            TMP_Text tierText = tierObject.AddComponent<TextMeshProUGUI>();
            tierText.alignment = TextAlignmentOptions.Center;
            tierText.fontSize = 16f;
            tierText.fontStyle = FontStyles.Bold;
            tierText.raycastTarget = false;

            root.SetActive(false);
            return new UpgradeBadge
            {
                Root = root,
                Background = background,
                Icon = icon,
                TierText = tierText
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

            if (label.StartsWith("system_pair_"))
            {
                label = label.Substring("system_pair_".Length);
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

        private static Sprite LoadSkillIconResource(string skillId)
        {
            return string.IsNullOrEmpty(skillId) ? null : Resources.Load<Sprite>("SkillIcons/" + skillId);
        }

        private Sprite ResolveSystemPairIcon(string skillId)
        {
            if (pairedSystemIconSheet == null || string.IsNullOrEmpty(skillId) || !skillId.StartsWith("system_pair_"))
            {
                return null;
            }

            if (pairedSystemSprites.TryGetValue(skillId, out Sprite cached))
            {
                return cached;
            }

            int index = ResolvePairIconIndex(skillId);
            int columns = 4;
            int rows = 4;
            int cellWidth = pairedSystemIconSheet.width / columns;
            int cellHeight = pairedSystemIconSheet.height / rows;
            int column = Mathf.Clamp(index % columns, 0, columns - 1);
            int rowFromTop = Mathf.Clamp(index / columns, 0, rows - 1);
            Rect rect = new Rect(column * cellWidth, pairedSystemIconSheet.height - (rowFromTop + 1) * cellHeight, cellWidth, cellHeight);
            Sprite sprite = Sprite.Create(pairedSystemIconSheet, rect, new Vector2(0.5f, 0.5f), Mathf.Max(cellWidth, cellHeight));
            pairedSystemSprites[skillId] = sprite;
            return sprite;
        }

        private static int ResolvePairIconIndex(string pairId)
        {
            switch (pairId)
            {
                case "system_pair_territory_components":
                    return 0;
                case "system_pair_territory_sacrifice":
                    return 1;
                case "system_pair_territory_rhythm":
                    return 2;
                case "system_pair_territory_tension":
                    return 3;
                case "system_pair_components_sacrifice":
                    return 4;
                case "system_pair_components_rhythm":
                    return 5;
                case "system_pair_components_tension":
                    return 6;
                case "system_pair_sacrifice_rhythm":
                    return 7;
                case "system_pair_sacrifice_tension":
                    return 8;
                case "system_pair_rhythm_tension":
                    return 9;
            }

            unchecked
            {
                int hash = 17;
                for (int i = 0; i < pairId.Length; i++)
                {
                    hash = hash * 31 + pairId[i];
                }

                return Mathf.Abs(hash) % 16;
            }
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
            return SkillRuntimeCatalog.Get(skillId).Cooldown;
        }

        private static string FormatStats(string skillId)
        {
            SkillRuntimeStats stats = SkillRuntimeCatalog.Get(skillId);
            if (skillId.StartsWith("system_"))
            {
                return stats.Effect;
            }

            return stats.BaseSpec;
        }
    }
}
