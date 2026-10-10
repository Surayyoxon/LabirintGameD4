using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("Player Health: " + currentHealth + "%");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // 0 dan pastga tushib ketmasin
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Player Damage: -" + damage);
        Debug.Log("Player Health: " + currentHealth + "%");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");

        // Keyinchalik Game Over tizimini shu yerga qo'shamiz.
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}