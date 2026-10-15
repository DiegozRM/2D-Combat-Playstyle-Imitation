using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(Rigidbody2D))]
public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Attack References")]
    [SerializeField] private MeleeHitbox meleeHitbox;
    [SerializeField] private GameObject slashVisual;

    private Health health;
    private Rigidbody2D body;
    private PlayerInputReader inputReader;
    private PlayerMotor motor;
    private PlayerMeleeAttack meleeAttack;

    private bool deathHandled;

    private void Awake()
    {
        health = GetComponent<Health>();
        body = GetComponent<Rigidbody2D>();
        inputReader = GetComponent<PlayerInputReader>();
        motor = GetComponent<PlayerMotor>();
        meleeAttack = GetComponent<PlayerMeleeAttack>();

        if (inputReader == null ||
            motor == null ||
            meleeAttack == null ||
            meleeHitbox == null ||
            slashVisual == null)
        {
            Debug.LogError(
                "PlayerDeathHandler requires the player input, motor, " +
                "melee attack, hitbox and slash visual references.",
                this);

            enabled = false;
        }
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
    }

    private void Start()
    {
        if (health.IsDead)
        {
            HandleDeath();
        }
    }

    private void OnDisable()
    {
        health.Died -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (deathHandled)
        {
            return;
        }

        deathHandled = true;
        Debug.Log("PlayerDeathHandler handled death.", this);

        inputReader.enabled = false;
        motor.enabled = false;
        meleeAttack.enabled = false;

        meleeHitbox.EndAttack();
        slashVisual.SetActive(false);

        Vector2 velocity = body.linearVelocity;
        velocity.x = 0f;
        body.linearVelocity = velocity;
    }
}