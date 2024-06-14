using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkeletonBoss : MonoBehaviour
{
    public GameObject orangeSpellPrefab;  // Prefab for the orange spell
    public GameObject blueSpellPrefab;    // Prefab for the blue spell
    private string[] animationTriggers = { "TrLC", "TrRC"};  // Animation triggers for spells

    public Transform player;  // Reference to the player (XR Rig)
    public float detectionRange = 20.0f;  // Detection range to trigger spell casting
    public float spellCooldown = 3.0f;   // Cooldown between spell casts

    private Animator animator;
    private float lastSpellCastTime;  // Time when the last spell was cast
    public bool isInRangedPhase = false;
    public bool hasTwoPhases = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player (XR Rig) by tag
        lastSpellCastTime = -spellCooldown;  // Set initial cast time to allow immediate first cast
    }

    void Update()
    {
        // Check distance to player and cast spells accordingly
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (isInRangedPhase && distanceToPlayer <= detectionRange && Time.time - lastSpellCastTime > spellCooldown)
        {
            // Cast a spell only if enough time has passed since the last cast
            CastSpell();
            lastSpellCastTime = Time.time;  // Update last cast time
        }
    }

    void CastSpell()
    {
        // Select a random spell index
        int spellIndex = Random.Range(0, 2); // 0 for orangeSpellPrefab, 1 for blueSpellPrefab

        // Trigger the corresponding animation immediately
        if (spellIndex == 0)
        {
            // Trigger TrLC animation
            animator.SetTrigger("TrLC");
        }
        else if (spellIndex == 1)
        {
            // Trigger TrRC animation
            animator.SetTrigger("TrRC");
        }
        else
        {
            // If spellIndex is out of bounds (should not happen with current setup)
            Debug.LogWarning("Spell index out of bounds for animation triggers.");
            return;
        }

        // Delay before spawning the spell (adjust the delay time as needed)
        float delayTime = 0.3f; // Example delay time in seconds
        StartCoroutine(SpawnSpellAfterDelay(spellIndex, delayTime));
    }

    IEnumerator SpawnSpellAfterDelay(int spellIndex, float delayTime)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delayTime);

        // Calculate spawn positions with offsets
        Vector3 spawnPosition = transform.position + Vector3.up * 1.7f;
        Vector3 orangeOffset = -transform.right * 1.0f;  // Offset to the left for orange spell
        Vector3 blueOffset = transform.right * 1.0f;     // Offset to the right for blue spell

        // Determine the spawn position based on spell index
        Vector3 spawnOffset = spellIndex == 0 ? orangeOffset : blueOffset;

        // Instantiate the selected spell prefab at the adjusted position
        GameObject spell = Instantiate(spellIndex == 0 ? orangeSpellPrefab : blueSpellPrefab, spawnPosition + spawnOffset, Quaternion.identity);

        // Initialize the spell with the player's position
        spell.GetComponent<Spell>().Initialize(player);
    }




    void ResetAnimationTriggers()
    {
        // Reset all animation triggers to prevent unintended looping
        foreach (var trigger in animationTriggers)
        {
            animator.ResetTrigger(trigger);
        }
    }

    public void TakeDamage(int amount)
    {
        // Handle taking damage
    }

    void Die()
    {
        if (!isInRangedPhase)
        {
            animator.SetTrigger("TrDeathMelee");
        }
        else
        {
            animator.SetTrigger("TrDeathRange");
        }

        StartCoroutine(ReviveAfterDelay());
    }

    IEnumerator ReviveAfterDelay()
    {
        yield return new WaitForSeconds(3.0f); // Adjust delay time as needed

        if (hasTwoPhases && !isInRangedPhase)
        {
            Revive();
        }
    }


    void Revive()
    {
        animator.SetTrigger("TrRevive");
        isInRangedPhase = true;
    }
}

