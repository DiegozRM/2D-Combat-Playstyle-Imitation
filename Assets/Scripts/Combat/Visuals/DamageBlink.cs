using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public class DamageBlink : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Blink")]
    [SerializeField, Min(0.01f)] private float blinkInterval = 0.1f;

    private Health health;

    private bool originalVisibility;
    private bool isBlinking;
    private bool blinkVisible;
    private float nextBlinkTime;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (targetRenderer == null)
        {
            Debug.LogError(
                "DamageBlink requires a target renderer.",
                this);

            enabled = false;
            return;
        }

        originalVisibility = targetRenderer.enabled;
    }

    private void OnEnable()
    {
        health.DamageReceived += HandleDamage;
        health.Died += StopBlink;

        if (health.IsInvulnerable && !health.IsDead)
        {
            BeginBlink();
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.DamageReceived -= HandleDamage;
            health.Died -= StopBlink;
        }

        StopBlink();
    }

    private void Update()
    {
        if (!isBlinking)
        {
            return;
        }

        if (!health.IsInvulnerable || health.IsDead)
        {
            StopBlink();
            return;
        }

        if (Time.time < nextBlinkTime)
        {
            return;
        }

        blinkVisible = !blinkVisible;
        targetRenderer.enabled = originalVisibility && blinkVisible;

        nextBlinkTime = Time.time + blinkInterval;
    }

    private void HandleDamage(DamageInfo damageInfo)
    {
        if (health.IsInvulnerable && !health.IsDead)
        {
            BeginBlink();
        }
    }

    private void BeginBlink()
    {
        isBlinking = true;
        blinkVisible = false;
        nextBlinkTime = Time.time + blinkInterval;

        targetRenderer.enabled = false;
    }

    private void StopBlink()
    {
        if (!isBlinking)
        {
            return;
        }

        isBlinking = false;

        if (targetRenderer != null)
        {
            targetRenderer.enabled = originalVisibility;
        }
    }
}