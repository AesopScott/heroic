using Heroic.Combat;
using Heroic.Player;
using UnityEngine;
using System.Collections;

namespace Heroic.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HitFlashVisual : MonoBehaviour
    {
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.08f;

        private SpriteRenderer spriteRenderer;
        private Color baseColor;
        private Coroutine flashRoutine;
        private Damageable damageable;
        private PlayerHealth playerHealth;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseColor = spriteRenderer.color;
            damageable = GetComponent<Damageable>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            if (damageable != null)
            {
                damageable.Damaged += HandleDamageableDamaged;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged += HandlePlayerDamaged;
            }
        }

        private void OnDisable()
        {
            if (damageable != null)
            {
                damageable.Damaged -= HandleDamageableDamaged;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged -= HandlePlayerDamaged;
            }
        }

        private void HandleDamageableDamaged(Damageable damaged, int amount)
        {
            Flash();
        }

        private void HandlePlayerDamaged(int amount)
        {
            Flash();
        }

        private void Flash()
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = baseColor;
            flashRoutine = null;
        }
    }
}
