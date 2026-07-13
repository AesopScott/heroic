using Heroic.Player;
using Heroic.Systems;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroic.UI
{
    public class MovementSlotPresenter : MonoBehaviour
    {
        [SerializeField] private MovementCaster movementCaster;
        [SerializeField] private RunBuildState buildState;
        [SerializeField] private int displayIndex;
        [SerializeField] private Image skillIconImage;
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private Image cooldownFill;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float upgradeBadgeWidth = 54f;
        [SerializeField] private float upgradeBadgeHeight = 54f;
        [SerializeField] private float upgradeBadgeSpacing = 60f;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Outline activeOutline;
        private readonly List<UpgradeBadge> upgradeBadges = new List<UpgradeBadge>();
        private const float SlotSize = 144f;

        private class UpgradeBadge
        {
            public GameObject Root;
            public Image Background;
            public Image Icon;
            public TMP_Text TierText;
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (buildState == null)
            {
                buildState = FindAnyObjectByType<RunBuildState>();
            }

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            backgroundImage = GetComponent<Image>();
            if (backgroundImage == null)
            {
                backgroundImage = gameObject.AddComponent<Image>();
            }

            activeOutline = GetComponent<Outline>();
            if (activeOutline == null)
            {
                activeOutline = gameObject.AddComponent<Outline>();
            }

            activeOutline.effectColor = new Color(1f, 0.84f, 0.2f, 1f);
            activeOutline.effectDistance = new Vector2(4f, 4f);
            gameObject.AddComponent<SkillTooltipTrigger>();

            ForceSquareWindow();
            EnsureIcon();

            if (cooldownFill != null)
            {
                cooldownFill.enabled = false;
                cooldownFill.raycastTarget = false;
            }
        }

        private void Update()
        {
            if (movementCaster == null)
            {
                return;
            }

            MovementCaster.MovementSkillId skill = movementCaster.GetDisplayedMovementSkill(displayIndex);
            float remaining = movementCaster.GetDisplayedRemainingCooldown(displayIndex);
            float cooldown = movementCaster.GetDisplayedCooldown(displayIndex);
            bool hasSkill = skill != MovementCaster.MovementSkillId.None;
            bool isActive = movementCaster.IsDisplayedSkillActive(displayIndex);
            bool isReady = hasSkill && remaining <= 0f;
            string movementSkillId = MovementIdFor(skill);
            canvasGroup.alpha = hasSkill ? 1f : 0f;
            canvasGroup.interactable = hasSkill;
            canvasGroup.blocksRaycasts = hasSkill;
            if (backgroundImage != null)
            {
                backgroundImage.sprite = hasSkill ? GetSkillIconSprite(skill) : null;
                backgroundImage.preserveAspect = true;
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.raycastTarget = hasSkill;
                backgroundImage.color = !hasSkill
                    ? new Color(0.05f, 0.08f, 0.1f, 0.12f)
                    : isReady
                        ? Color.white
                        : new Color(0.42f, 0.42f, 0.46f, 1f);
            }

            if (activeOutline != null)
            {
                activeOutline.enabled = hasSkill && isActive;
            }

            if (skillIconImage != null)
            {
                skillIconImage.enabled = false;
            }

            if (skillNameText != null)
            {
                skillNameText.text = hasSkill ? (displayIndex + 1).ToString() : string.Empty;
                skillNameText.fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
                skillNameText.color = hasSkill ? (isReady ? Color.white : new Color(0.86f, 0.86f, 0.86f, 1f)) : Color.clear;
            }

            if (cooldownText != null)
            {
                cooldownText.text = hasSkill && remaining > 0f ? remaining.ToString("0.0") : string.Empty;
                cooldownText.color = hasSkill ? new Color(0.95f, 0.95f, 0.98f, 1f) : Color.clear;
            }

            if (cooldownFill != null)
            {
                cooldownFill.enabled = false;
                cooldownFill.fillAmount = 0f;
                cooldownFill.color = Color.clear;
            }

            RefreshUpgradeBadges(hasSkill ? movementSkillId : string.Empty);
            ConfigureTooltip(hasSkill ? movementSkillId : string.Empty);
        }

        private void ForceSquareWindow()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(SlotSize, SlotSize);
            }
        }

        private void EnsureIcon()
        {
            if (skillIconImage != null)
            {
                return;
            }

            Transform iconTransform = transform.Find("SkillIcon");
            if (iconTransform == null)
            {
                GameObject iconObject = new GameObject("SkillIcon");
                iconObject.transform.SetParent(transform, false);
                RectTransform iconRect = iconObject.AddComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.offsetMin = Vector2.zero;
                iconRect.offsetMax = Vector2.zero;
                skillIconImage = iconObject.AddComponent<Image>();
            }
            else
            {
                skillIconImage = iconTransform.GetComponent<Image>();
            }

            if (skillIconImage != null && skillIconImage.sprite == null)
            {
                skillIconImage.sprite = GetSkillIconSprite(MovementCaster.MovementSkillId.None);
                skillIconImage.preserveAspect = true;
                skillIconImage.raycastTarget = false;
            }

            skillIconImage.enabled = false;
            skillIconImage.transform.SetAsFirstSibling();
        }

        private static Sprite GetSkillIconSprite(MovementCaster.MovementSkillId skill)
        {
            string resourceName = MovementIdFor(skill);

            if (!string.IsNullOrEmpty(resourceName))
            {
                return SkillIconRegistry.GetIcon(resourceName);
            }

            return Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        }

        private void RefreshUpgradeBadges(string movementSkillId)
        {
            List<RunBuildState.SkillUpgradeState> upgrades = UpgradesForSkill(movementSkillId);
            EnsureUpgradeBadgeCount(upgrades.Count);

            for (int i = 0; i < upgradeBadges.Count; i++)
            {
                UpgradeBadge badge = upgradeBadges[i];
                bool hasUpgrade = i < upgrades.Count;
                badge.Root.SetActive(hasUpgrade);
                if (!hasUpgrade)
                {
                    continue;
                }

                ApplyBadgePlacement(badge.Root.GetComponent<RectTransform>(), i);
                RunBuildState.SkillUpgradeState upgrade = upgrades[i];
                Color tierColor = SkillIconRegistry.GetTierColor(upgrade.Tier);
                badge.Background.color = new Color(tierColor.r, tierColor.g, tierColor.b, 0.92f);
                badge.Icon.sprite = SkillIconRegistry.GetUpgradeIcon(upgrade.UpgradePathId) ?? SkillIconRegistry.GetIcon(movementSkillId);
                badge.Icon.color = Color.white;
                badge.TierText.text = upgrade.Tier.ToString();
                badge.TierText.color = Color.white;
                ConfigureBadgeTooltip(badge.Root, upgrade);
            }
        }

        private void ConfigureTooltip(string movementSkillId)
        {
            SkillTooltipTrigger tooltip = GetComponent<SkillTooltipTrigger>();
            if (tooltip == null)
            {
                tooltip = gameObject.AddComponent<SkillTooltipTrigger>();
            }

            tooltip.Configure(
                string.IsNullOrEmpty(movementSkillId) ? string.Empty : SkillTooltipText.TitleFor(movementSkillId),
                string.IsNullOrEmpty(movementSkillId) ? string.Empty : SkillTooltipText.BodyFor(movementSkillId));
        }

        private static void ConfigureBadgeTooltip(GameObject target, RunBuildState.SkillUpgradeState upgrade)
        {
            SkillTooltipTrigger tooltip = target.GetComponent<SkillTooltipTrigger>();
            if (tooltip == null)
            {
                tooltip = target.AddComponent<SkillTooltipTrigger>();
            }

            string skillId = SkillIconRegistry.ResolveSkillId(upgrade.SkillId);
            tooltip.Configure(SkillTooltipText.TitleFor(skillId), SkillTooltipText.BodyFor(skillId, SkillTooltipText.UpgradeBody(upgrade.UpgradePathId, upgrade.Tier)));
        }

        private void ApplyBadgePlacement(RectTransform badgeRect, int badgeIndex)
        {
            int equippedCount = movementCaster != null ? movementCaster.GetEquippedMovementSkillCount() : 0;
            if (equippedCount >= 3 || displayIndex >= 2)
            {
                badgeRect.anchorMin = new Vector2(0.5f, 1f);
                badgeRect.anchorMax = new Vector2(0.5f, 1f);
                badgeRect.pivot = new Vector2(0.5f, 0f);
                badgeRect.anchoredPosition = new Vector2(0f, 12f + badgeIndex * upgradeBadgeSpacing);
                return;
            }

            bool placeRight = displayIndex == 0;
            badgeRect.anchorMin = new Vector2(0.5f, 0.5f);
            badgeRect.anchorMax = new Vector2(0.5f, 0.5f);
            badgeRect.pivot = placeRight ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
            float x = placeRight ? (SlotSize * 0.5f + 12f) : -(SlotSize * 0.5f + 12f);
            float y = SlotSize * 0.32f - badgeIndex * upgradeBadgeSpacing;
            badgeRect.anchoredPosition = new Vector2(x, y);
        }

        private List<RunBuildState.SkillUpgradeState> UpgradesForSkill(string movementSkillId)
        {
            List<RunBuildState.SkillUpgradeState> matches = new List<RunBuildState.SkillUpgradeState>();
            if (buildState == null || string.IsNullOrEmpty(movementSkillId))
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
                if (upgrade.SkillId == movementSkillId || resolvedSkillId == movementSkillId)
                {
                    matches.Add(upgrade);
                }
            }

            return matches;
        }

        private void EnsureUpgradeBadgeCount(int count)
        {
            while (upgradeBadges.Count < count)
            {
                upgradeBadges.Add(CreateUpgradeBadge("MovementUpgradeBadge" + (upgradeBadges.Count + 1)));
            }
        }

        private UpgradeBadge CreateUpgradeBadge(string name)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(transform, false);
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(upgradeBadgeWidth, upgradeBadgeHeight);

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

        private static string MovementIdFor(MovementCaster.MovementSkillId skill)
        {
            switch (skill)
            {
                case MovementCaster.MovementSkillId.Blink:
                    return "movement_blink";
                case MovementCaster.MovementSkillId.Lunge:
                    return "movement_lunge";
                case MovementCaster.MovementSkillId.Teleport:
                    return "movement_teleport";
                case MovementCaster.MovementSkillId.Whirlwind:
                    return "movement_whirlwind";
                case MovementCaster.MovementSkillId.CloudWalk:
                    return "movement_cloud_walk";
                case MovementCaster.MovementSkillId.Invisibility:
                    return "movement_invisibility";
                case MovementCaster.MovementSkillId.Stoneskin:
                    return "movement_stoneskin";
                case MovementCaster.MovementSkillId.Tunnel:
                    return "movement_tunnel";
                case MovementCaster.MovementSkillId.Flight:
                    return "movement_flight";
                default:
                    return string.Empty;
            }
        }

        private static string FormatUpgradeLabel(string upgradePathId, string movementSkillId)
        {
            string label = upgradePathId ?? string.Empty;
            if (label.StartsWith("upgrade_"))
            {
                label = label.Substring("upgrade_".Length);
            }

            if (!string.IsNullOrEmpty(movementSkillId) && label.StartsWith(movementSkillId + "_"))
            {
                label = label.Substring(movementSkillId.Length + 1);
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
                if (string.IsNullOrEmpty(words[i]))
                {
                    continue;
                }

                words[i] = char.ToUpperInvariant(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1) : string.Empty);
            }

            return string.Join(" ", words);
        }
    }
}
