using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float detectionRange = 30.0f;  // Detection range to engage the player
    public float desiredDistance = 3.0f;  // Desired distance to keep from the player
    public float meleeDistance = 3.5f;    // Melee Range
    public float meleeCooldown = 2.0f;  // Cooldown between melee attacks MIN 1.2f
    public float blockCooldown = 5.0f;  // Cooldown between blocks
    public float blockDuration = 2.0f;  // Duration of the block
    public float moveSpeed = 2.5f;  // Speed at which the enemy moves towards or away from the player
    public float pushBackDistance = 5.0f;  // Distance to push the enemy back from the player
    private string blockAnimationTrigger = "TrBlock4";  // Animation trigger for blocking
    private string combatIdleTrigger = "TrComIdle4";  // Animation trigger for combat idle
    private string meleeTrigger = "TrMelee4";  // Animation trigger for combat idle
    private string moveTrigger = "TrMove";  // Animation trigger for moving
    private string moveEndTrigger = "TrMoveEnd";  // Animation trigger for stopping moving
    private int succesfullBlockCount = 0;
    public bool BlocksDone = false;

    private Animator animator;
    private float lastMeleeAttackTime;  // Time when the last melee attack was made
    private float lastBlockTime;  // Time when the last block was made
    private bool isBlocking = false;  // Indicates if the enemy is currently blocking
    private bool isAttacking = false;  // Indicates if the enemy is in attacking animation
    private bool isAlive = true;  // Indicates if the enemy is alive
    private bool isInCombat = false;  // Indicates if the enemy is in combat
    private bool isMoving = false;  // Indicates if the enemy is moving

    public StateController stateController;
    public PlayerStates playerStates;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Find the player by tag
        lastMeleeAttackTime = -meleeCooldown;  // Set initial attack time to allow immediate first attack
        lastBlockTime = -blockCooldown;  // Set initial block time to allow immediate first block
    }

    void Update()
    {
        if (!isAlive || !isInCombat) return;  // Stop updating if the enemy is dead or not in combat

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Make the enemy look at the player
        LookAtPlayer();

        if (succesfullBlockCount >= 2)
        {
            BlocksDone = true;
        }

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
            if (distanceToPlayer <= meleeDistance && Time.time - lastMeleeAttackTime > meleeCooldown && !isBlocking && !BlocksDone)
            {
                StartCoroutine(PerformMeleeAttack());
                lastMeleeAttackTime = Time.time;  // Update last attack time
            }

            // Block occasionally if not already blocking and cooldown period has passed
            if (!isBlocking && Time.time - lastBlockTime > blockCooldown && !isAttacking && BlocksDone)
            {
                StartCoroutine(PerformBlock());
                lastBlockTime = Time.time;  // Update last block time
            }
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;
        stateController.isAttacking = true;
        animator.SetTrigger(meleeTrigger);

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

    public void TakeDamage()
    {
        // Handle taking damage normally
        animator.SetTrigger("TrGetHit4");
        Debug.Log("Took damage!");
    }

    public void PushBack()
    {
        if (BlocksDone == false)
        {
            succesfullBlockCount++;
        }

        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        directionAwayFromPlayer.y = 0; // Ignore Y axis
        StartCoroutine(MoveBackOverTime(directionAwayFromPlayer));
    }

    IEnumerator MoveBackOverTime(Vector3 direction)
    {
        float duration = 0.2f; // Duration of the pushback in seconds
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * pushBackDistance;

        animator.SetTrigger("TrCancel4"); // Set the cancel trigger if needed

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
        animator.SetTrigger("TrDeath4");
        isAlive = false;  // Mark the enemy as dead
        ExitCombat();
        playerStates.currentEnemy = null;
    }

    public void EnterCombat()
    {
        isInCombat = true;
        stateController.inCombat = true;
        animator.SetTrigger("TrCombat4");
        animator.SetTrigger("TrCombatMove4");
    }

    public void ExitCombat()
    {
        isInCombat = false;
        stateController.inCombat = false;
        StopMoving();
    }

    private void StartMoving()
    {
        if (!isMoving)
        {
            isMoving = true;
            animator.SetTrigger(moveTrigger);
        }
    }

    private void StopMoving()
    {
        if (isMoving)
        {
            isMoving = false;
            animator.SetTrigger(moveEndTrigger);
        }
    }
}
