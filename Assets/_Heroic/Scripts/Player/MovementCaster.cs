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
            Whirlwind
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
                        range = 5f;
                        damage = 18;
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
        [SerializeField] private bool equipPrototypeMovementSetOnStart = true;

        private PlayerController playerController;
        private Coroutine activeLunge;

        public event Action<MovementSkillId> MovementActivated;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();

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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryActivateSlot(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TryActivateSlot(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TryActivateSlot(2);
            }
        }

        private void OnDisable()
        {
            if (playerController != null)
            {
                playerController.SetMovementLocked(false);
            }
        }

        public void EquipMovementSkill(int slotIndex, MovementSkillId skillId)
        {
            if (!IsValidSlot(slotIndex))
            {
                return;
            }

            movementSlots[slotIndex].Equip(skillId);
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
            }
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
            if (activeLunge != null)
            {
                return false;
            }

            Vector2 direction = GetFacingDirection();
            Vector2 destination = FindValidDestination(transform.position, direction, slot.Range);
            activeLunge = StartCoroutine(WhirlwindRoutine(destination, slot.Damage));
            return true;
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

        private IEnumerator WhirlwindRoutine(Vector2 destination, int damage)
        {
            Vector2 start = transform.position;
            float elapsed = 0f;
            float duration = lungeDuration * 1.45f;
            if (playerController != null)
            {
                playerController.SetMovementLocked(true);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector2.Lerp(start, destination, percent);
                TemporaryVisualEffect.CreateCircle(transform.position, new Color(1f, 0.58f, 0.14f, 0.26f), 0.75f, 0.08f);
                DamageAround(transform.position, damage, lungeHitRadius * 1.25f);
                yield return null;
            }

            transform.position = destination;
            TemporaryVisualEffect.CreateCircle(destination, new Color(1f, 0.42f, 0.08f, 0.38f), 1.1f, 0.16f);
            if (playerController != null)
            {
                playerController.SetMovementLocked(false);
            }

            activeLunge = null;
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
    }
}
