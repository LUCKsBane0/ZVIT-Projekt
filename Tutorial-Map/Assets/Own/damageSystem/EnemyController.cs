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

    private CombatController combatController;

    private void Awake()
    {
        _currentHealth = MaxHealth;
        HealthBar.Initialize(MaxHealth, _currentHealth);
        combatController = FindObjectOfType<CombatController>();
    }

    public void TakeDamage(int damage)
    {
        if (combatController != null && combatController.InCombat())
        {
            _currentHealth -= damage;
            HealthBar.Change(-damage);

            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
