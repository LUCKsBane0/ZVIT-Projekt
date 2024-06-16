using UnityEngine;
using System.Collections;

public class HitController : MonoBehaviour
{
    public float cooldownTime = 1f; // Time before the sword can hit the same enemy again
    public float minDistanceToRehit = 1f; // Minimum distance the sword must move away from the enemy before it can hit again

    private bool canHit = true;
    private Vector3 lastHitPosition;
    private GameObject lastHitEnemy;

    void OnTriggerEnter(Collider other)
    {
        if (canHit && other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Debug.Log("HIT");
                enemy.GetComponent<EnemyController>().TakeDamage(10);
                StartCoroutine(HitCooldown());
                lastHitPosition = transform.position;
                lastHitEnemy = other.gameObject;
            }
        }
    }

    void Update()
    {
        if (!canHit && lastHitEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, lastHitEnemy.transform.position);
            //Debug.Log(distance);
            if (distance >= minDistanceToRehit)
            {
                canHit = true;
                lastHitEnemy = null; // Reset the last hit enemy
            }
        }
    }

    IEnumerator HitCooldown()
    {
        canHit = false;
        yield return new WaitForSeconds(cooldownTime);
    }
}
