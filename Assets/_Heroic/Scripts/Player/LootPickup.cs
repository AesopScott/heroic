using System;
using UnityEngine;

namespace Heroic.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class LootPickup : MonoBehaviour
    {
        public enum LootKind
        {
            HealthRestore,
            ExperienceBoost,
            SpeedBoost,
            Invulnerability
        }

        [SerializeField] private LootKind kind = LootKind.HealthRestore;
        [SerializeField] private int value = 5;
        [SerializeField] private int tier = 1;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float multiplier = 1.25f;
        [SerializeField] private float magnetRange = 0.5f;
        [SerializeField] private float magnetSpeed = 9f;
        [SerializeField] private Sprite iconSprite;

        private Transform target;
        private bool collected;

        public event Action<LootPickup> Collected;

        public LootKind Kind => kind;
        public int Value => value;
        public int Tier => tier;
        public float Duration => duration;
        public float Multiplier => multiplier;

        public void Configure(LootKind lootKind, int lootValue, int lootTier, float lootDuration = 0f, float lootMultiplier = 1f)
        {
            kind = lootKind;
            value = Mathf.Max(1, lootValue);
            tier = Mathf.Clamp(lootTier, 1, 5);
            duration = Mathf.Max(0f, lootDuration);
            multiplier = Mathf.Max(1f, lootMultiplier);
            ApplyVisual();
        }

        private void Start()
        {
            ApplyVisual();
        }

        private void Update()
        {
            if (target == null)
            {
                FindTarget();
            }

            if (target == null)
            {
                return;
            }

            if (magnetRange <= 0f || magnetSpeed <= 0f)
            {
                return;
            }

            if (Vector2.Distance(transform.position, target.position) <= magnetRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, magnetSpeed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryCollect(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryCollect(other);
        }

        private void TryCollect(Collider2D other)
        {
            if (collected)
            {
                return;
            }

            if (kind == LootKind.HealthRestore)
            {
                PlayerHealth health = ResolvePlayerComponent<PlayerHealth>(other);
                if (health == null)
                {
                    return;
                }

                health.Heal(value);
            }
            else if (kind == LootKind.ExperienceBoost)
            {
                PlayerTemporaryBuffs buffs = ResolvePlayerComponent<PlayerTemporaryBuffs>(other);
                if (buffs == null)
                {
                    return;
                }

                buffs.ApplyExperienceBoost(multiplier, duration);
            }
            else
            {
                PlayerTemporaryBuffs buffs = ResolvePlayerComponent<PlayerTemporaryBuffs>(other);
                if (buffs == null)
                {
                    return;
                }

                if (kind == LootKind.SpeedBoost)
                {
                    buffs.ApplySpeedBoost(multiplier, duration);
                }
                else
                {
                    buffs.ApplyInvulnerability(duration);
                }
            }

            collected = true;
            Collected?.Invoke(this);
            Destroy(gameObject);
        }

        private static T ResolvePlayerComponent<T>(Collider2D other) where T : Component
        {
            if (other == null)
            {
                return null;
            }

            T component = other.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            component = other.GetComponentInParent<T>();
            if (component != null)
            {
                return component;
            }

            Rigidbody2D body = other.attachedRigidbody;
            if (body == null)
            {
                return null;
            }

            component = body.GetComponent<T>();
            return component != null ? component : body.GetComponentInParent<T>();
        }

        private void FindTarget()
        {
            PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                target = playerHealth.transform;
            }
        }

        private void ApplyVisual()
        {
            ClearVisualLayers();
            Heroic.Visuals.AutoSpriteVisual proceduralVisual = GetComponent<Heroic.Visuals.AutoSpriteVisual>();
            if (proceduralVisual != null)
            {
                proceduralVisual.enabled = false;
            }

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = gameObject.AddComponent<SpriteRenderer>();
            }

            renderer.sprite = iconSprite != null ? iconSprite : LoadRuntimeIcon();
            renderer.color = Color.white;
            renderer.sortingOrder = 34;
            transform.localScale = Vector3.one * Mathf.Lerp(0.24f, 0.3f, tier / 5f);

            if (renderer.sprite == null)
            {
                renderer.enabled = false;
            }
        }

        private Sprite LoadRuntimeIcon()
        {
            string resourcePath = kind switch
            {
                LootKind.HealthRestore => "PickupIcons/pickup-art/pickup_health_potion",
                LootKind.ExperienceBoost => "PickupIcons/pickup-art/pickup_xp_crystal",
                LootKind.SpeedBoost => "PickupIcons/pickup-art/pickup_speed_boot",
                LootKind.Invulnerability => "PickupIcons/pickup-art/pickup_invulnerability_shield",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(resourcePath))
            {
                return null;
            }

            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite != null)
            {
                return sprite;
            }

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            return texture != null
                ? Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), Mathf.Max(texture.width, texture.height))
                : null;
        }

        public void SetIconSprite(Sprite sprite)
        {
            iconSprite = sprite;
            ApplyVisual();
        }

        private void ClearVisualLayers()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (!child.name.StartsWith("LootVisual_"))
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private void OnValidate()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = iconSprite;
            }
        }
    }
}
