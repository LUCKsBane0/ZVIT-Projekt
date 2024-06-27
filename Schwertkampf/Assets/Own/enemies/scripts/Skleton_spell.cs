using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float speed = 10f;  // Speed of the spell
    private Transform currentTarget;  // Current target to move towards (player or XR Rig)
    public Transform playerTarget;  // Player target
    public Transform enemyTarget;  // Enemy target
    public bool autoDeleteEnabled = false;  // Flag to enable auto deletion
    public float reflectSpeedMultiplier = 1.5f;  // Speed multiplier when spell is reflected
    public bool isReflected = false;  // Flag to track if the spell has been reflected

    void Start()
    {
        if (autoDeleteEnabled)
        {
            // Enable auto deletion
            Invoke("AutoDelete", 2f);
        }
        currentTarget = playerTarget; // Set the initial target to the player
    }

    void Update()
    {
        if (currentTarget != null)
        {
            // Move towards the current target
            Vector3 moveDirection = (currentTarget.position - transform.position).normalized;
            float currentSpeed = isReflected ? speed * reflectSpeedMultiplier : speed;
            transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // If current target is null (should not happen with current setup)
            Debug.LogWarning("Spell current target is null.");
            Destroy(gameObject);
        }
    }

    public void Initialize(Transform newPlayerTarget, Transform newEnemyTarget)
    {
        playerTarget = newPlayerTarget;
        enemyTarget = newEnemyTarget;
        currentTarget = playerTarget; // Initially target the player
    }

    void OnTriggerEnter(Collider other)
    {
        // Example collision logic (destroy spell on impact)
        if (other.CompareTag("ChestMail"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player_Sword"))
        {
            ReflectSpell();
        }
    }

    void ReflectSpell()
    {
        isReflected = true;
        currentTarget = enemyTarget; // Change target to the enemy when reflected
    }

    void AutoDelete()
    {
        // Destroy the spell
        Destroy(gameObject);
    }
}
