using Heroic.Player;
using Heroic.Systems;
using UnityEngine;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerVisualController : MonoBehaviour
    {
        [SerializeField] private Texture2D levelOneTexture;
        [SerializeField] private Texture2D levelTwoTexture;
        [SerializeField] private Texture2D levelSixTexture;
        [SerializeField] private int frameWidth = 384;
        [SerializeField] private int frameHeight = 512;
        [SerializeField] private float secondsPerFrame = 0.22f;
        [SerializeField] private int sortingOrder = 20;
        [SerializeField] private float pixelsPerUnit = 384f;
        [SerializeField] private Vector2 pivotNormalized = new Vector2(0.5f, 0.18f);
        [SerializeField] private Vector2 worldScale = new Vector2(1.12f, 1.12f);

        [Header("School Tints")]
        [SerializeField] private Color arcaneTint = new Color(0.78f, 0.92f, 1f);
        [SerializeField] private Color fireTint = new Color(1f, 0.42f, 0.18f);
        [SerializeField] private Color coldTint = new Color(0.56f, 0.92f, 1f);
        [SerializeField] private Color lightningTint = new Color(1f, 0.93f, 0.28f);
        [SerializeField] private Color earthTint = new Color(0.64f, 0.44f, 0.24f);
        [SerializeField] private Color mindTint = new Color(0.84f, 0.48f, 1f);
        [SerializeField] private Color bloodTint = new Color(0.77f, 0.16f, 0.25f);
        [SerializeField] private Color poisonTint = new Color(0.45f, 0.85f, 0.3f);

        private SpriteRenderer spriteRenderer;
        private PlayerExperience playerExperience;
        private UpgradeManager upgradeManager;
        private string currentSchoolId;
        private Color? accumulatedRobeColor;
        private Sprite[] frames = new Sprite[0];
        private Texture2D activeTexture;
        private int frameIndex;
        private float nextFrameTime;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerExperience = GetComponent<PlayerExperience>();
            upgradeManager = FindAnyObjectByType<UpgradeManager>();
            ApplyForCurrentState();
        }

        private void Update()
        {
            if (frames == null || frames.Length < 2 || Time.time < nextFrameTime)
            {
                return;
            }

            frameIndex = (frameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[frameIndex];
            nextFrameTime = Time.time + secondsPerFrame;
        }

        private void OnEnable()
        {
            if (playerExperience != null)
            {
                playerExperience.LevelChanged += HandleLevelChanged;
            }

            if (upgradeManager != null)
            {
                upgradeManager.ChoiceApplied += HandleChoiceApplied;
            }
        }

        private void OnDisable()
        {
            if (playerExperience != null)
            {
                playerExperience.LevelChanged -= HandleLevelChanged;
            }

            if (upgradeManager != null)
            {
                upgradeManager.ChoiceApplied -= HandleChoiceApplied;
            }
        }

        public void Configure(Texture2D newLevelOneTexture, Texture2D newLevelTwoTexture)
        {
            levelOneTexture = newLevelOneTexture;
            levelTwoTexture = newLevelTwoTexture;
            ApplyForCurrentState();
        }

        public void Configure(Texture2D newLevelOneTexture, Texture2D newLevelTwoTexture, Texture2D newLevelSixTexture)
        {
            levelOneTexture = newLevelOneTexture;
            levelTwoTexture = newLevelTwoTexture;
            levelSixTexture = newLevelSixTexture;
            ApplyForCurrentState();
        }

        private void HandleLevelChanged(int level)
        {
            ApplyForCurrentState();
        }

        private void HandleChoiceApplied(UpgradeManager.DraftChoice choice)
        {
            if (choice == null)
            {
                return;
            }

            currentSchoolId = ResolveSchoolId(choice);
            AccumulateRobeColor(currentSchoolId);
            ApplyForCurrentState();
        }

        private void ApplyForCurrentState()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (playerExperience == null)
            {
                playerExperience = GetComponent<PlayerExperience>();
            }

            int level = playerExperience != null ? playerExperience.Level : 1;
            Texture2D texture = level >= 6 && levelSixTexture != null ? levelSixTexture : level >= 2 ? levelTwoTexture : levelOneTexture;

            if (texture != null && texture != activeTexture)
            {
                BuildFrames(texture);
            }

            spriteRenderer.sortingOrder = sortingOrder;
            transform.localScale = new Vector3(worldScale.x, worldScale.y, 1f);
            spriteRenderer.color = level >= 2 && accumulatedRobeColor.HasValue ? accumulatedRobeColor.Value : Color.white;
        }

        private void AccumulateRobeColor(string schoolId)
        {
            Color nextColor = ResolveTint(schoolId);
            if (!accumulatedRobeColor.HasValue)
            {
                accumulatedRobeColor = nextColor;
                return;
            }

            // Blend the new school into the existing robe color so the palette evolves over time.
            accumulatedRobeColor = Color.Lerp(accumulatedRobeColor.Value, nextColor, 0.5f);
        }

        private void BuildFrames(Texture2D texture)
        {
            activeTexture = texture;
            frameIndex = 0;

            if (texture == null || frameWidth <= 0 || frameHeight <= 0)
            {
                frames = new Sprite[0];
                return;
            }

            int frameCount = Mathf.Max(1, texture.width / frameWidth);
            int rowY = Mathf.Max(0, texture.height - frameHeight);
            frames = new Sprite[frameCount];
            Vector2 pivot = new Vector2(Mathf.Clamp01(pivotNormalized.x), Mathf.Clamp01(pivotNormalized.y));

            for (int i = 0; i < frameCount; i++)
            {
                int x = i * frameWidth;
                frames[i] = Sprite.Create(texture, new Rect(x, rowY, frameWidth, frameHeight), pivot, pixelsPerUnit);
            }

            if (frames.Length > 0)
            {
                spriteRenderer.sprite = frames[0];
                nextFrameTime = Time.time + secondsPerFrame;
            }
        }

        private static string ResolveSchoolId(UpgradeManager.DraftChoice choice)
        {
            if (choice.Category == UpgradeManager.UpgradeCategory.Boost)
            {
                return "arcane";
            }

            string id = choice.Id.ToLowerInvariant();

            if (id.StartsWith("arcane_") || id.Contains("_arcane_"))
            {
                return "arcane";
            }

            if (id.StartsWith("fire_") || id.Contains("_fire_"))
            {
                return "fire";
            }

            if (id.StartsWith("cold_") || id.Contains("_cold_"))
            {
                return "cold";
            }

            if (id.StartsWith("lightning_") || id.Contains("_lightning_"))
            {
                return "lightning";
            }

            if (id.StartsWith("earth_") || id.Contains("_earth_"))
            {
                return "earth";
            }

            if (id.StartsWith("mind_") || id.Contains("_mind_"))
            {
                return "mind";
            }

            if (id.StartsWith("blood_") || id.Contains("_blood_"))
            {
                return "blood";
            }

            if (id.StartsWith("poison_") || id.Contains("_poison_"))
            {
                return "poison";
            }

            return "arcane";
        }

        private Color ResolveTint(string schoolId)
        {
            if (string.IsNullOrEmpty(schoolId))
            {
                return Color.white;
            }

            switch (schoolId)
            {
                case "fire":
                    return fireTint;
                case "cold":
                    return coldTint;
                case "lightning":
                    return lightningTint;
                case "earth":
                    return earthTint;
                case "mind":
                    return mindTint;
                case "blood":
                    return bloodTint;
                case "poison":
                    return poisonTint;
                default:
                    return arcaneTint;
            }
        }
    }
}
