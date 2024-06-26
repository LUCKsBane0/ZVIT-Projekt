using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    private List<float> enemyTimestamps = new List<float>();
    private PlayerStates playerStates;
    private Coroutine challengeCoroutine;
	
    void Start()
    {
		
        // Start the coroutine to periodically challenge enemies
        challengeCoroutine = StartCoroutine(ChallengeEnemyRoutine());
 	playerStates = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<PlayerStates>();
	
    }

    void Update()
    {
		 GameObject chestMail = GameObject.Find("ChestMail");
        if (chestMail != null)
        {
            transform.position = chestMail.transform.position;
            transform.rotation = chestMail.transform.rotation;
           
        }
        else
        {
            Debug.LogError("ChestMail GameObject not found in the scene.");
        }
        // Remove enemies that are no longer valid
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
                enemyTimestamps.RemoveAt(i);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
			
			if(other.GetComponent<TutorialEnemy>() == null){
			  // Add the enemy and current timestamp to the lists
            enemies.Add(other.gameObject);
            enemyTimestamps.Add(Time.time);
			}
          
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Find the index of the enemy in the list
            int index = enemies.IndexOf(other.gameObject);
            if (index != -1)
            {
                // Remove the enemy and its timestamp from the lists
                enemies.RemoveAt(index);
                enemyTimestamps.RemoveAt(index);
            }
        }
    }

    private IEnumerator ChallengeEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (!playerStates.inCombat && enemies.Count > 0)
            {
                // Find the enemy with the lowest timestamp
                float minTimestamp = float.MaxValue;
                int minIndex = -1;
                for (int i = 0; i < enemyTimestamps.Count; i++)
                {
                    if (enemyTimestamps[i] < minTimestamp)
                    {
                        minTimestamp = enemyTimestamps[i];
                        minIndex = i;
                    }
                }

                if (minIndex != -1)
                {
                    // Challenge the enemy with the lowest timestamp
                    Challenge(enemies[minIndex]);

                    // Remove the challenged enemy and its timestamp from the lists
                    enemies.RemoveAt(minIndex);
                    enemyTimestamps.RemoveAt(minIndex);
                }
            }
        }
    }

    private void Challenge(GameObject enemyObject)
    {
        playerStates.inCombat = true;
		gameObject.GetComponent<SphereCollider>().enabled = false;
		if (enemyObject.GetComponent<LightEnemy>() != null)
        {
            enemyObject.GetComponent<LightEnemy>().EnterCombat();
        }

        if (enemyObject.GetComponent<MediumEnemy>() != null)
        {
            enemyObject.GetComponent<MediumEnemy>().EnterCombat();
        }
        if (enemyObject.GetComponent<HeavyEnemy>() != null)
        {
            enemyObject.GetComponent<HeavyEnemy>().EnterCombat();
        }
        if (enemyObject.GetComponent<TutorialEnemy>() != null)
        {
            enemyObject.GetComponent<TutorialEnemy>().EnterCombat();
        }
		 playerStates.currentEnemy = enemyObject;
    }
}
