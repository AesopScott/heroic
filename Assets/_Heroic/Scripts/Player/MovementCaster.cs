using UnityEngine;
using System;
using System.Collections;
using Heroic.Combat;
using Heroic.Visuals;

namespace Heroic.Player
{
    public class MovementCaster : MonoBehaviour
    {
        public enum MovementSkillId
        {
            None,
            Blink,
            Lunge,
            Teleport,
            Whirlwind,
            CloudWalk,
            Invisibility,
            Stoneskin,
            Tunnel,
            Flight
        }

        [Serializable]
        public class MovementSlot
        {
            [SerializeField] private MovementSkillId skill = MovementSkillId.None;
            [SerializeField] private float cooldown = 6f;
            [SerializeField] private float range = 3f;
            [SerializeField] private int damage = 0;

            private float nextReadyTime;

            public MovementSkillId Skill => skill;
            public float Cooldown => cooldown;
            public float Range => range;
            public int Damage => damage;
            public float RemainingCooldown => Mathf.Max(0f, nextReadyTime - Time.time);
            public bool IsReady => Time.time >= nextReadyTime;

            public void Equip(MovementSkillId newSkill)
            {
                skill = newSkill;
                ApplyDefaultsForSkill();
                nextReadyTime = 0f;
            }

            public void StartCooldown()
            {
                nextReadyTime = Time.time + cooldown;
            }

            public void SetDamage(int newDamage)
            {
                damage = Mathf.Max(0, newDamage);
            }

            public void SetCooldown(float newCooldown) => cooldown = Mathf.Max(0.1f, newCooldown);
            public void SetRange(float newRange) => range = Mathf.Max(0f, newRange);

            private void ApplyDefaultsForSkill()
            {
                switch (skill)
                {
                    case MovementSkillId.Blink:
                        cooldown = 5f;
                        range = 3f;
                        damage = 10;
                        break;
                    case MovementSkillId.Lunge:
                        cooldown = 7f;
                        range = 4f;
                        damage = 20;
                        break;
                    case MovementSkillId.Teleport:
                        cooldown = 12f;
                        range = 8f;
                        damage = 0;
                        break;
                    case MovementSkillId.Whirlwind:
                        cooldown = 9f;
                        range = 3f;
                        damage = 12;
                        break;
                    case MovementSkillId.CloudWalk:
                        cooldown = 8f;
                        range = 5f;
                        damage = 0;
                        break;
                    case MovementSkillId.Invisibility:
                        cooldown = 10f;
                        range = 3f;
                        damage = 0;
                        break;
                    case MovementSkillId.Stoneskin:
                        cooldown = 11f;
                        range = 4f;
                        damage = 16;
                        break;
                    case MovementSkillId.Tunnel:
                        cooldown = 10f;
                        range = 5f;
                        damage = 18;
                        break;
                    case MovementSkillId.Flight:
                        cooldown = 10f;
                        range = 6f;
                        damage = 12;
                        break;
                    default:
                        cooldown = 0f;
                        range = 0f;
                        damage = 0;
                        break;
                }
            }
        }

        [SerializeField] private MovementSlot[] movementSlots =
        {
            new MovementSlot(),
            new MovementSlot(),
            new MovementSlot()
        };

        [SerializeField] private LayerMask blockingLayers;
        [SerializeField] private LayerMask damageableLayers;
        [SerializeField] private float collisionCheckRadius = 0.35f;
        [SerializeField] private float lungeDuration = 0.16f;
        [SerializeField] private float lungeHitRadius = 0.6f;
        [SerializeField] private float whirlwindHitRadius = 1.15f;
        [SerializeField] private float whirlwindTickInterval = 0.3f;
        [SerializeField] private float whirlwindSpeedMultiplier = 0.75f;
        [SerializeField] private float whirlwindVisualSpinSpeed = 720f;
        [SerializeField] private float invisibilityDuration = 2.4f;
        [SerializeField] private float invisibilitySpeedMultiplier = 1.2f;
        [SerializeField] private int invisibilityExitDamage;
        [SerializeField] private float stoneskinDuration = 3.2f;
        [SerializeField] private float stoneskinSpeedMultiplier = 0.72f;
        [SerializeField] private float stoneskinPulseInterval = 0.5f;
        [SerializeField] private float tunnelDuration = 0.7f;
        [SerializeField] private float tunnelEruptionRadius = 1.1f;
        [SerializeField] private float flightDuration = 0.55f;
        [SerializeField] private float flightLandingRadius = 0.9f;
        [SerializeField] private bool equipPrototypeMovementSetOnStart = true;

