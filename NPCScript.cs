using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        checkDeath();
    }

    void checkDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log(name + " has died.");
            Destroy(gameObject);
        }
    }

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(name + "CURRENT HP: " + currentHealth);
    }
}
