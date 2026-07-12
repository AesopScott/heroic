using UnityEngine;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AutoSpriteVisual : MonoBehaviour
    {
        public enum Shape
        {
            Circle,
            Diamond,
            Ring,
            Triangle
        }

        [SerializeField] private Shape shape = Shape.Circle;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Vector2 size = Vector2.one;
        [SerializeField] private int sortingOrder;
        [SerializeField] private bool pulse;
        [SerializeField] private float pulseAmount = 0.08f;
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private bool rotate;
        [SerializeField] private float rotationSpeed = 90f;

        private SpriteRenderer spriteRenderer;
        private Vector3 baseScale;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Apply();
        }

        private void Update()
        {
            if (pulse)
            {
                float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                transform.localScale = new Vector3(baseScale.x * scale, baseScale.y * scale, baseScale.z);
            }

            if (rotate)
            {
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }

        public void Apply()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = shape switch
            {
                Shape.Circle => ProceduralSpriteFactory.GetCircle(gameObject.name, color),
                Shape.Ring => ProceduralSpriteFactory.GetRing(gameObject.name, color),
                Shape.Triangle => ProceduralSpriteFactory.GetTriangle(gameObject.name, color),
                _ => ProceduralSpriteFactory.GetDiamond(gameObject.name, color)
            };

            spriteRenderer.sortingOrder = sortingOrder;
            transform.localScale = new Vector3(size.x, size.y, 1f);
            baseScale = transform.localScale;
        }

        public void Configure(Shape newShape, Color newColor, Vector2 newSize, int newSortingOrder, bool shouldPulse, bool shouldRotate, float newPulseAmount = 0.08f, float newPulseSpeed = 3f, float newRotationSpeed = 90f)
        {
            shape = newShape;
            color = newColor;
            size = newSize;
            sortingOrder = newSortingOrder;
            pulse = shouldPulse;
            rotate = shouldRotate;
            pulseAmount = newPulseAmount;
            pulseSpeed = newPulseSpeed;
            rotationSpeed = newRotationSpeed;
            Apply();
        }
    }
}