        private PlayerController playerController;
        private CloudWalkController cloudWalkController;
        private PlayerHealth playerHealth;
        private Coroutine activeLunge;
        private Coroutine activeWhirlwind;
        private Coroutine activeInvisibility;
        private Coroutine activeStoneskin;
        private Coroutine activeTunnel;
        private Coroutine activeFlight;
        private int activeSlotIndex;

        public event Action<MovementSkillId> MovementActivated;
        public event Action<int> ActiveSlotChanged;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            cloudWalkController = GetComponent<CloudWalkController>();
            playerHealth = GetComponent<PlayerHealth>();

            if (movementSlots.Length != 3)
            {
                Array.Resize(ref movementSlots, 3);
            }
        }

        private void Start()
        {
            if (!equipPrototypeMovementSetOnStart)
            {
                return;
            }

            if (movementSlots[0].Skill == MovementSkillId.None)
            {
                movementSlots[0].Equip(MovementSkillId.Blink);
            }

            if (movementSlots[1].Skill == MovementSkillId.None)
            {
                movementSlots[1].Equip(MovementSkillId.Lunge);
            }

            if (movementSlots[2].Skill == MovementSkillId.None)
            {
                movementSlots[2].Equip(MovementSkillId.Teleport);
            }

            SelectFirstAvailableSlot();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectSlot(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectSlot(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectSlot(2);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryActivateSlot(activeSlotIndex);
            }

            if (!IsActiveSlotUsable())
            {
                SelectFirstAvailableSlot();
            }
        }

        private void OnDisable()
        {
            if (playerController != null)
            {
                playerController.SetMovementLocked(false);
                playerController.SetTemporarySpeedMultiplier(1f);
            }

            playerHealth?.SetInvulnerable(false);
        }

        public void EquipMovementSkill(int slotIndex, MovementSkillId skillId)
        {
            if (!IsValidSlot(slotIndex))
            {
                return;
            }

            movementSlots[slotIndex].Equip(skillId);
            if (skillId == MovementSkillId.CloudWalk)
            {
                cloudWalkController?.EnableCloudWalk();
            }

            if (!IsActiveSlotEquipped())
            {
                SelectSlot(slotIndex);
            }
        }

        public MovementSkillId GetEquippedSkill(int slotIndex)
        {
            return IsValidSlot(slotIndex) ? movementSlots[slotIndex].Skill : MovementSkillId.None;
        }

        public int GetEquippedMovementSkillCount()
        {
            int count = 0;
            for (int i = 0; i < movementSlots.Length; i++)
            {
                if (IsValidSlot(i) && movementSlots[i].Skill != MovementSkillId.None)
                {
                    count++;
                }
            }

            return count;
        }

        public MovementSkillId GetDisplayedMovementSkill(int displayIndex)
        {
            int actualSlot = ResolveDisplayedSlotIndex(displayIndex);
            return actualSlot >= 0 ? movementSlots[actualSlot].Skill : MovementSkillId.None;
        }

        public float GetDisplayedRemainingCooldown(int displayIndex)
        {
            int actualSlot = ResolveDisplayedSlotIndex(displayIndex);
            return actualSlot >= 0 ? movementSlots[actualSlot].RemainingCooldown : 0f;
        }

        public float GetDisplayedCooldown(int displayIndex)
        {
            int actualSlot = ResolveDisplayedSlotIndex(displayIndex);
            return actualSlot >= 0 ? movementSlots[actualSlot].Cooldown : 0f;
        }

        public bool IsDisplayedSkillActive(int displayIndex)
        {
            int actualSlot = ResolveDisplayedSlotIndex(displayIndex);
            return actualSlot >= 0 && actualSlot == activeSlotIndex;
        }

        public float GetRemainingCooldown(int slotIndex)
        {
            return IsValidSlot(slotIndex) ? movementSlots[slotIndex].RemainingCooldown : 0f;
        }

        public float GetCooldown(int slotIndex)
        {
            return IsValidSlot(slotIndex) ? movementSlots[slotIndex].Cooldown : 0f;
        }

        public int GetActiveSlotIndex()
        {
            return activeSlotIndex;
        }

        public bool IsSlotActive(int slotIndex)
        {
            return slotIndex == activeSlotIndex;
        }

        private void SelectSlot(int slotIndex)
        {
            if (!IsValidSlot(slotIndex) || movementSlots[slotIndex].Skill == MovementSkillId.None)
            {
                return;
            }

            if (activeSlotIndex == slotIndex)
            {
                return;
            }

            activeSlotIndex = slotIndex;
            ActiveSlotChanged?.Invoke(activeSlotIndex);
        }

        private void TryActivateSlot(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
            {
                return;
            }

            MovementSlot slot = movementSlots[slotIndex];
            if (slot.Skill == MovementSkillId.None || !slot.IsReady)
            {
                return;
            }

            bool activated = Activate(slot);
            if (activated)
            {
                slot.StartCooldown();
                MovementActivated?.Invoke(slot.Skill);
                SelectFirstAvailableSlot();
            }
        }

        private void SelectFirstAvailableSlot()
        {
            for (int i = 0; i < movementSlots.Length; i++)
            {
                if (IsValidSlot(i) && movementSlots[i].Skill != MovementSkillId.None && movementSlots[i].IsReady)
                {
                    SelectSlot(i);
                    return;
                }
            }
        }

        private bool IsActiveSlotEquipped()
        {
            return IsValidSlot(activeSlotIndex) && movementSlots[activeSlotIndex].Skill != MovementSkillId.None;
        }

        private bool IsActiveSlotUsable()
        {
            return IsValidSlot(activeSlotIndex) && movementSlots[activeSlotIndex].Skill != MovementSkillId.None && movementSlots[activeSlotIndex].IsReady;
        }

        private bool Activate(MovementSlot slot)
        {
            switch (slot.Skill)
            {
                case MovementSkillId.Blink:
                    return Blink(slot);
                case MovementSkillId.Lunge:
                    return Lunge(slot);
                case MovementSkillId.Teleport:
                    return Teleport(slot);
                case MovementSkillId.Whirlwind:
                    return Whirlwind(slot);
                case MovementSkillId.CloudWalk:
                    return CloudWalk(slot);
                case MovementSkillId.Invisibility:
                    return Invisibility(slot);
                case MovementSkillId.Stoneskin:
                    return Stoneskin(slot);
                case MovementSkillId.Tunnel:
                    return Tunnel(slot);
                case MovementSkillId.Flight:
                    return Flight(slot);
                default:
                    return false;
            }
        }

        private bool Blink(MovementSlot slot)
        {
            Vector2 direction = GetFacingDirection();
            Vector2 destination = FindValidDestination(transform.position, direction, slot.Range);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.35f, 0.9f, 1f, 0.4f), 0.8f, 0.16f);
            transform.position = destination;
            TemporaryVisualEffect.CreateCircle(destination, new Color(0.35f, 0.9f, 1f, 0.5f), 0.9f, 0.18f);
            DamageAround(destination, slot.Damage, 0.75f);
            return true;
        }

        private bool Lunge(MovementSlot slot)
        {
            if (activeLunge != null)
            {
                return false;
            }

            Vector2 direction = GetFacingDirection();
            Vector2 destination = FindValidDestination(transform.position, direction, slot.Range);
            activeLunge = StartCoroutine(LungeRoutine(destination, slot.Damage));
            return true;
        }

        private bool Teleport(MovementSlot slot)
        {
            Vector2 direction = GetFacingDirection();
            Vector2 destination = FindValidDestination(transform.position, direction, slot.Range);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.8f, 0.95f, 1f, 0.35f), 0.75f, 0.14f);
            transform.position = destination;
            TemporaryVisualEffect.CreateCircle(destination, new Color(0.8f, 0.95f, 1f, 0.45f), 1f, 0.2f);
            return true;
        }

        private bool Whirlwind(MovementSlot slot)
        {
            if (activeWhirlwind != null)
            {
                return false;
            }

            activeWhirlwind = StartCoroutine(WhirlwindRoutine(slot.Range, slot.Damage));
            return true;
        }

        private bool CloudWalk(MovementSlot slot)
        {
            if (cloudWalkController == null)
            {
                return false;
            }

            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.72f, 1f, 0.9f, 0.24f), 1f, 0.14f);
            cloudWalkController.BeginCloudWalk(slot.Range);
            return true;
        }

        private bool Invisibility(MovementSlot slot)
        {
            if (activeInvisibility != null)
            {
                return false;
            }

            activeInvisibility = StartCoroutine(InvisibilityRoutine(slot));
            return true;
        }

        private bool Stoneskin(MovementSlot slot)
        {
            if (activeStoneskin != null)
            {
                return false;
            }

            activeStoneskin = StartCoroutine(StoneskinRoutine(slot));
            return true;
        }

        private bool Tunnel(MovementSlot slot)
        {
            if (activeTunnel != null)
            {
                return false;
            }

            Vector2 direction = GetFacingDirection();
            Vector2 destination = FindValidDestination(transform.position, direction, slot.Range);
            activeTunnel = StartCoroutine(TunnelRoutine(destination, slot.Damage));
            return true;
        }

        private bool Flight(MovementSlot slot)
        {
            if (activeFlight != null)
            {
                return false;
            }

            Vector2 direction = GetFacingDirection();
            Vector2 destination = (Vector2)transform.position + direction.normalized * slot.Range;
            activeFlight = StartCoroutine(FlightRoutine(destination, slot.Damage));
            return true;
        }

        public void SetCloudWalkStandardMovementTier(int tier)
        {
            cloudWalkController?.SetStandardMovementTier(tier);
        }

        public void SetCloudWalkPickupRangeTier(int tier)
        {
            cloudWalkController?.SetPickupRangeTier(tier);
        }

        public void SetCloudWalkKnockbackTier(int tier)
        {
            cloudWalkController?.SetKnockbackTier(tier);
        }

        public void SetWhirlwindGaleTier(int tier)
        {
            int clampedTier = Mathf.Clamp(tier, 0, 5);
            float[] speedMultipliers = { 0.75f, 0.9f, 1.05f, 1.2f, 1.35f, 1.5f };
            int[] damages = { 12, 16, 21, 27, 34, 42 };

            whirlwindSpeedMultiplier = speedMultipliers[clampedTier];
            SetEquippedMovementDamage(MovementSkillId.Whirlwind, damages[clampedTier]);
        }

        public void SetWhirlwindRadiusTier(int tier)
        {
            whirlwindHitRadius = Value(tier, 1.35f, 1.55f, 1.8f, 2.1f, 2.5f);
        }

        public void SetMovementRangeTier(MovementSkillId skillId, int tier)
        {
            float multiplier = Value(tier, 1.12f, 1.25f, 1.4f, 1.6f, 1.85f);
            foreach (MovementSlot slot in movementSlots)
            {
                if (slot != null && slot.Skill == skillId)
                {
                    slot.SetRange(DefaultRange(skillId) * multiplier);
                }
            }
        }

        public void SetMovementCooldownTier(MovementSkillId skillId, int tier)
        {
            float multiplier = Value(tier, 0.9f, 0.82f, 0.74f, 0.66f, 0.58f);
            foreach (MovementSlot slot in movementSlots)
            {
                if (slot != null && slot.Skill == skillId)
                {
                    slot.SetCooldown(DefaultCooldown(skillId) * multiplier);
                }
            }
        }

        public void SetMovementDamageTier(MovementSkillId skillId, int tier)
        {
            int damage = Mathf.RoundToInt(DefaultDamage(skillId) * Value(tier, 1.3f, 1.65f, 2f, 2.4f, 2.9f));
            SetEquippedMovementDamage(skillId, damage);
        }

        public void SetInvisibilityDurationTier(int tier) => invisibilityDuration = Value(tier, 3f, 3.6f, 4.3f, 5.1f, 6f);
        public void SetInvisibilitySpeedTier(int tier) => invisibilitySpeedMultiplier = Value(tier, 1.28f, 1.38f, 1.5f, 1.65f, 1.85f);
        public void SetInvisibilityExitDamageTier(int tier) => invisibilityExitDamage = Value(tier, 16, 24, 34, 48, 66);
        public void SetStoneskinDurationTier(int tier) => stoneskinDuration = Value(tier, 3.8f, 4.6f, 5.5f, 6.6f, 8f);
        public void SetStoneskinSpeedTier(int tier) => stoneskinSpeedMultiplier = Value(tier, 0.78f, 0.86f, 0.95f, 1.05f, 1.18f);
        public void SetStoneskinPulseDamageTier(int tier) => SetMovementDamageTier(MovementSkillId.Stoneskin, tier);
        public void SetTunnelDurationTier(int tier) => tunnelDuration = Value(tier, 0.85f, 1f, 1.18f, 1.38f, 1.65f);
        public void SetTunnelEruptionRadiusTier(int tier) => tunnelEruptionRadius = Value(tier, 1.25f, 1.45f, 1.7f, 2f, 2.4f);
        public void SetFlightDurationTier(int tier) => flightDuration = Value(tier, 0.48f, 0.42f, 0.36f, 0.3f, 0.24f);
        public void SetFlightLandingRadiusTier(int tier) => flightLandingRadius = Value(tier, 1.05f, 1.25f, 1.5f, 1.85f, 2.25f);

        private IEnumerator LungeRoutine(Vector2 destination, int damage)
        {
            Vector2 start = transform.position;
            float elapsed = 0f;
            if (playerController != null)
            {
                playerController.SetMovementLocked(true);
            }

            while (elapsed < lungeDuration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / lungeDuration);
                transform.position = Vector2.Lerp(start, destination, percent);
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.65f, 0.95f, 1f, 0.2f), 0.45f, 0.08f);
                DamageAround(transform.position, damage, lungeHitRadius);
                yield return null;
            }

            transform.position = destination;
            if (playerController != null)
            {
                playerController.SetMovementLocked(false);
            }

            activeLunge = null;
        }

        private IEnumerator WhirlwindRoutine(float duration, int damage)
        {
            float elapsed = 0f;
            float nextTickTime = 0f;
            SpinningWhirlwindVisual.Attach(transform, whirlwindHitRadius, duration, whirlwindVisualSpinSpeed);

            if (playerController != null)
            {
                playerController.SetTemporarySpeedMultiplier(whirlwindSpeedMultiplier);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (Time.time >= nextTickTime)
                {
                    DamageAround(transform.position, damage, whirlwindHitRadius);
                    TemporaryVisualEffect.CreateCircle(transform.position, new Color(1f, 0.58f, 0.14f, 0.18f), whirlwindHitRadius, 0.08f);
                    nextTickTime = Time.time + whirlwindTickInterval;
                }

                yield return null;
            }

            TemporaryVisualEffect.CreateCircle(transform.position, new Color(1f, 0.42f, 0.08f, 0.32f), whirlwindHitRadius * 1.2f, 0.16f);
            if (playerController != null)
            {
                playerController.SetTemporarySpeedMultiplier(1f);
            }

            activeWhirlwind = null;
        }

        private IEnumerator InvisibilityRoutine(MovementSlot slot)
        {
            float elapsed = 0f;
            playerHealth?.SetInvulnerable(true);
            playerController?.SetTemporarySpeedMultiplier(invisibilitySpeedMultiplier);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.55f, 0.65f, 1f, 0.22f), 1f, 0.18f);

            while (elapsed < invisibilityDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            playerHealth?.SetInvulnerable(false);
            playerController?.SetTemporarySpeedMultiplier(1f);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.55f, 0.65f, 1f, 0.34f), 1.1f, 0.18f);
            DamageAround(transform.position, invisibilityExitDamage, Mathf.Max(0.75f, slot.Range * 0.35f));
            activeInvisibility = null;
        }

        private IEnumerator StoneskinRoutine(MovementSlot slot)
        {
            float elapsed = 0f;
            float nextPulseAt = 0f;
            playerHealth?.SetInvulnerable(true);
            playerController?.SetTemporarySpeedMultiplier(stoneskinSpeedMultiplier);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.62f, 0.52f, 0.36f, 0.34f), 1.1f, 0.18f);

            while (elapsed < stoneskinDuration)
            {
                elapsed += Time.deltaTime;
                if (Time.time >= nextPulseAt)
                {
                    DamageAround(transform.position, slot.Damage, Mathf.Max(1f, slot.Range * 0.35f));
                    TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.62f, 0.52f, 0.36f, 0.22f), Mathf.Max(1f, slot.Range * 0.35f), 0.12f);
                    nextPulseAt = Time.time + stoneskinPulseInterval;
                }

                yield return null;
            }

            playerHealth?.SetInvulnerable(false);
            playerController?.SetTemporarySpeedMultiplier(1f);
            activeStoneskin = null;
        }

        private IEnumerator TunnelRoutine(Vector2 destination, int damage)
        {
            Vector2 start = transform.position;
            float elapsed = 0f;
            playerHealth?.SetInvulnerable(true);
            playerController?.SetMovementLocked(true);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.42f, 0.28f, 0.14f, 0.3f), 0.9f, 0.16f);

            while (elapsed < tunnelDuration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / tunnelDuration);
                transform.position = Vector2.Lerp(start, destination, percent);
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.36f, 0.22f, 0.12f, 0.18f), 0.55f, 0.08f);
                yield return null;
            }

            transform.position = destination;
            DamageAround(destination, damage, tunnelEruptionRadius);
            TemporaryVisualEffect.CreateCircle(destination, new Color(0.5f, 0.32f, 0.14f, 0.34f), tunnelEruptionRadius, 0.18f);
            playerHealth?.SetInvulnerable(false);
            playerController?.SetMovementLocked(false);
            activeTunnel = null;
        }

        private IEnumerator FlightRoutine(Vector2 destination, int damage)
        {
            Vector2 start = transform.position;
            float elapsed = 0f;
            playerHealth?.SetInvulnerable(true);
            playerController?.SetMovementLocked(true);
            TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.88f, 0.96f, 1f, 0.28f), 0.95f, 0.14f);

            while (elapsed < flightDuration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / flightDuration);
                float eased = Mathf.Sin(percent * Mathf.PI * 0.5f);
                transform.position = Vector2.Lerp(start, destination, eased);
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(0.82f, 0.94f, 1f, 0.18f), 0.5f, 0.07f);
                yield return null;
            }

            transform.position = destination;
            DamageAround(destination, damage, flightLandingRadius);
            TemporaryVisualEffect.CreateCircle(destination, new Color(0.88f, 0.96f, 1f, 0.34f), flightLandingRadius, 0.16f);
            playerHealth?.SetInvulnerable(false);
            playerController?.SetMovementLocked(false);
            activeFlight = null;
        }

        private Vector2 FindValidDestination(Vector2 origin, Vector2 direction, float range)
        {
            Vector2 destination = origin + direction.normalized * range;
            RaycastHit2D hit = Physics2D.CircleCast(origin, collisionCheckRadius, direction, range, blockingLayers);
            if (hit.collider != null)
            {
                destination = hit.point - direction.normalized * collisionCheckRadius;
            }

            return destination;
        }

        private void DamageAround(Vector2 center, int damage, float radius)
        {
            if (damage <= 0)
            {
                return;
            }

            Collider2D[] hits = damageableLayers.value == 0
                ? Physics2D.OverlapCircleAll(center, radius)
                : Physics2D.OverlapCircleAll(center, radius, damageableLayers);
            foreach (Collider2D hit in hits)
            {
                var damageable = hit.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(damage);
                }
            }
        }

        private Vector2 GetFacingDirection()
        {
            if (playerController != null && playerController.LastMoveDirection.sqrMagnitude > 0.001f)
            {
                return playerController.LastMoveDirection;
            }

            return Vector2.right;
        }

        private bool IsValidSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < movementSlots.Length && movementSlots[slotIndex] != null;
        }

        private int ResolveDisplayedSlotIndex(int displayIndex)
        {
            if (displayIndex < 0)
            {
                return -1;
            }

            int seen = 0;
            for (int i = 0; i < movementSlots.Length; i++)
            {
                if (!IsValidSlot(i) || movementSlots[i].Skill == MovementSkillId.None)
                {
                    continue;
                }

                if (seen == displayIndex)
                {
                    return i;
                }

                seen++;
            }

            return -1;
        }

        private void SetEquippedMovementDamage(MovementSkillId skillId, int damage)
        {
            foreach (MovementSlot slot in movementSlots)
            {
                if (slot != null && slot.Skill == skillId)
                {
                    slot.SetDamage(damage);
                }
            }
        }

        private static float DefaultCooldown(MovementSkillId skillId)
        {
            switch (skillId)
            {
                case MovementSkillId.Blink:
                    return 5f;
                case MovementSkillId.Lunge:
                    return 7f;
                case MovementSkillId.Teleport:
                    return 12f;
                case MovementSkillId.Whirlwind:
                    return 9f;
                case MovementSkillId.CloudWalk:
                    return 8f;
                case MovementSkillId.Invisibility:
                    return 10f;
                case MovementSkillId.Stoneskin:
                    return 11f;
                case MovementSkillId.Tunnel:
                    return 10f;
                case MovementSkillId.Flight:
                    return 10f;
                default:
                    return 1f;
            }
        }

        private static float DefaultRange(MovementSkillId skillId)
        {
            switch (skillId)
            {
                case MovementSkillId.Blink:
                    return 3f;
                case MovementSkillId.Lunge:
                    return 4f;
                case MovementSkillId.Teleport:
                    return 8f;
                case MovementSkillId.Whirlwind:
                    return 3f;
                case MovementSkillId.CloudWalk:
                    return 5f;
                case MovementSkillId.Invisibility:
                    return 3f;
                case MovementSkillId.Stoneskin:
                    return 4f;
                case MovementSkillId.Tunnel:
                    return 5f;
                case MovementSkillId.Flight:
                    return 6f;
                default:
                    return 0f;
            }
        }

        private static int DefaultDamage(MovementSkillId skillId)
        {
            switch (skillId)
            {
                case MovementSkillId.Blink:
                    return 10;
                case MovementSkillId.Lunge:
                    return 20;
                case MovementSkillId.Teleport:
                    return 18;
                case MovementSkillId.Whirlwind:
                    return 12;
                case MovementSkillId.Stoneskin:
                    return 16;
                case MovementSkillId.Tunnel:
                    return 18;
                case MovementSkillId.Flight:
                    return 12;
                default:
                    return 0;
            }
        }

        private static int Value(int tier, int basic, int advanced, int expert, int master, int grandmaster)
        {
            switch (Mathf.Clamp(tier, 1, 5))
            {
                case 1:
                    return basic;
                case 2:
                    return advanced;
                case 3:
                    return expert;
                case 4:
                    return master;
                default:
                    return grandmaster;
            }
        }

        private static float Value(int tier, float basic, float advanced, float expert, float master, float grandmaster)
        {
            switch (Mathf.Clamp(tier, 1, 5))
            {
                case 1:
                    return basic;
                case 2:
                    return advanced;
                case 3:
                    return expert;
                case 4:
                    return master;
                default:
                    return grandmaster;
            }
        }
    }
}
