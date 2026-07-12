using UnityEngine;

namespace Heroic.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float baseMoveSpeed = 6f;
        [SerializeField] private float lootSpeedMultiplier = 1f;

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
    }
}
