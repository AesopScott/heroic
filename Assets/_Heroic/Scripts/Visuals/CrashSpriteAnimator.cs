using UnityEngine;
using Heroic.Player;

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
        [SerializeField] private bool facePlayer = true;

        private SpriteRenderer spriteRenderer;
        private Sprite[] frames;
        private int frameIndex;
        private float nextFrameTime;
        private Transform playerTransform;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerTransform = FindAnyObjectByType<PlayerController>()?.transform;
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
                UpdateFacing();
                return;
            }

            frameIndex = (frameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[frameIndex];
            nextFrameTime = Time.time + secondsPerFrame;
            UpdateFacing();
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

        private void UpdateFacing()
        {
            if (!facePlayer)
            {
                return;
            }

            if (playerTransform == null)
            {
                playerTransform = FindAnyObjectByType<PlayerController>()?.transform;
                if (playerTransform == null)
                {
                    return;
                }
            }

            Vector3 scale = transform.localScale;
            float direction = playerTransform.position.x < transform.position.x ? -1f : 1f;
            scale.x = Mathf.Abs(worldScale.x) * direction;
            scale.y = worldScale.y;
            transform.localScale = scale;
        }
    }
}
