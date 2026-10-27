using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public class DamageFlash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Flash")]
    [SerializeField, Min(0.01f)] private float flashDuration = 0.1f;

    private static readonly int FlashAmountId =
        Shader.PropertyToID("_FlashAmount");

    private Health health;
    private MaterialPropertyBlock propertyBlock;

    private bool isFlashing;
    private float flashEndTime;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (targetRenderer == null)
        {
            Debug.LogError(
                "DamageFlash requires a target SpriteRenderer.",
                this);

            enabled = false;
            return;
        }

        Material material = targetRenderer.sharedMaterial;

        if (material == null || !material.HasProperty(FlashAmountId))
        {
            Debug.LogError(
                "DamageFlash requires a material with a _FlashAmount property.",
                this);

            enabled = false;
            return;
        }

        propertyBlock = new MaterialPropertyBlock();
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

        StopFlash();
    }

    private void Update()
    {
        if (isFlashing && Time.time >= flashEndTime)
        {
            StopFlash();
        }
    }

    private void HandleDamage(DamageInfo damageInfo)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        flashEndTime = Time.time + flashDuration;
        isFlashing = true;

        SetFlashAmount(1f);
    }

    private void StopFlash()
    {
        if (!isFlashing)
        {
            return;
        }

        isFlashing = false;

        if (targetRenderer != null)
        {
            SetFlashAmount(0f);
        }
    }

    private void SetFlashAmount(float amount)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(FlashAmountId, amount);

        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}