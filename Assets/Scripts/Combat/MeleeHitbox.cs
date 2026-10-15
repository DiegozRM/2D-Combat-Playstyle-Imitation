using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MeleeHitbox : MonoBehaviour
{
    private readonly HashSet<IDamageable> hitTargets = new();

    private BoxCollider2D attackCollider;
    private LayerMask targetLayers;
    private int damage;
    private bool attackActive;

    private void Awake()
    {
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.isTrigger = true;
        attackCollider.enabled = false;
    }

    public void BeginAttack(int attackDamage, LayerMask layers)
    {
        hitTargets.Clear();
        damage = attackDamage;
        targetLayers = layers;
        attackActive = true;
        attackCollider.enabled = true;
    }

    public void EndAttack()
    {
        attackActive = false;
        attackCollider.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D other)
    {
        if (!attackActive)
            return;

        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target == null || !hitTargets.Add(target))
            return;

        DamageInfo damageInfo = new DamageInfo(
            damage,
            transform.position);

        target.TakeDamage(damageInfo);

        Debug.Log($"Attack1 hit: {other.name}", this);
    }
}