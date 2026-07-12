using Heroic.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroic.UI
{
    public class MovementSlotPresenter : MonoBehaviour
    {
        [SerializeField] private MovementCaster movementCaster;
        [SerializeField] private int displayIndex;
        [SerializeField] private Image skillIconImage;
        [SerializeField] private TMP_Text skillNameText;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private Image cooldownFill;
        [SerializeField] private Image backgroundImage;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Outline activeOutline;
        private static readonly string MovementIconRoot = "MovementIcons/";
        private const float SlotSize = 144f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
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
            string resourceName = skill switch
            {
                MovementCaster.MovementSkillId.Blink => "movement_blink",
                MovementCaster.MovementSkillId.Lunge => "movement_lunge",
                MovementCaster.MovementSkillId.Teleport => "movement_teleport",
                MovementCaster.MovementSkillId.Whirlwind => "movement_whirlwind",
                MovementCaster.MovementSkillId.CloudWalk => "movement_cloud_walk",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(resourceName))
            {
                Sprite sprite = Resources.Load<Sprite>(MovementIconRoot + resourceName);
                if (sprite != null)
                {
                    return sprite;
                }

                Texture2D texture = Resources.Load<Texture2D>(MovementIconRoot + resourceName);
                if (texture != null)
                {
                    Rect rect = new Rect(0f, 0f, texture.width, texture.height);
                    return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 384f);
                }
            }

            return Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        }
    }
}
