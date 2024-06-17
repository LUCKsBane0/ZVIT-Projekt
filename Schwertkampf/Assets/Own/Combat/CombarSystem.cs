using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour
{
    public float deg1;
    public float deg2;
    public float deg3;
    public Transform swordTip; // Reference to the tip of the player's sword
    public GameObject arrowPrefab; // Prefab for the arrow with particle effect
    public Transform enemy; // Reference to the enemy
    public float cooldownTime = 1f; // Time before the sword can hit the same enemy again
    public float minDistanceToRehit = 1f; // Minimum distance the sword must move away from the enemy before it can hit again
    public float arrowDespawnTime = 2f; // Time after which the arrow despawns
    public float arrowMoveSpeed = 1.5f; // Speed at which the arrow moves

    public StateController stateController; // Reference to the state controller (for enemy states)

    private Vector3 attackDirection; // Direction from which the enemy is vulnerable
    private bool canHit = true; // Controls if the player can hit the enemy
    private Vector3 lastHitPosition; // Position of the last hit
    private GameObject lastHitEnemy; // Reference to the last hit enemy
    private GameObject currentArrow; // Reference to the current arrow

    void Update()
    {
        // Update the current enemy state
        // Assume stateController refers to the current enemy

        if (stateController.isBlocking)
        {
            if (currentArrow == null)
            {
                DisplayBlockingArrow();
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
        }

        if (!canHit && lastHitEnemy != null)
        {
            float distance = Vector3.Distance(swordTip.position, lastHitEnemy.transform.position);
            if (distance >= minDistanceToRehit)
            {
                canHit = true;
                lastHitEnemy = null; // Reset the last hit enemy
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemySword") && stateController.isAttacking)
        {
            PushBackEnemy();
        }
        
        else if (other.CompareTag("Enemy") && stateController.isAttacking && canHit)
        {
            PushBackEnemy();
            EnemyController enemyController = other.GetComponent<EnemyController>();
            Debug.Log("Successful Hit!");
            enemyController.GetComponent<EnemyController>().TakeDamage(10);
        }
        
        //Das hier muss getestet werden
        else if (other.CompareTag("Enemy") && stateController.isBlocking && canHit)
        {
            Vector3 hitDirection = (other.transform.position - swordTip.position).normalized;
            if (Vector3.Dot(hitDirection, attackDirection) > 0.8f) // Adjust threshold as needed
            {
                EnemyController enemyController = other.GetComponent<EnemyController>();
                Debug.Log("Successful Hit!");
                enemyController.GetComponent<EnemyController>().TakeDamage(10);

                StartCoroutine(HitCooldown());
                lastHitPosition = swordTip.position;
                lastHitEnemy = other.gameObject;
            }
            else
            {
                Debug.Log("Failed Hit. Wrong Direction!");
            }
        }
    }

    void PushBackEnemy()
    {
        // Placeholder function for pushing back the enemy
        Debug.Log("Enemy pushed back");
        // Implement logic to push back the enemy
    }

    void DisplayBlockingArrow()
    {
        // Determine the direction
        int direction = Random.Range(0, 3); // 0 = top, 1 = right, 2 = left
        direction = 1;
        Vector3 arrowPosition = enemy.position;
        Quaternion arrowRotation = Quaternion.identity;

        switch (direction)
        {
            case 0:
                arrowPosition += Vector3.up * 2; // Adjust height as needed
                arrowRotation = Quaternion.Euler(180, 90, 0);
                attackDirection = Vector3.down;
                break;
            case 1:
                arrowPosition += Vector3.right * 2; // Adjust offset as needed
                arrowRotation = Quaternion.Euler(deg1, deg2, deg3);
                attackDirection = Vector3.left;
                break;
            case 2:
                arrowPosition += Vector3.left * 2; // Adjust offset as needed
                arrowRotation = Quaternion.Euler(0, 0, 90);
                attackDirection = Vector3.right;
                break;
        }

        currentArrow = Instantiate(arrowPrefab, arrowPosition, arrowRotation, enemy);
        currentArrow.tag = "Arrow";
        StartCoroutine(DespawnArrowAfterTime(arrowDespawnTime));
    }

    void MoveArrow()
    {
        currentArrow.transform.position += attackDirection * arrowMoveSpeed * Time.deltaTime;
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
    }
}
