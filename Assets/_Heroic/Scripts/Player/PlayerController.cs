using UnityEngine;

namespace Heroic.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float baseMoveSpeed = 6f;
        [SerializeField] private float lootSpeedMultiplier = 1f;
        [SerializeField] private Vector2 arenaHalfExtents = new Vector2(29.2f, 29.2f);

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private Vector2 lastMoveDirection = Vector2.right;
        private int lastHorizontalFacing = 1;
        private bool movementLocked;
        private float temporarySpeedMultiplier = 1f;

        public Vector2 LastMoveDirection => lastMoveDirection;
        public int LastHorizontalFacing => lastHorizontalFacing;
        public float BaseMoveSpeed => baseMoveSpeed;
        public float CurrentMoveSpeed => baseMoveSpeed * temporarySpeedMultiplier * lootSpeedMultiplier;
        public float LootSpeedMultiplier => lootSpeedMultiplier;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(horizontal, vertical).normalized;
            if (moveInput.sqrMagnitude > 0.001f)
            {
                lastMoveDirection = moveInput;
            }

            if (horizontal > 0.01f)
            {
                lastHorizontalFacing = 1;
            }
            else if (horizontal < -0.01f)
            {
                lastHorizontalFacing = -1;
            }
        }

        private void FixedUpdate()
        {
            if (movementLocked)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = moveInput * CurrentMoveSpeed;
            ClampToArena();
        }

        public void SetMoveSpeed(float newMoveSpeed)
        {
            SetBaseMoveSpeed(newMoveSpeed);
        }

        public void SetBaseMoveSpeed(float newBaseMoveSpeed)
        {
            baseMoveSpeed = Mathf.Max(0f, newBaseMoveSpeed);
        }

        public void SetTemporarySpeedMultiplier(float multiplier)
        {
            temporarySpeedMultiplier = Mathf.Max(0f, multiplier);
        }

        public void SetLootSpeedMultiplier(float multiplier)
        {
            lootSpeedMultiplier = Mathf.Max(0f, multiplier);
        }

        public void SetMovementLocked(bool isLocked)
        {
            movementLocked = isLocked;
        }

        private void ClampToArena()
        {
            if (arenaHalfExtents.x <= 0f || arenaHalfExtents.y <= 0f)
            {
                return;
            }

            Vector2 position = rb.position;
            Vector2 clamped = new Vector2(
                Mathf.Clamp(position.x, -arenaHalfExtents.x, arenaHalfExtents.x),
                Mathf.Clamp(position.y, -arenaHalfExtents.y, arenaHalfExtents.y));

            if ((clamped - position).sqrMagnitude > 0.0001f)
            {
                rb.position = clamped;
                transform.position = new Vector3(clamped.x, clamped.y, transform.position.z);
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
