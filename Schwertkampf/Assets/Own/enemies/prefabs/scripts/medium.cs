using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemy : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float detectionRange = 30.0f;  // Detection range to engage the player
    public float meleeRange = 3.0f;  // Range to perform melee attacks
    public float meleeCooldown = 2.0f;  // Cooldown between melee attacks MIN 1.2f
    public float blockCooldown = 5.0f;  // Cooldown between blocks
    public float blockDuration = 2.0f;  // Duration of the block
    public float moveSpeed = 2.5f;  // Speed at which the enemy moves towards the player
    public float strafeSpeed = 2.0f;  // Speed at which the enemy strafes left and right
    public float strafeDistance = 2.0f;  // Distance the enemy strafes from the center position
    public float strafePauseDuration = 2.0f;  // Duration of pause between strafes
    public float pushBackDistance = 5.0f;  // Distance to push the enemy back from the player
    private string[] meleeAnimationTriggers = { "TrMelee", "TrSpin", "TrPunch" };  // Animation triggers for melee attacks
    private string blockAnimationTrigger = "TrBlock";  // Animation trigger for blocking
    private string combatIdleTrigger = "TrComIdle";  // Animation trigger for combat idle

    private Animator animator;
    private float lastMeleeAttackTime;  // Time when the last melee attack was made
    private float lastBlockTime;  // Time when the last block was made
    private bool isBlocking = false;  // Indicates if the enemy is currently blocking
    private bool isAttacking = false; //Indicates if enemy is in attacking animation
    private bool isAlive = true;  // Indicates if the enemy is alive
    private bool isInCombat = false;  // Indicates if the enemy is in combat
    private bool isStrafingRight = true;  // Indicates the current strafe direction
    private bool isStrafingPaused = false;  // Indicates if strafing is currently paused
    private Vector3 initialPosition;  // Initial position to calculate strafing
    
    public StateController stateController;
    public PlayerStates playerStates;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player by tag
        lastMeleeAttackTime = -meleeCooldown;  // Set initial attack time to allow immediate first attack
        lastBlockTime = -blockCooldown;  // Set initial block time to allow immediate first block
        initialPosition = transform.position;  // Store the initial position for strafing
    }

    void Update()
    {
        if (!isAlive || !isInCombat) return;  // Stop updating if the enemy is dead or not in combat

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Engage the player if within detection range

            if (distanceToPlayer <= meleeRange)
            {
                // Perform melee attack if within melee range and cooldown period has passed
                if (Time.time - lastMeleeAttackTime > meleeCooldown && !isBlocking)
                {
                    StartCoroutine(PerformMeleeAttack());
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
            if (!isBlocking && Time.time - lastBlockTime > blockCooldown && !isAttacking)
            {
                StartCoroutine(PerformBlock());
                lastBlockTime = Time.time;  // Update last block time
            }
        }
    }

    IEnumerator PerformMeleeAttack()
    {
       
        // Select a random melee attack index
        int attackIndex = Random.Range(0, meleeAnimationTriggers.Length);

        // Trigger the corresponding animation immediately
        string selectedTrigger = meleeAnimationTriggers[attackIndex];
        isAttacking = true;
        stateController.isAttacking = true;
        animator.SetTrigger(selectedTrigger);

        //waiting Time depending on animation

        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
        stateController.isAttacking = false;
    }

    IEnumerator PerformBlock()
    {
        stateController.isBlocking = true;
        isBlocking = true;
        animator.SetTrigger(blockAnimationTrigger);

        // Wait for the duration of the block
        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        stateController.isBlocking = false;
        animator.SetTrigger(combatIdleTrigger); // Transition back to combat idle
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
    }

    IEnumerator PauseStrafing()
    {
        isStrafingPaused = true;
        yield return new WaitForSeconds(strafePauseDuration);
        initialPosition = transform.position;
        isStrafingPaused = false;
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
            animator.SetTrigger("TrGetHit");
            Debug.Log("Took damage!");
        }
    }

    public void PushBack()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        directionAwayFromPlayer.y = 0; // Ignore Y axis
        Vector3 newPosition = transform.position + directionAwayFromPlayer * pushBackDistance;
        transform.position = newPosition;
        animator.SetTrigger("TrCancel"); // Set the cancel trigger if needed
    }

    void Die()
    {
        animator.SetTrigger("TrDeath");
        isAlive = false;  // Mark the enemy as dead
        ExitCombat();
        playerStates.currentEnemy = null;
    }

    public void EnterCombat()
    {
        isInCombat = true;
        stateController.inCombat = true;
        animator.SetTrigger("TrCombat");
    }

    public void ExitCombat()
    {
        isInCombat = false;
        stateController.inCombat = false;
    }
}
