using UnityEngine;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CrashSpriteAnimator : MonoBehaviour
    {
        [SerializeField] private Texture2D sourceTexture;
        [SerializeField] private int frameWidth = 384;
        [SerializeField] private int frameHeight = 512;
        [SerializeField] private float secondsPerFrame = 0.35f;
        [SerializeField] private int sortingOrder = 20;
        [SerializeField] private float pixelsPerUnit = 384f;
        [SerializeField] private Vector2 pivotNormalized = new Vector2(0.5f, 0.18f);
        [SerializeField] private Vector2 worldScale = new Vector2(1.12f, 1.12f);

        private SpriteRenderer spriteRenderer;
        private Sprite[] frames;
        private int frameIndex;
        private float nextFrameTime;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            BuildFrames();

            spriteRenderer.sortingOrder = sortingOrder;
            transform.localScale = new Vector3(worldScale.x, worldScale.y, 1f);

            if (frames.Length > 0)
            {
                spriteRenderer.sprite = frames[0];
                nextFrameTime = Time.time + secondsPerFrame;
            }
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

        private void BuildFrames()
        {
            if (sourceTexture == null || frameWidth <= 0 || frameHeight <= 0)
            {
                frames = new Sprite[0];
                return;
            }

            int topRowY = sourceTexture.height - frameHeight;
            frames = new[]
            {
                CreateFrame(0, topRowY),
                CreateFrame(frameWidth, topRowY)
            };
        }

        private Sprite CreateFrame(int x, int y)
        {
            Rect rect = new Rect(x, y, frameWidth, frameHeight);
            Vector2 pivot = new Vector2(Mathf.Clamp01(pivotNormalized.x), Mathf.Clamp01(pivotNormalized.y));
            return Sprite.Create(sourceTexture, rect, pivot, pixelsPerUnit);
        }
    }
}
