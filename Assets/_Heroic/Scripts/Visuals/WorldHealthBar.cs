using Heroic.Combat;
using Heroic.Player;
using UnityEngine;

namespace Heroic.Visuals
{
    public class WorldHealthBar : MonoBehaviour
    {
        [SerializeField] private Vector2 offset = new Vector2(0f, 0.78f);
        [SerializeField] private Vector2 size = new Vector2(1.1f, 0.09f);
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private Color backgroundColor = new Color(0.02f, 0.04f, 0.05f, 0.82f);
        [SerializeField] private Color fillColor = new Color(0.22f, 0.95f, 0.68f, 0.95f);

        private Damageable damageable;
        private PlayerHealth playerHealth;
        private Transform fillTransform;
        private GameObject root;

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
            playerHealth = GetComponent<PlayerHealth>();
            CreateBar();
            Refresh();
        }

        private void OnEnable()
        {
            if (damageable != null)
            {
                damageable.Damaged += HandleDamageableChanged;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged += HandlePlayerChanged;
            }
        }

        private void OnDisable()
        {
            if (damageable != null)
            {
                damageable.Damaged -= HandleDamageableChanged;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged -= HandlePlayerChanged;
            }
        }

        private void LateUpdate()
        {
            Refresh();
        }

        public void Configure(Vector2 newOffset, Vector2 newSize, bool shouldHideWhenFull, Color newFillColor)
        {
            offset = newOffset;
            size = newSize;
            hideWhenFull = shouldHideWhenFull;
            fillColor = newFillColor;
            if (root != null)
            {
                Destroy(root);
            }

            CreateBar();
            Refresh();
        }

        private void CreateBar()
        {
            root = new GameObject("WorldHealthBar");
            root.transform.SetParent(transform, false);
            root.transform.localPosition = offset;

            SpriteRenderer background = CreateSegment("Background", backgroundColor, root.transform, 4);
            background.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer fill = CreateSegment("Fill", fillColor, root.transform, 5);
            fill.transform.localPosition = new Vector3(-size.x * 0.5f, 0f, -0.01f);
            fill.transform.localScale = new Vector3(size.x, size.y, 1f);
            fillTransform = fill.transform;
        }

        private SpriteRenderer CreateSegment(string name, Color color, Transform parent, int sortingOrder)
        {
            GameObject segment = new GameObject(name);
            segment.transform.SetParent(parent, false);
            SpriteRenderer spriteRenderer = segment.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = ProceduralSpriteFactory.GetSolid(name, color);
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
            return spriteRenderer;
        }

        private void Refresh()
        {
            if (root == null || fillTransform == null)
            {
                return;
            }

            float percent = GetHealthPercent();
            root.SetActive(!hideWhenFull || percent < 0.999f);
            fillTransform.localScale = new Vector3(size.x * percent, size.y, 1f);
            fillTransform.localPosition = new Vector3(-size.x * 0.5f + size.x * percent * 0.5f, 0f, -0.01f);
        }

        private float GetHealthPercent()
        {
            if (damageable != null)
            {
                return damageable.MaxHealth > 0 ? Mathf.Clamp01(damageable.CurrentHealth / (float)damageable.MaxHealth) : 0f;
            }

            if (playerHealth != null)
            {
                return playerHealth.MaxHealth > 0 ? Mathf.Clamp01(playerHealth.CurrentHealth / (float)playerHealth.MaxHealth) : 0f;
            }

            return 1f;
        }

        private void HandleDamageableChanged(Damageable target, int amount)
        {
            Refresh();
        }

        private void HandlePlayerChanged(int amount)
        {
            Refresh();
        }
    }
}
