using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField, Min(1)] private int maxHealth = 5;

    [Header("Damage Protection")]
    [SerializeField, Min(0f)] private float invulnerabilityDuration = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = false;

    private float invulnerableUntil;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsDead => CurrentHealth <= 0;
    public bool IsInvulnerable => Time.time < invulnerableUntil;

    public event Action<int, int> HealthChanged;
    public event Action<DamageInfo> DamageReceived;
    public event Action Died;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        invulnerabilityDuration = Mathf.Max(0f, invulnerabilityDuration);

        CurrentHealth = maxHealth;
        invulnerableUntil = 0f;
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(new DamageInfo(amount));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        TryTakeDamage(damageInfo);
    }

    public bool TryTakeDamage(int amount)
    {
        return TryTakeDamage(new DamageInfo(amount));
    }

    public bool TryTakeDamage(DamageInfo damageInfo)
    {
        if (!isActiveAndEnabled ||
            IsDead ||
            IsInvulnerable ||
            damageInfo.Amount <= 0)
        {
            return false;
        }

        int appliedDamage = Mathf.Min(
            damageInfo.Amount,
            CurrentHealth);

        CurrentHealth -= appliedDamage;
        invulnerableUntil = Time.time + invulnerabilityDuration;

        DamageInfo acceptedDamage = damageInfo.HasSourcePosition
            ? new DamageInfo(appliedDamage, damageInfo.SourcePosition)
            : new DamageInfo(appliedDamage);

        if (showDebugMessages)
        {
            Debug.Log(
                $"{name} received {appliedDamage} damage. " +
                $"Health: {CurrentHealth}/{maxHealth}.",
                this);
        }

        HealthChanged?.Invoke(CurrentHealth, maxHealth);
        DamageReceived?.Invoke(acceptedDamage);

        if (IsDead)
        {
            if (showDebugMessages)
            {
                Debug.Log($"{name} died.", this);
            }

            Died?.Invoke();
        }

        return true;
    }
}