using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private enum AttackPhase
    {
        Ready,
        Startup,
        Active
    }

    [Header("References")]
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private GroundDetector groundDetector;
    [SerializeField] private Transform attackRig;
    [SerializeField] private MeleeHitbox hitbox;
    [SerializeField] private GameObject slashVisual;

    [Header("Attack")]
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField] private LayerMask targetLayers;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float startupDuration = 0f;
    [SerializeField, Min(0.02f)] private float activeDuration = 0.12f;
    [SerializeField, Min(0.02f)] private float attackInterval = 0.35f;

    private PlayerMotor playerMotor;
    private PlayerVisuals playerVisuals;
    private PlayerKnockback knockback;

    private AttackPhase phase = AttackPhase.Ready;
    private float startupEndTime;
    private float activeEndTime;
    private float nextAttackStartTime;


    private void Awake()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerVisuals = GetComponent<PlayerVisuals>();
        knockback = GetComponent<PlayerKnockback>();

        if (inputReader == null ||
            groundDetector == null ||
            attackRig == null ||
            hitbox == null ||
            slashVisual == null ||
            playerMotor == null ||
            playerVisuals == null)
        {
            Debug.LogError(
                "PlayerMeleeAttack has a missing reference.",
                this
            );

            enabled = false;
            return;
        }

        if (startupDuration + activeDuration > attackInterval)
        {
            Debug.LogError(
                "Startup Duration plus Active Duration must not exceed Attack Interval.",
                this
            );

            enabled = false;
            return;
        }

        slashVisual.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (knockback != null && knockback.IsActive)
        {
            inputReader.ConsumeAttack1();
            inputReader.ConsumeAttack2();
            return;
        }

        float now = Time.fixedTime;

        if (phase == AttackPhase.Startup && now >= startupEndTime)
        {
            BeginActivePhase();
        }

        if (phase == AttackPhase.Active && now >= activeEndTime)
        {
            EndAttack();
        }

        if (!inputReader.ConsumeAttack1())
            return;

        if (phase != AttackPhase.Ready || now < nextAttackStartTime)
            return;

        BeginAttack(now);
    }

    private void BeginAttack(float now)
    {
        Vector2 direction = GetAttackDirection();
        int facing = GetFacingForAttack();

        playerVisuals.LockFacing(facing);

        float angle = direction.y > 0f
            ? 90f
            : direction.y < 0f ? -90f : 0f;

        attackRig.localRotation = Quaternion.Euler(0f, 0f, angle);

        startupEndTime = now + startupDuration;
        activeEndTime = startupEndTime + activeDuration;
        nextAttackStartTime = now + attackInterval;

        if (startupDuration == 0f)
        {
            BeginActivePhase();
        }
        else
        {
            phase = AttackPhase.Startup;
        }

        Debug.Log($"Attack1 started: {direction}", this);
    }

    private void BeginActivePhase()
    {
        phase = AttackPhase.Active;

        hitbox.BeginAttack(damage, targetLayers);
        slashVisual.SetActive(true);
    }

    public void CancelAttack()
    {
        if (phase == AttackPhase.Ready)
        {
            return;
        }

        EndAttack();
    }
    private void EndAttack()
    {
        hitbox.EndAttack();
        slashVisual.SetActive(false);
        playerVisuals.UnlockFacing();

        phase = AttackPhase.Ready;
    }

    private Vector2 GetAttackDirection()
    {
        Vector2 move = inputReader.Move;

        if (move.y > 0.5f)
            return Vector2.up;

        if (move.y < -0.5f && !groundDetector.IsGrounded())
            return Vector2.down;

        return GetFacingForAttack() > 0
            ? Vector2.right
            : Vector2.left;
    }

    private int GetFacingForAttack()
    {
        float horizontalInput = inputReader.Move.x;

        if (Mathf.Abs(horizontalInput) > 0.1f)
            return horizontalInput > 0f ? 1 : -1;

        return playerMotor.FacingDirection;
    }

    private void OnDisable()
    {
        CancelAttack();
    }
}