using UnityEngine;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TextureSpriteVisual : MonoBehaviour
    {
        [SerializeField] private Texture2D sourceTexture;
        [SerializeField] private int sortingOrder = 10;
        [SerializeField] private float pixelsPerUnit = 384f;
        [SerializeField] private Vector2 pivotNormalized = new Vector2(0.5f, 0.18f);
        [SerializeField] private Vector2 worldScale = new Vector2(1f, 1f);

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Apply();
        }

        public void Apply()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (sourceTexture != null)
            {
                Vector2 pivot = new Vector2(Mathf.Clamp01(pivotNormalized.x), Mathf.Clamp01(pivotNormalized.y));
                spriteRenderer.sprite = Sprite.Create(sourceTexture, new Rect(0f, 0f, sourceTexture.width, sourceTexture.height), pivot, pixelsPerUnit);
            }

            spriteRenderer.sortingOrder = sortingOrder;
            transform.localScale = new Vector3(worldScale.x, worldScale.y, 1f);
        }

        public void Configure(Texture2D texture, int newSortingOrder, float newPixelsPerUnit, Vector2 newPivotNormalized, Vector2 newWorldScale)
        {
            sourceTexture = texture;
            sortingOrder = newSortingOrder;
            pixelsPerUnit = newPixelsPerUnit;
            pivotNormalized = newPivotNormalized;
            worldScale = newWorldScale;
            Apply();
        }
    }
}
