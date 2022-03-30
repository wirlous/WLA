﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Freya;

public class HealthSystem : MonoBehaviour
{
    [Range(5, 100)] public int minHealth;
    [Range(5, 100)] public int maxHealth;
    
    [SerializeField] private int health;

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

    public void ReceiveDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

}