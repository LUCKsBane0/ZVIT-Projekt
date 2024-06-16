using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyEnemy : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float detectionRange = 30.0f;  // Detection range to engage the player
    public float meleeRange = 3.5f;  // Range to perform melee attacks
    public float meleeCooldown = 1.8f;  // Cooldown between melee attacks
    public float blockCooldown = 5.0f;  // Cooldown between blocks
    public float blockDuration = 3.0f;  // Duration of the block
    public float moveSpeed = 2.0f;  // Speed at which the enemy moves towards the player
    public float strafeSpeed = 1.0f;  // Speed at which the enemy strafes left and right
    public float strafeDistance = 1.0f;  // Distance the enemy strafes from the center position
    public float strafePauseDuration = 3.0f;  // Duration of pause between strafes
    private string[] meleeAnimationTriggers = { "TrMelee2", "TrSpin2", "TrReverseHit2" };  // Animation triggers for melee attacks
    private string blockAnimationTrigger = "TrBlock2";  // Animation trigger for blocking
    private string combatIdleTrigger = "TrComIdle2";  // Animation trigger for combat idle

    private Animator animator;
    private float lastMeleeAttackTime;  // Time when the last melee attack was made
    private float lastBlockTime;  // Time when the last block was made
    private bool isBlocking = false;  // Indicates if the enemy is currently blocking
    private bool isAlive = true;  // Indicates if the enemy is alive
    private bool isInCombat = false;  // Indicates if the enemy is in combat
    private bool isStrafingRight = true;  // Indicates the current strafe direction
    private bool isStrafingPaused = false;  // Indicates if strafing is currently paused
    private Vector3 initialPosition;  // Initial position to calculate strafing

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player by tag
        lastMeleeAttackTime = -meleeCooldown;  // Set initial attack time to allow immediate first attack
        lastBlockTime = -blockCooldown;  // Set initial block time to allow immediate first block
        initialPosition = transform.position;  // Store the initial position for strafing

        if (player == null)
        {
            Debug.LogError("Player not found. Make sure the player GameObject has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (!isAlive || !isInCombat) return;  // Stop updating if the enemy is dead or not in combat

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Engage the player if within detection range
            Debug.Log("Player detected within range.");

            if (distanceToPlayer <= meleeRange)
            {
                // Perform melee attack if within melee range and cooldown period has passed
                if (Time.time - lastMeleeAttackTime > meleeCooldown)
                {
                    PerformMeleeAttack();
                    lastMeleeAttackTime = Time.time;  // Update last attack time
                }

                // Strafe left and right with a pause between strafes
                if (!isStrafingPaused)
                {
                    Strafe(strafeDistance);
                }
            }
            else
            {
                // Move towards the player if not in melee range
                MoveTowardsPlayer();
            }

            // Block occasionally if not already blocking and cooldown period has passed
            if (!isBlocking && Time.time - lastBlockTime > blockCooldown)
            {
                StartCoroutine(PerformBlock());
                lastBlockTime = Time.time;  // Update last block time
            }
        }
    }

    void PerformMeleeAttack()
    {
        // Select a random melee attack index
        int attackIndex = Random.Range(0, meleeAnimationTriggers.Length);

        // Trigger the corresponding animation immediately
        string selectedTrigger = meleeAnimationTriggers[attackIndex];
        animator.SetTrigger(selectedTrigger);
        Debug.Log("Performed melee attack: " + selectedTrigger);
    }

    IEnumerator PerformBlock()
    {
        isBlocking = true;
        animator.SetTrigger(blockAnimationTrigger);

        // Wait for the duration of the block
        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        animator.SetTrigger(combatIdleTrigger); // Transition back to combat idle
        Debug.Log("Block finished, returning to combat idle.");
    }

    void Strafe(float strafeDistance)
    {
        float strafeSpeedAdjusted = strafeSpeed * Time.deltaTime;
        Vector3 strafeDirection = isStrafingRight ? Vector3.right : Vector3.left;
        transform.Translate(strafeDirection * strafeSpeedAdjusted);

        if (Vector3.Distance(transform.position, initialPosition) >= strafeDistance)
        {
            isStrafingRight = !isStrafingRight;
            StartCoroutine(PauseStrafing());
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Ignore Y axis
        Vector3 newPosition = transform.position + directionToPlayer * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
        Debug.Log("Moving towards player.");
    }

    IEnumerator PauseStrafing()
    {
        isStrafingPaused = true;
        yield return new WaitForSeconds(strafePauseDuration);
        initialPosition = transform.position;
        isStrafingPaused = false;
        Debug.Log("Resuming strafing.");
    }

    public void TakeDamage(int amount)
    {
        if (isBlocking)
        {
            // Handle the case where the enemy is blocking (e.g., reduce damage or ignore it)
            Debug.Log("Blocked the attack!");
        }
        else
        {
            // Handle taking damage normally
            animator.SetTrigger("TrGetHit2");
            Debug.Log("Took damage!");
        }
    }

    void Die()
    {
        animator.SetTrigger("TrDeath2");
        isAlive = false;  // Mark the enemy as dead
        isInCombat = false;  // Mark the enemy as out of combat
        Debug.Log("Enemy died.");
    }

    public void EnterCombat()
    {
        isInCombat = true;
        animator.SetTrigger("TrCombat2");
        Debug.Log("Entered combat.");
    }

    public void ExitCombat()
    {
        isInCombat = false;
        Debug.Log("Exited combat.");
    }
}
