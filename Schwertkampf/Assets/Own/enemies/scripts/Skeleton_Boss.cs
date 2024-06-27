using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBoss : MonoBehaviour
{
    public GameObject orangeSpellPrefab;  // Prefab for the orange spell
    public GameObject blueSpellPrefab;    // Prefab for the blue spell
    private string[] spellAnimationTriggers = { "TrLC", "TrRC" };  // Animation triggers for spells
    private string[] meleeAnimationTriggers = { "TrL", "TrR", "TrLR" };  // Animation triggers for melee attacks
    

    public Transform player;  // Reference to the player (XR Rig)
    public float moveSpeed = 2.5f;  // Speed at which the enemy moves towards or away from the player   
    public float meleeRange = 3.0f;  // Range to trigger melee attacks
    public float desiredMeleeDistance = 3.0f;  // Desired distance to maintain from the player in melee mode
    public float spellCooldown = 3.0f;   // Cooldown between spell casts
    public float meleeCooldown = 2.0f;   // Cooldown between melee attacks
    public float strafeSpeed = 2.0f;   // Speed at which the boss strafes left and right
    public float meleeStrafeDistance = 2.0f;   // Distance the boss strafes from the center position in melee mode
    public float rangedStrafeDistance = 10.0f;   // Distance the boss strafes from the center position in ranged mode
    public float strafePauseDuration = 0.5f;  // Duration of pause between strafes
    public float rangedElevationHeight = 1.0f;  // Height at which the boss hovers in ranged mode
    public float retreatDistance = 13.0f;  // Distance to retreat from the player in ranged mode
    public float pushBackDistance = 3.0f;  // Distance to push the enemy back from the player

    private Animator animator;
    private float lastSpellCastTime;  // Time when the last spell was cast
    private float lastMeleeAttackTime;  // Time when the last melee attack was made
    public bool isInRangedPhase = false;  // Indicates if the boss is in ranged phase
    public bool hasTwoPhases = true;  // Indicates if the boss has two phases (melee and ranged)
    private bool isStrafingRight = true;  // Indicates the current strafe direction
    private bool isStrafingPaused = false;  // Indicates if strafing is currently paused
    private bool isAlive = true;  // Indicates if the boss is alive
    private bool isElevated = false;  // Indicates if the boss is elevated
    private Vector3 initialPosition;  // Initial position to calculate strafing
    private PlayerStates playerStates;


    void Start()
    {
        playerStates = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<PlayerStates>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player (XR Rig) by tag
        lastSpellCastTime = -spellCooldown;  // Set initial cast time to allow immediate first cast
        lastMeleeAttackTime = -meleeCooldown;  // Set initial attack time to allow immediate first attack
        initialPosition = transform.position;  // Store the initial position for strafing
    }

    void Update()
    {
        if (playerStates.hasPassedBoss)
        {



            //look if player is at correct position

            if (!playerStates.inCombat)
            {
                playerStates.inCombat = true;
                playerStates.currentEnemy = gameObject;
            }
            if (!isAlive) return;  // Stop updating if the boss is dead

            LookAtPlayer();

            // Check distance to player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (isInRangedPhase)
            {
                // Ranged phase logic: Cast spells if within detection range and cooldown period has passed
                if (Time.time - lastSpellCastTime > spellCooldown)
                {
                    CastSpell();
                    lastSpellCastTime = Time.time;  // Update last cast time
                }

                // Move away from the player if within retreat distance
                if (distanceToPlayer <= retreatDistance)
                {
                    RetreatFromPlayer();
                }
                else
                {
                    // Strafe left and right with a pause between strafes
                    if (!isStrafingPaused)
                    {
                        Strafe(rangedStrafeDistance);
                    }
                }
            }
            else
            {
                // Melee phase logic: Perform melee attack if within melee range and cooldown period has passed
                if (distanceToPlayer <= meleeRange && Time.time - lastMeleeAttackTime > meleeCooldown)
                {
                    PerformMeleeAttack();
                    lastMeleeAttackTime = Time.time;  // Update last attack time
                }

                // Maintain desired melee distance
                if (distanceToPlayer < desiredMeleeDistance)
                {
                    MaintainMeleeDistance();
                }
                else if (distanceToPlayer > meleeRange)
                {
                    // Move towards the player if not in melee range
                    MoveTowardsPlayer();
                }

                // Strafe left and right with a pause between strafes
                if (!isStrafingPaused)
                {
                    Strafe(meleeStrafeDistance);
                }
            }
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Ignore Y axis
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);
    }

    void CastSpell()
    {
        // Select a random spell index
        int spellIndex = Random.Range(0, spellAnimationTriggers.Length);

        // Trigger the corresponding animation immediately
        string selectedTrigger = spellAnimationTriggers[spellIndex];
        animator.SetTrigger(selectedTrigger);

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

    void PerformMeleeAttack()
    {
        // Select a random melee attack index
        int attackIndex = Random.Range(0, meleeAnimationTriggers.Length);

        // Trigger the corresponding animation immediately
        string selectedTrigger = meleeAnimationTriggers[attackIndex];
        animator.SetTrigger(selectedTrigger);
    }

    void Strafe(float strafeDistance)
    {
        float strafeSpeedAdjusted = strafeSpeed * Time.deltaTime;
        Vector3 strafeDirection = isStrafingRight ? Vector3.right : Vector3.left;
        transform.Translate(strafeDirection * strafeSpeedAdjusted);

        if (Vector3.Distance(transform.position, initialPosition) >= strafeDistance)
        {
            isStrafingRight = !isStrafingRight;
            StartCoroutine(PauseStrafing(strafeDistance));
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Ignore Y axis
        Vector3 newPosition = transform.position + directionToPlayer * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    void RetreatFromPlayer()
    {
        Vector3 directionToPlayer = (transform.position - player.position).normalized;
        directionToPlayer.y = 0; // Ignore Y axis
        Vector3 newPosition = transform.position + directionToPlayer * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    void MaintainMeleeDistance()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        directionAwayFromPlayer.y = 0; // Ignore Y axis
        Vector3 newPosition = transform.position + directionAwayFromPlayer * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    IEnumerator PauseStrafing(float strafeDistance)
    {
        isStrafingPaused = true;
        yield return new WaitForSeconds(strafePauseDuration);
        initialPosition = transform.position;
        isStrafingPaused = false;
    }

    /*void ResetAnimationTriggers()
    {
        // Reset all animation triggers to prevent unintended looping
        foreach (var trigger in spellAnimationTriggers)
        {
            animator.ResetTrigger(trigger);
        }

        foreach (var trigger in meleeAnimationTriggers)
        {
            animator.ResetTrigger(trigger);
        }
    }*/

    public void PushBack()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        directionAwayFromPlayer.y = 0; // Ignore Y axis
        animator.SetTrigger("TrCancel"); // Set the cancel trigger if needed
        StartCoroutine(MoveBackOverTime(directionAwayFromPlayer));
    }

    IEnumerator MoveBackOverTime(Vector3 direction)
    {
        float duration = 0.2f; // Duration of the pushback in seconds
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * pushBackDistance;


        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }


    public void TakeDamage()
    {
        if (!isInRangedPhase)
        {
            animator.SetTrigger("TrGetHitMelee");
        }
        else
        {
            animator.SetTrigger("TrGetHitRange");
        }
    }

    public void Die()
    {
        animator.SetTrigger("TrMoveEndBoss");
        
        if (!isInRangedPhase)
        {
            animator.SetTrigger("TrDeathMelee");
        }
        else
        {
            animator.SetTrigger("TrDeathRange");
        }

        isAlive = false;  // Mark the boss as dead
        StopAllCoroutines();  // Stop any ongoing coroutines (e.g., strafing)

        StartCoroutine(ReviveAfterDelay());
    }

    IEnumerator ReviveAfterDelay()
    {
        yield return new WaitForSeconds(3.0f);

        if (hasTwoPhases && !isInRangedPhase)
        {
            Revive();
        }
    }

    void Revive()
    {
        animator.SetTrigger("TrRevive");
        isInRangedPhase = true;
        isAlive = true;  // Mark the boss as alive again

        // Elevate the boss when reviving in ranged mode
        if (!isElevated)
        {
            ElevateBoss();
        }
    }

    void ElevateBoss()
    {
        Vector3 elevatedPosition = transform.position;
        elevatedPosition.y += rangedElevationHeight;
        transform.position = elevatedPosition;
        isElevated = true;  // Mark the boss as elevated
    }
    
}
