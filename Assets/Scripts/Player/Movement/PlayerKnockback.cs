using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(Rigidbody2D))]
public class PlayerKnockback : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField, Min(0f)] private float horizontalSpeed = 8f;
    [SerializeField, Min(0f)] private float verticalSpeed = 4f;
    [SerializeField, Min(0f)] private float reactionDuration = 0.15f;

    private Health health;
    private Rigidbody2D body;
    private PlayerMotor motor;
    private PlayerMeleeAttack meleeAttack;
    private PlayerInputReader inputReader;

    private float knockbackUntil;

    public bool IsActive =>
        isActiveAndEnabled &&
        health != null &&
        !health.IsDead &&
        Time.time < knockbackUntil;

    private void Awake()
    {
        health = GetComponent<Health>();
        body = GetComponent<Rigidbody2D>();
        motor = GetComponent<PlayerMotor>();
        meleeAttack = GetComponent<PlayerMeleeAttack>();
        inputReader = GetComponent<PlayerInputReader>();

        if (motor == null || meleeAttack == null || inputReader == null)
        {
            Debug.LogError(
                "PlayerKnockback requires PlayerMotor, " +
                "PlayerMeleeAttack and PlayerInputReader on the same object.",
                this);

            enabled = false;
        }
    }

    private void OnEnable()
    {
        health.DamageReceived += HandleDamage;
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.DamageReceived -= HandleDamage;
        }

        knockbackUntil = 0f;
    }

    private void HandleDamage(DamageInfo damageInfo)
    {
        if (!isActiveAndEnabled || health.IsDead)
        {
            return;
        }

        int direction = -motor.FacingDirection;

        if (damageInfo.HasSourcePosition)
        {
            float horizontalDifference =
                body.position.x - damageInfo.SourcePosition.x;

            if (Mathf.Abs(horizontalDifference) > 0.01f)
            {
                direction = horizontalDifference > 0f ? 1 : -1;
            }
        }

        knockbackUntil = Time.time + reactionDuration;

        meleeAttack.CancelAttack();

        inputReader.ConsumeJump();
        inputReader.ConsumeAttack1();
        inputReader.ConsumeAttack2();

        body.linearVelocity = new Vector2(
            direction * horizontalSpeed,
            verticalSpeed);
    }
}