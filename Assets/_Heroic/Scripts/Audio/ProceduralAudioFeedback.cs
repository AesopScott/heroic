using Heroic.Combat;
using Heroic.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Heroic.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class ProceduralAudioFeedback : MonoBehaviour
    {
        public enum Preset
        {
            Player,
            Enemy,
            Boss,
            Pickup,
            Movement
        }

        [SerializeField] private Preset preset;
        [SerializeField] private float volume = 0.45f;
        [SerializeField] private float minimumGlobalInterval = 0.035f;

        private static readonly Dictionary<string, float> LastPlayedAtByKey = new Dictionary<string, float>();

        private AudioSource source;
        private Damageable damageable;
        private PlayerHealth playerHealth;
        private ExperiencePickup pickup;
        private MovementCaster movementCaster;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0.25f;
            source.volume = volume;

            damageable = GetComponent<Damageable>();
            playerHealth = GetComponent<PlayerHealth>();
            pickup = GetComponent<ExperiencePickup>();
            movementCaster = GetComponent<MovementCaster>();
        }

        private void OnEnable()
        {
            if (damageable != null)
            {
                damageable.Damaged += HandleDamageableDamaged;
                damageable.Died += HandleDamageableDied;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged += HandlePlayerDamaged;
                playerHealth.Died += HandlePlayerDied;
            }

            if (pickup != null)
            {
                pickup.Collected += HandlePickupCollected;
            }

            if (movementCaster != null)
            {
                movementCaster.MovementActivated += HandleMovementActivated;
            }
        }

        private void OnDisable()
        {
            if (damageable != null)
            {
                damageable.Damaged -= HandleDamageableDamaged;
                damageable.Died -= HandleDamageableDied;
            }

            if (playerHealth != null)
            {
                playerHealth.Damaged -= HandlePlayerDamaged;
                playerHealth.Died -= HandlePlayerDied;
            }

            if (pickup != null)
            {
                pickup.Collected -= HandlePickupCollected;
            }

            if (movementCaster != null)
            {
                movementCaster.MovementActivated -= HandleMovementActivated;
            }
        }

        private void HandleDamageableDamaged(Damageable target, int amount)
        {
            Play(preset == Preset.Boss ? "boss_hit" : "enemy_hit", preset == Preset.Boss ? 150f : 240f, 0.06f, preset == Preset.Boss ? 80f : 30f);
        }

        private void HandleDamageableDied(Damageable target)
        {
            Play(preset == Preset.Boss ? "boss_death" : "enemy_death", preset == Preset.Boss ? 110f : 180f, preset == Preset.Boss ? 0.38f : 0.16f, preset == Preset.Boss ? 90f : 100f);
        }

        private void HandlePlayerDamaged(int amount)
        {
            Play("player_hit", 130f, 0.12f, 40f);
        }

        private void HandlePlayerDied()
        {
            Play("player_death", 95f, 0.32f, 70f);
        }

        private void HandlePickupCollected(ExperiencePickup collectedPickup)
        {
            if (!CanPlay("xp_pickup"))
            {
                return;
            }

            AudioClip clip = ProceduralAudio.Tone("xp_pickup", 760f, 0.08f, volume, -120f);
            AudioSource.PlayClipAtPoint(clip, collectedPickup.transform.position, volume);
        }

        private void HandleMovementActivated(MovementCaster.MovementSkillId skill)
        {
            switch (skill)
            {
                case MovementCaster.MovementSkillId.Blink:
                    Play("move_blink", 620f, 0.08f, -140f);
                    break;
                case MovementCaster.MovementSkillId.Lunge:
                    Play("move_lunge", 420f, 0.1f, 120f);
                    break;
                case MovementCaster.MovementSkillId.Teleport:
                    Play("move_teleport", 520f, 0.16f, -260f);
                    break;
            }
        }

        private void Play(string key, float frequency, float duration, float descend)
        {
            if (source == null || !CanPlay(key))
            {
                return;
            }

            AudioClip clip = ProceduralAudio.Tone(key, frequency, duration, volume, descend);
            source.PlayOneShot(clip, volume);
        }

        private bool CanPlay(string key)
        {
            float now = Time.unscaledTime;
            if (LastPlayedAtByKey.TryGetValue(key, out float lastPlayedAt) && now - lastPlayedAt < minimumGlobalInterval)
            {
                return false;
            }

            LastPlayedAtByKey[key] = now;
            return true;
        }
    }
}
