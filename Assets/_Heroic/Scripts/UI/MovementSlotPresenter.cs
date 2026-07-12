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
        [SerializeField] private float upgradeBadgeWidth = 172f;
        [SerializeField] private float upgradeBadgeHeight = 30f;
        [SerializeField] private float upgradeBadgeSpacing = 34f;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Outline activeOutline;
        private readonly List<UpgradeBadge> upgradeBadges = new List<UpgradeBadge>();
        private const float SlotSize = 144f;

        private class UpgradeBadge
        {
            public GameObject Root;
            public Image Background;
            public Image ColorChip;
            public TMP_Text Label;
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
                backgroundImage.raycastTarget = false;
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
                Color color = SkillIconRegistry.GetColor(movementSkillId);
                badge.Background.color = new Color(color.r * 0.22f, color.g * 0.22f, color.b * 0.22f, 0.88f);
                badge.ColorChip.color = color;
                badge.Label.text = FormatUpgradeLabel(upgrades[i].UpgradePathId, movementSkillId) + " " + upgrades[i].Tier;
            }
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
