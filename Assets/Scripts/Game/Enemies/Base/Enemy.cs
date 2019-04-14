using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Enemy : MonoBehaviour, IHealth
{
    [Header("Base")]
    public int maxHealth = 100;
    
    private int health = 0;
    
    protected virtual void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        UIManager.Instance.SpawnFloatingText(damage.ToString(), transform);

        // Reduce health with damage
        health -= damage;
        // Update slider
        if (health <= damage)
        {
            Destroy(gameObject);
        }
    }
}
