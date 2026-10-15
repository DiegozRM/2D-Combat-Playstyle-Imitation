using UnityEngine;

public class DamageableDummy : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int maxHealth = 5;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (currentHealth <= 0 || damageInfo.Amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(
            0,
            currentHealth - damageInfo.Amount);

        Debug.Log($"Dummy health: {currentHealth}", this);
    }
}