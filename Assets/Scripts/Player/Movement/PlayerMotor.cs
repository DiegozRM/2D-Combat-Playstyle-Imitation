using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private GroundDetector groundDetector;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField, Min(0f)] private float jumpSpeed = 10f;

    private Rigidbody2D body;
    private PlayerKnockback knockback;

    public int FacingDirection { get; private set; } = 1;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        knockback = GetComponent<PlayerKnockback>();

        if (inputReader == null || groundDetector == null)
        {
            Debug.LogError(
                "PlayerMotor requires a PlayerInputReader and a GroundDetector.",
                this);

            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        bool jumpPressed = inputReader.ConsumeJump();

        if (knockback != null && knockback.IsActive)
        {
            return;
        }

        Vector2 move = inputReader.Move;
        Vector2 velocity = body.linearVelocity;

        velocity.x = move.x * moveSpeed;

        if (Mathf.Abs(move.x) > 0.1f)
        {
            FacingDirection = move.x > 0f ? 1 : -1;
        }

        if (jumpPressed && groundDetector.IsGrounded())
        {
            velocity.y = jumpSpeed;
        }

        body.linearVelocity = velocity;
    }
}