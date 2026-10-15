using UnityEngine;

[DisallowMultipleComponent]
public class ContactDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField] private LayerMask targetLayers;

    private Health ownerHealth;

    private void Awake()
    {
        Collider2D contactCollider = GetComponent<Collider2D>();

        if (contactCollider == null || !contactCollider.isTrigger)
        {
            Debug.LogError(
                "ContactDamage requires a Collider2D with Is Trigger enabled.",
                this);

            enabled = false;
            return;
        }

        ownerHealth = GetComponentInParent<Health>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (ownerHealth != null &&
            (!ownerHealth.isActiveAndEnabled || ownerHealth.IsDead))
        {
            return;
        }

        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        Health targetHealth = other.GetComponentInParent<Health>();

        if (targetHealth == null || targetHealth == ownerHealth)
        {
            return;
        }

        DamageInfo damageInfo = new DamageInfo(
            damage,
            transform.position);

        targetHealth.TryTakeDamage(damageInfo);
    }
}