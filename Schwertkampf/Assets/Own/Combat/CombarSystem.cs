using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class CombatSystem : MonoBehaviour
{
    public Transform swordTip; // Reference to the tip of the player's sword
    public GameObject arrowPrefab; // Prefab for the arrow with particle effect
    private Transform enemy; // Reference to the enemy
    public float cooldownTime = 1f; // Time before the sword can hit the same enemy again
    public float minDistanceToRehit = 1f; // Minimum distance the sword must move away from the enemy before it can hit again
    public float arrowDespawnTime = 2f; // Time after which the arrow despawns
    public float arrowMoveSpeed = 1.5f; // Speed at which the arrow moves

    private StateController stateController; // Reference to the state controller (for enemy states)
    public PlayerStates playerStates;
    private bool canHit = true; // Controls if the player can hit the enemy
    private bool canHitDistance = true;
    private Vector3 lastHitPosition; // Position of the last hit
    private GameObject lastHitEnemy; // Reference to the last hit enemy
    private GameObject currentArrow; // Reference to the current arrow

    private bool blockStateChanged = true;
    private Vector3 attackDirection; // Direction from which the enemy is vulnerable

    // Reference to the right hand controller
    public XRBaseController rightHandController;

    void Update()
    {
        // Update the current enemy state
        if (playerStates.currentEnemy != null)
        {
            stateController = playerStates.currentEnemy.GetComponent<StateController>();
            enemy = playerStates.currentEnemy.GetComponent<Transform>();

            if (!stateController.isBlocking)
            {
                blockStateChanged = true;
            }

            if (stateController.isBlocking)
            {
                if (currentArrow == null && blockStateChanged)
                {
                    DisplayBlockingArrow();
                    blockStateChanged = false;
                }
                else
                {
                    MoveArrow();
                }
            }
            else
            {
                if (currentArrow != null)
                {
                    Destroy(currentArrow);
                }
                stateController.DisableAllColliders();
            }

            if (!canHitDistance && lastHitEnemy != null)
            {
                float distance = Vector3.Distance(swordTip.position, lastHitEnemy.transform.position);
                if (distance >= minDistanceToRehit)
                {
                    canHitDistance = true;
                    lastHitEnemy = null; // Reset the last hit enemy
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (stateController != null)
        {
            if (other.CompareTag("EnemySword") && stateController.isAttacking)
            {
                PushBackEnemy();
                TriggerHapticFeedback();
            }
            else if (other.CompareTag("Enemy") && !stateController.isAttacking && !stateController.isBlocking)
            {
                SuccessfulHit(other);
                TriggerHapticFeedback();
            }
            else if (other.CompareTag("Enemy") && stateController.isAttacking && canHit && canHitDistance)
            {
                PushBackEnemy();
                TriggerHapticFeedback();
                EnemyController enemyController = other.GetComponent<EnemyController>();
                Debug.Log("Successful Hit!");
               

                enemyController.GetComponent<EnemyController>().TakeDamage(10);
                if (playerStates.currentEnemy.GetComponent<MediumEnemy>() != null)
                {
                      playerStates.currentEnemy.GetComponent<MediumEnemy>().TakeDamage();
                }
                if (playerStates.currentEnemy.GetComponent<LightEnemy>() != null)
                {
                    playerStates.currentEnemy.GetComponent<LightEnemy>().TakeDamage();
                }
                if (playerStates.currentEnemy.GetComponent<HeavyEnemy>() != null)
                {
                    playerStates.currentEnemy.GetComponent<HeavyEnemy>().TakeDamage();
                }
				if (playerStates.currentEnemy.GetComponent<TutorialEnemy>() != null)
                {
					enemyController.GetComponent<EnemyController>().TakeDamage(40);
                    playerStates.currentEnemy.GetComponent<TutorialEnemy>().TakeDamage();
                }

                StartCoroutine(HitCooldown());
            }
            else if (other.CompareTag("LeftHitbox") && stateController.isBlocking && canHit && canHitDistance)
            {
                SuccessfulHit(other);
                TriggerHapticFeedback();
            }
            else if (other.CompareTag("TopHitbox") && stateController.isBlocking && canHit && canHitDistance)
            {
                SuccessfulHit(other);
                TriggerHapticFeedback();
            }
            else if (other.CompareTag("RightHitbox") && stateController.isBlocking && canHit && canHitDistance)
            {
                SuccessfulHit(other);
                TriggerHapticFeedback();
            }
        }
    }

    void SuccessfulHit(Collider other)
    {
        EnemyController enemyController = other.GetComponentInParent<EnemyController>();
        Debug.Log("Successful Hit!");
        enemyController.TakeDamage(10);
        SoundEffectsManager.instance.PlayHitSound();

        if (playerStates.currentEnemy.GetComponent<MediumEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<MediumEnemy>().TakeDamage();
        }
        if (playerStates.currentEnemy.GetComponent<LightEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<LightEnemy>().TakeDamage();
        }
        if (playerStates.currentEnemy.GetComponent<HeavyEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<HeavyEnemy>().TakeDamage();
        }
		if (playerStates.currentEnemy.GetComponent<TutorialEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<TutorialEnemy>().TakeDamage();
			enemyController.TakeDamage(40);
        }
        StartCoroutine(HitCooldown());
        lastHitPosition = swordTip.position;
        lastHitEnemy = other.transform.parent.gameObject;
    }

    void PushBackEnemy()
    {
        Debug.Log("Enemy pushed back");
        if (playerStates.currentEnemy.GetComponent<MediumEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<MediumEnemy>().PushBack();
        }
        if (playerStates.currentEnemy.GetComponent<LightEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<LightEnemy>().PushBack();
        }
        if (playerStates.currentEnemy.GetComponent<HeavyEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<HeavyEnemy>().PushBack();
        }

		if (playerStates.currentEnemy.GetComponent<TutorialEnemy>() != null)
        {
            playerStates.currentEnemy.GetComponent<TutorialEnemy>().PushBack();
        }
    }

    void DisplayBlockingArrow()
    {
        int direction = Random.Range(0, 3); // 0 = top, 1 = right, 2 = left

        Transform arrowPos = enemy.Find("Arrow_Pos");
        if (arrowPos == null)
        {
            Debug.LogError("Arrow_Pos child object not found in the enemy.");
            return;
        }

        Vector3 arrowPosition = arrowPos.position;
        Quaternion arrowRotation = Quaternion.identity;
        string colliderDirection = "";

        switch (direction)
        {
            case 0:
                arrowPosition += enemy.up * 2; // Adjust height as needed
                arrowRotation = Quaternion.LookRotation(-enemy.up, enemy.forward);
                attackDirection = -enemy.up;
                colliderDirection = "top";
                break;
            case 1:
                arrowPosition += enemy.right * 2; // Adjust offset as needed
                arrowRotation = Quaternion.LookRotation(-enemy.right, enemy.up);
                attackDirection = -enemy.right;
                colliderDirection = "right";
                break;
            case 2:
                arrowPosition += -enemy.right * 2; // Adjust offset as needed
                arrowRotation = Quaternion.LookRotation(enemy.right, enemy.up);
                attackDirection = enemy.right;
                colliderDirection = "left";
                break;
        }

        currentArrow = Instantiate(arrowPrefab, arrowPosition, arrowRotation, enemy);
        currentArrow.tag = "Arrow";
        StartCoroutine(DespawnArrowAfterTime(arrowDespawnTime));

        stateController.EnableCollider(colliderDirection);
    }

    void MoveArrow()
    {
        if (currentArrow != null)
        {
            currentArrow.transform.position += attackDirection * arrowMoveSpeed * Time.deltaTime;
        }
    }

    IEnumerator DespawnArrowAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (currentArrow != null)
        {
            Destroy(currentArrow);
        }
    }

    IEnumerator HitCooldown()
    {
        canHit = false;
        yield return new WaitForSeconds(cooldownTime);
        canHit = true;
    }

    void TriggerHapticFeedback()
    {
        if (rightHandController != null)
        {
            rightHandController.SendHapticImpulse(0.5f, 0.2f); // Adjust amplitude and duration as needed
        }
    }
}
