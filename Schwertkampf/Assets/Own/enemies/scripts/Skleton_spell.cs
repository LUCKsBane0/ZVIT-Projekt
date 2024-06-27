using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float speed = 10f;  // Speed of the spell
    private Transform target;  // Target to move towards (player or XR Rig)
    public bool autoDeleteEnabled = false;  // Flag to enable auto deletion
    public float reflectSpeedMultiplier = 1.5f;  // Speed multiplier when spell is reflected
    private bool isReflected = false;  // Flag to track if the spell has been reflected

    void Start()
    {
        if (autoDeleteEnabled)
        {
            // Enable auto deletion
            Invoke("AutoDelete", 2f);
        }
    }

    void Update()
    {
        if (target != null && !isReflected)
        {
            // Move towards the target
            Vector3 moveDirection = (target.position - transform.position).normalized;
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        }
        else if (isReflected)
        {
            // Move back towards the enemy when reflected
            Vector3 moveDirection = (target.position - transform.position).normalized;
            transform.Translate(moveDirection * speed * reflectSpeedMultiplier * Time.deltaTime, Space.World);
        }
        else
        {
            // If target is null (should not happen with current setup)
            Debug.LogWarning("Spell target is null.");
            Destroy(gameObject);
        }
    }

    public void Initialize(Transform newTarget)
    {
        target = newTarget;
    }

    void OnTriggerEnter(Collider other)
    {
        // Example collision logic (destroy spell on impact)
        if (other.CompareTag("Player")) // Assuming the player has the "Player" tag
        {
            Destroy(gameObject);
            //damage system
        }
        else if (other.CompareTag("Player_Sword"))  // Example tag for surface that can reflect spells
        {
            // Reflect the spell
            ReflectSpell();
        }
    }

    void ReflectSpell()
    {
        isReflected = true;
    }

    void AutoDelete()
    {
        // Destroy the spell
        Destroy(gameObject);
    }
}





