using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour
{
    public Transform swordTip; // Reference to the tip of the player's sword
    public GameObject arrowPrefab; // Prefab for the arrow with particle effect
    public Transform enemy; // Reference to the enemy
    public float cooldownTime = 1f; // Time before the sword can hit the same enemy again
    public float minDistanceToRehit = 1f; // Minimum distance the sword must move away from the enemy before it can hit again

    public StateController stateController; // MUSS ZU CURRENT ENEMY GEÄNDERT WERDEN


    private Vector3 attackDirection; // Direction from which the enemy is vulnerable
    private bool canHit = true; // Controls if the player can hit the enemy
    private Vector3 lastHitPosition; // Position of the last hit
    private GameObject lastHitEnemy; // Reference to the last hit enemy



    


    void Update()
    {
       //update den currentEnemy

        if (stateController.isBlocking)
        {
            DisplayBlockingArrow();
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
        // Clear any existing arrows
        foreach (Transform child in enemy)
        {
            if (child.CompareTag("Arrow"))
            {
                Destroy(child.gameObject);
            }
        }

        // Determine the direction
        int direction = Random.Range(0, 3); // 0 = top, 1 = right, 2 = left
        Vector3 arrowPosition = enemy.position;
        Quaternion arrowRotation = Quaternion.identity;

        switch (direction)
        {
            case 0:
                arrowPosition += Vector3.up * 2; // Adjust height as needed
                arrowRotation = Quaternion.Euler(90, 0, 0);
                attackDirection = Vector3.down;
                break;
            case 1:
                arrowPosition += Vector3.right * 2; // Adjust offset as needed
                arrowRotation = Quaternion.Euler(0, 0, -90);
                attackDirection = Vector3.left;
                break;
            case 2:
                arrowPosition += Vector3.left * 2; // Adjust offset as needed
                arrowRotation = Quaternion.Euler(0, 0, 90);
                attackDirection = Vector3.right;
                break;
        }

        GameObject arrow = Instantiate(arrowPrefab, arrowPosition, arrowRotation, enemy);
        arrow.tag = "Arrow";
    }

    IEnumerator HitCooldown()
    {
        canHit = false;
        yield return new WaitForSeconds(cooldownTime);
    }
}

   
