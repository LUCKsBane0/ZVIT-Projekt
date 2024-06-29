using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemy : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float detectionRange = 30.0f;  // Detection range to engage the player
    public float desiredDistance = 3.0f;  // Desired distance to keep from the player
    public float meleeDistance = 3.5f;    // Melee Range
    public float meleeCooldown = 2.0f;  // Cooldown between melee attacks MIN 1.2f
    public float blockCooldown = 5.0f;  // Cooldown between blocks
    public float blockDuration = 2.0f;  // Duration of the block
    public float moveSpeed = 2.5f;  // Speed at which the enemy moves towards or away from the player
    public float strafeSpeed = 2.0f;  // Speed at which the enemy strafes left and right
    public float strafeDistance = 2.0f;  // Distance the enemy strafes from the center position
    public float strafePauseDuration = 2.0f;  // Duration of pause between strafes
    public float pushBackDistance = 5.0f;  // Distance to push the enemy back from the player
    private string[] meleeAnimationTriggers = { "TrMelee", "TrSpin", "TrPunch" };  // Animation triggers for melee attacks
    private string blockAnimationTrigger = "TrBlock";  // Animation trigger for blocking
    private string combatIdleTrigger = "TrComIdle";  // Animation trigger for combat idle
    private string moveTrigger = "TrMove";  // Animation trigger for moving
    private string moveEndTrigger = "TrMoveEnd";  // Animation trigger for stopping moving

    private Animator animator;
    private float lastMeleeAttackTime;  // Time when the last melee attack was made
    private float lastBlockTime;  // Time when the last block was made
    private bool isBlocking = false;  // Indicates if the enemy is currently blocking
    private bool isAttacking = false;  // Indicates if the enemy is in attacking animation
    public bool isAlive = true;  // Indicates if the enemy is alive
    private bool isInCombat = false;  // Indicates if the enemy is in combat
    private bool isStrafingRight = true;  // Indicates the current strafe direction
    private bool isStrafingPaused = false;  // Indicates if strafing is currently paused
    private Vector3 initialPosition;  // Initial position to calculate strafing
    private bool isMoving = false;  // Indicates if the enemy is moving

    public StateController stateController;
    private PlayerStates playerStates;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player by tag
        lastMeleeAttackTime = -meleeCooldown;  // Set initial attack time to allow immediate first attack
        lastBlockTime = -blockCooldown;  // Set initial block time to allow immediate first block
        initialPosition = transform.position;  // Store the initial position for strafing
        playerStates = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<PlayerStates>();

    }

    void Update()
    {
        if (!isAlive || !isInCombat) return;  // Stop updating if the enemy is dead or not in combat

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Make the enemy look at the player
        LookAtPlayer();

        if (distanceToPlayer <= detectionRange)
        {
            // Engage the player if within detection range

            // Adjust position to maintain the desired distance
            if (Mathf.Abs(distanceToPlayer - desiredDistance) > 0.1f)
            {
                MaintainDistanceFromPlayer(distanceToPlayer);
            }
            else if (isMoving)
            {
                StopMoving();
            }

            // Perform melee attack if within melee range and cooldown period has passed
            if (distanceToPlayer <= meleeDistance && Time.time - lastMeleeAttackTime > meleeCooldown && !isBlocking)
            {
                StartCoroutine(PerformMeleeAttack());
                lastMeleeAttackTime = Time.time;  // Update last attack time
            }

            // Strafe left and right with a pause between strafes
            if (distanceToPlayer <= desiredDistance && !isStrafingPaused)
            {
                Strafe(strafeDistance);
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

        // Waiting time depending on animation
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

        StartMoving();
    }

    void MaintainDistanceFromPlayer(float distanceToPlayer)
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Ignore Y axis

        if (distanceToPlayer > desiredDistance)
        {
            // Move closer to the player
            Vector3 newPosition = transform.position + directionToPlayer * moveSpeed * Time.deltaTime;
            transform.position = newPosition;
            StartMoving();
        }
        else if (distanceToPlayer < desiredDistance)
        {
            // Move away from the player
            Vector3 newPosition = transform.position - directionToPlayer * moveSpeed * 0.5f * Time.deltaTime;
            transform.position = newPosition;
            StartMoving();
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Ignore Y axis
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);
    }

    IEnumerator PauseStrafing()
    {
        isStrafingPaused = true;
        yield return new WaitForSeconds(strafePauseDuration);
        initialPosition = transform.position;
        isStrafingPaused = false;
    }

    public void TakeDamage()
    {
        // Handle taking damage normally
        animator.SetTrigger("TrGetHit");
        Debug.Log("Took damage!");
    }

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
            StartMoving();
            yield return null;
        }

        transform.position = targetPosition;
        StopMoving();
    }

    public void Die()
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
        animator.SetTrigger("TrCombatMove");
    }

    public void ExitCombat()
    {
        isInCombat = false;
        stateController.inCombat = false;
    }

    void StartMoving()
    {
        if (!isMoving)
        {
            isMoving = true;
            animator.SetTrigger(moveTrigger);
        }
    }

    void StopMoving()
    {
        if (isMoving)
        {
            isMoving = false;
            animator.SetTrigger(moveEndTrigger);
        }
    }
}
