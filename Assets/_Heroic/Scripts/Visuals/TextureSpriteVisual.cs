using UnityEngine;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TextureSpriteVisual : MonoBehaviour
    {
        [SerializeField] private Texture2D sourceTexture;
        [SerializeField] private Texture2D[] sourceFrames;
        [SerializeField] private int frameWidth = 0;
        [SerializeField] private int frameHeight = 0;
        [SerializeField] private float secondsPerFrame = 0.35f;
        [SerializeField] private int sortingOrder = 10;
        [SerializeField] private float pixelsPerUnit = 384f;
        [SerializeField] private Vector2 pivotNormalized = new Vector2(0.5f, 0.18f);
        [SerializeField] private Vector2 worldScale = new Vector2(1f, 1f);
        [SerializeField] private bool facePlayer = false;

        private SpriteRenderer spriteRenderer;
        private Sprite[] frames;
        private int frameIndex;
        private float nextFrameTime;
        private Transform playerTransform;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (facePlayer)
            {
                playerTransform = FindAnyObjectByType<Heroic.Player.PlayerController>()?.transform;
            }
            BuildFrames();
            Apply();
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
            nextFrameTime = Time.time + secondsPerFrame;
            UpdateFacing();
        }

        public void Apply()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (frames != null && frames.Length > 0)
            {
                spriteRenderer.sprite = frames[frameIndex];
            }

            spriteRenderer.sortingOrder = sortingOrder;
            transform.localScale = new Vector3(worldScale.x, worldScale.y, 1f);
            UpdateFacing();
        }

        public void Configure(Texture2D texture, int newSortingOrder, float newPixelsPerUnit, Vector2 newPivotNormalized, Vector2 newWorldScale)
        {
            sourceTexture = texture;
            sourceFrames = null;
            sortingOrder = newSortingOrder;
            pixelsPerUnit = newPixelsPerUnit;
            pivotNormalized = newPivotNormalized;
            worldScale = newWorldScale;
            frameWidth = 0;
            frameHeight = 0;
            facePlayer = false;
            BuildFrames();
            Apply();
        }

        public void Configure(Texture2D[] textures, int newSortingOrder, float newPixelsPerUnit, Vector2 newPivotNormalized, Vector2 newWorldScale)
        {
            Configure(textures, newSortingOrder, newPixelsPerUnit, newPivotNormalized, newWorldScale, false);
        }

        public void Configure(Texture2D[] textures, int newSortingOrder, float newPixelsPerUnit, Vector2 newPivotNormalized, Vector2 newWorldScale, bool newFacePlayer)
        {
            sourceTexture = null;
            sourceFrames = textures;
            sortingOrder = newSortingOrder;
            pixelsPerUnit = newPixelsPerUnit;
            pivotNormalized = newPivotNormalized;
            worldScale = newWorldScale;
            frameWidth = 0;
            frameHeight = 0;
            facePlayer = newFacePlayer;
            BuildFrames();
            Apply();
        }

        public void Configure(Texture2D texture, int newFrameWidth, int newFrameHeight, float newSecondsPerFrame, int newSortingOrder, float newPixelsPerUnit, Vector2 newPivotNormalized, Vector2 newWorldScale, bool newFacePlayer)
        {
            sourceTexture = texture;
            sourceFrames = null;
            frameWidth = newFrameWidth;
            frameHeight = newFrameHeight;
            secondsPerFrame = newSecondsPerFrame;
            sortingOrder = newSortingOrder;
            pixelsPerUnit = newPixelsPerUnit;
            pivotNormalized = newPivotNormalized;
            worldScale = newWorldScale;
            facePlayer = newFacePlayer;
            BuildFrames();
            Apply();
        }

        private void BuildFrames()
        {
            if (sourceFrames != null && sourceFrames.Length > 0)
            {
                frames = new Sprite[sourceFrames.Length];
                Vector2 pivot = new Vector2(Mathf.Clamp01(pivotNormalized.x), Mathf.Clamp01(pivotNormalized.y));
                for (int i = 0; i < sourceFrames.Length; i++)
                {
                    Texture2D texture = sourceFrames[i];
                    if (texture == null)
                    {
                        continue;
                    }

                    frames[i] = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), pivot, pixelsPerUnit);
                }
                frameIndex = 0;
                if (frames.Length > 0 && frames[0] != null)
                {
                    spriteRenderer.sprite = frames[0];
                    nextFrameTime = Time.time + secondsPerFrame;
                }
                return;
            }

            if (sourceTexture == null)
            {
                frames = new Sprite[0];
                return;
            }

            Vector2 sharedPivot = new Vector2(Mathf.Clamp01(pivotNormalized.x), Mathf.Clamp01(pivotNormalized.y));
            if (frameWidth > 0 && frameHeight > 0)
            {
                int count = Mathf.Max(1, sourceTexture.width / frameWidth);
                int rowY = Mathf.Max(0, sourceTexture.height - frameHeight);
                frames = new Sprite[Mathf.Min(count, 2)];
                for (int i = 0; i < frames.Length; i++)
                {
                    int x = i * frameWidth;
                    frames[i] = Sprite.Create(sourceTexture, new Rect(x, rowY, frameWidth, frameHeight), sharedPivot, pixelsPerUnit);
                }
            }
            else
            {
                Vector2 fullFramePivot = sharedPivot;
                frames = new[]
                {
                    Sprite.Create(sourceTexture, new Rect(0f, 0f, sourceTexture.width, sourceTexture.height), fullFramePivot, pixelsPerUnit)
                };
            }

            frameIndex = 0;
            if (frames.Length > 0)
            {
                spriteRenderer.sprite = frames[0];
                nextFrameTime = Time.time + secondsPerFrame;
            }
        }

        private void UpdateFacing()
        {
            if (!facePlayer)
            {
                return;
            }

            if (playerTransform == null)
            {
                playerTransform = FindAnyObjectByType<Heroic.Player.PlayerController>()?.transform;
                if (playerTransform == null)
                {
                    return;
                }
            }

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(worldScale.x) * (playerTransform.position.x < transform.position.x ? -1f : 1f);
            scale.y = worldScale.y;
            transform.localScale = scale;
        }
    }
}
