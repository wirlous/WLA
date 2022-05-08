using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Range(1, 100)] public int minHealth;
    [Range(1, 100)] public int maxHealth;

    [SerializeField] private int health;

    public int Health { get => health; }

    public delegate void Death(); // Delegate
    public event Death OnDeath; // Event
    

    void Start()
    {
        ResetHealth();
    }

    public void SetMinMaxHealth(int min, int max)
    {
        minHealth = min;
        maxHealth = max;
    }

    public void ResetHealth()
    {
        health = Random.Range(minHealth, maxHealth);
    }

    public bool Heal(int heal)
    {
        if (health == maxHealth)
            return false;
        
        health = Mathf.Min(maxHealth, health + heal);
        return true;
    }

    public void ReceiveDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            OnDeath?.Invoke();
        }
    }

}
