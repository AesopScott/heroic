using UnityEngine;

namespace Heroic.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private Vector2 lastMoveDirection = Vector2.right;
        private bool movementLocked;

        public Vector2 LastMoveDirection => lastMoveDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (moveInput.sqrMagnitude > 0.001f)
            {
                lastMoveDirection = moveInput;
            }
        }

        private void FixedUpdate()
        {
            if (movementLocked)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = moveInput * moveSpeed;
        }

        public void SetMoveSpeed(float newMoveSpeed)
        {
            moveSpeed = newMoveSpeed;
        }

        public void SetMovementLocked(bool isLocked)
        {
            movementLocked = isLocked;
        }
    }
}
