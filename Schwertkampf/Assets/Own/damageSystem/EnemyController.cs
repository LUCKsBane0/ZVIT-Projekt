using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int MaxHealth = 100;
    private int _currentHealth;

    [SerializeField]
    private HealthBar HealthBar;

    public PlayerStates playerStates;

    private void Awake()
    {
        _currentHealth = MaxHealth;
        HealthBar.Initialize(MaxHealth, _currentHealth);
       
    }

    public void TakeDamage(int damage)
    {
        if (playerStates.inCombat)
        {
            Debug.Log("Took Damage");
            _currentHealth -= damage;
            HealthBar.Change(-damage);

            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
