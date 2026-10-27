using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public class EnemyDeathHandler : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Death")]
    [Tooltip("Seconds from death until the enemy is destroyed.")]
    [SerializeField, Min(0f)] private float destroyDelay = 2f;

    [Tooltip("Damage trigger colliders to disable when the enemy dies.")]
    [SerializeField] private Collider2D[] damageColliders;

    private static readonly int IsDeadHash =
        Animator.StringToHash("IsDead");

    private Health health;
    private bool deathHandled;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>(true);
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

        foreach (Collider2D damageCollider in damageColliders)
        {
            if (damageCollider != null)
            {
                damageCollider.enabled = false;
            }
        }

        if (animator != null)
        {
            animator.SetBool(IsDeadHash, true);
        }
        else
        {
            Debug.LogWarning(
                "Enemy death animation could not play: Animator is missing.",
                this);
        }

        Destroy(gameObject, destroyDelay);
    }
}