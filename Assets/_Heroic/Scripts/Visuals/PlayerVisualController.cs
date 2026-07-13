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
        [SerializeField] private Texture2D[] levelOneFrames;
        [SerializeField] private Texture2D[] levelTwoFrames;
        [SerializeField] private Texture2D[] levelSixFrames;
        [SerializeField] private int frameWidth = 384;
        [SerializeField] private int frameHeight = 512;
        [SerializeField] private float secondsPerFrame = 0.22f;
        [SerializeField] private int sortingOrder = 20;
        [SerializeField] private float pixelsPerUnit = 384f;
        [SerializeField] private Vector2 pivotNormalized = new Vector2(0.5f, 0.18f);
        [SerializeField] private Vector2 worldScale = new Vector2(1.12f, 1.12f);
        [SerializeField] private Color silhouetteColor = new Color(0.02f, 0.018f, 0.015f, 0.48f);
        [SerializeField] private Vector2 silhouetteOffset = new Vector2(0.05f, -0.05f);
        [SerializeField] private Vector2 silhouetteScale = new Vector2(1.08f, 1.06f);

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
        private PlayerController playerController;
        private PlayerStealth playerStealth;
        private SpriteRenderer silhouetteRenderer;
        private PlayerExperience playerExperience;
        private UpgradeManager upgradeManager;
        private string currentSchoolId;
        private Color? accumulatedRobeColor;
        private Sprite[] frames = new Sprite[0];
        private Texture2D activeTexture;
        private Texture2D[] activeFramesSource;
        private int frameIndex;
        private float nextFrameTime;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            EnsureSilhouetteRenderer();
            playerController = GetComponent<PlayerController>();
            playerStealth = GetComponent<PlayerStealth>();
            playerExperience = GetComponent<PlayerExperience>();
            upgradeManager = FindAnyObjectByType<UpgradeManager>();
            ApplyForCurrentState();
        }

        private void Update()
        {
            if (frames == null || frames.Length < 2 || Time.time < nextFrameTime)
            {
                UpdateFacing();
                return;
            }

            frameIndex = (frameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[frameIndex];
            if (silhouetteRenderer != null)
            {
                silhouetteRenderer.sprite = frames[frameIndex];
            }

            nextFrameTime = Time.time + secondsPerFrame;
            UpdateFacing();
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
            levelOneFrames = null;
            levelTwoFrames = null;
            levelSixFrames = null;
            activeFramesSource = null;
            ApplyForCurrentState();
        }

        public void Configure(Texture2D newLevelOneTexture, Texture2D newLevelTwoTexture, Texture2D newLevelSixTexture)
        {
            levelOneTexture = newLevelOneTexture;
            levelTwoTexture = newLevelTwoTexture;
            levelSixTexture = newLevelSixTexture;
            levelOneFrames = null;
            levelTwoFrames = null;
            levelSixFrames = null;
            activeFramesSource = null;
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

            EnsureSilhouetteRenderer();

            if (playerExperience == null)
            {
                playerExperience = GetComponent<PlayerExperience>();
            }

            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }

            if (playerStealth == null)
            {
                playerStealth = GetComponent<PlayerStealth>();
            }

            int level = playerExperience != null ? playerExperience.Level : 1;
            Texture2D[] framesSource = level >= 6 && HasFrames(levelSixFrames) ? levelSixFrames : level >= 2 && HasFrames(levelTwoFrames) ? levelTwoFrames : HasFrames(levelOneFrames) ? levelOneFrames : null;
            Texture2D texture = level >= 6 && levelSixTexture != null ? levelSixTexture : level >= 2 ? levelTwoTexture : levelOneTexture;

            if (framesSource != null && framesSource != activeFramesSource)
            {
                BuildFrames(framesSource);
            }
            else if (texture != null && texture != activeTexture)
            {
                BuildFrames(texture);
            }

            spriteRenderer.sortingOrder = sortingOrder;
            if (silhouetteRenderer != null)
            {
                silhouetteRenderer.sortingOrder = sortingOrder - 1;
            }

            transform.localScale = new Vector3(worldScale.x, worldScale.y, 1f);
            Color baseColor = level >= 2 && accumulatedRobeColor.HasValue ? accumulatedRobeColor.Value : Color.white;
            spriteRenderer.color = playerStealth != null ? playerStealth.ApplyToBaseColor(baseColor) : baseColor;
            ApplySilhouetteColor();
            UpdateFacing();
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

        private static bool HasFrames(Texture2D[] framesSource)
        {
            return framesSource != null && framesSource.Length > 0;
        }

        private void BuildFrames(Texture2D[] textures)
        {
            activeTexture = null;
            activeFramesSource = textures;
            frameIndex = 0;
            if (textures == null || textures.Length == 0)
            {
                frames = new Sprite[0];
                return;
            }

            frames = new Sprite[textures.Length];
            Vector2 pivot = new Vector2(Mathf.Clamp01(pivotNormalized.x), Mathf.Clamp01(pivotNormalized.y));
            for (int i = 0; i < textures.Length; i++)
            {
                Texture2D texture = textures[i];
                if (texture == null)
                {
                    continue;
                }

                frames[i] = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), pivot, pixelsPerUnit);
            }

            if (frames.Length > 0 && frames[0] != null)
            {
                spriteRenderer.sprite = frames[0];
                if (silhouetteRenderer != null)
                {
                    silhouetteRenderer.sprite = frames[0];
                }

                nextFrameTime = Time.time + secondsPerFrame;
            }
        }

        private void BuildFrames(Texture2D texture)
        {
            activeTexture = texture;
            activeFramesSource = null;
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
                if (silhouetteRenderer != null)
                {
                    silhouetteRenderer.sprite = frames[0];
                }

                nextFrameTime = Time.time + secondsPerFrame;
            }
        }

        private void UpdateFacing()
        {
            if (spriteRenderer == null || playerController == null)
            {
                return;
            }

            spriteRenderer.flipX = playerController.LastHorizontalFacing < 0;
            if (silhouetteRenderer != null)
            {
                silhouetteRenderer.flipX = spriteRenderer.flipX;
            }
        }

        private void EnsureSilhouetteRenderer()
        {
            if (silhouetteRenderer != null)
            {
                return;
            }

            Transform existing = transform.Find("Player Silhouette");
            GameObject silhouette = existing != null ? existing.gameObject : new GameObject("Player Silhouette");
            silhouette.transform.SetParent(transform, false);
            silhouette.transform.localPosition = silhouetteOffset;
            silhouette.transform.localScale = new Vector3(silhouetteScale.x, silhouetteScale.y, 1f);
            silhouetteRenderer = silhouette.GetComponent<SpriteRenderer>();
            if (silhouetteRenderer == null)
            {
                silhouetteRenderer = silhouette.AddComponent<SpriteRenderer>();
            }

            silhouetteRenderer.sortingOrder = sortingOrder - 1;
            ApplySilhouetteColor();
        }

        private void ApplySilhouetteColor()
        {
            if (silhouetteRenderer == null)
            {
                return;
            }

            Color color = silhouetteColor;
            if (playerStealth != null && playerStealth.IsInvisible)
            {
                color.a *= 0.35f;
            }

            silhouetteRenderer.color = color;
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
