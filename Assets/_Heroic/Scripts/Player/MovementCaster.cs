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
            CloudWalk
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
        [SerializeField] private bool equipPrototypeMovementSetOnStart = true;

        private PlayerController playerController;
        private CloudWalkController cloudWalkController;
        private Coroutine activeLunge;
        private Coroutine activeWhirlwind;
        private int activeSlotIndex;

        public event Action<MovementSkillId> MovementActivated;
        public event Action<int> ActiveSlotChanged;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            cloudWalkController = GetComponent<CloudWalkController>();

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
    }
}
