using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int MaxHealth = 100;
    private int _currentHealth;

    [SerializeField]
    private HealthBar HealthBar;
    private bool hasDied = false;
    private PlayerStates playerStates;
    private SceneChanger sceneLoader;

    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneChanger>();

        if (sceneLoader == null)
        {
            Debug.LogError("SceneChanger not found in the scene.");
        }
    }

    private void Awake()
    {
        _currentHealth = MaxHealth;
        HealthBar.Initialize(MaxHealth, _currentHealth);
		playerStates = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<PlayerStates>();
       
    }

    public void TakeDamage(int damage)
    {
        if (playerStates.inCombat)
        {
            Debug.Log("Took Damage");
            _currentHealth -= damage;
            HealthBar.Change(-damage);


         


            if (_currentHealth <= 0)
            {
                playerStates.inCombat = false;
                GameObject chestMail = GameObject.Find("ChallengeManager");
                chestMail.GetComponent<SphereCollider>().enabled = true;

                if (gameObject.GetComponent<MediumEnemy>() != null)
                {
                    gameObject.GetComponent<MediumEnemy>().Die();
                    StartCoroutine(DeathTimer());
                }
                if (gameObject.GetComponent<LightEnemy>() != null)
                {
                    gameObject.GetComponent<LightEnemy>().Die();
                    StartCoroutine(DeathTimer());
                }
                if (gameObject.GetComponent<HeavyEnemy>() != null)
                {
                    gameObject.GetComponent<HeavyEnemy>().Die();
                    StartCoroutine(DeathTimer());
                }

                if (gameObject.GetComponent<SkeletonBoss>() != null)
                {
                    if (gameObject.GetComponent<SkeletonBoss>().hasTwoPhases && !hasDied)
                    {
                        gameObject.GetComponent<SkeletonBoss>().Die();
                        hasDied = true;
                        _currentHealth = MaxHealth;
                    }
                    else if (!gameObject.GetComponent<SkeletonBoss>().hasTwoPhases)
                    {
                        StartCoroutine(DeathTimer());
                        sceneLoader.ChangeScene("LevelCompleted");
                        SoundEffectsManager.instance.PlaySuccessSound();
                    }
                    else if (hasDied)
                    {
                        StartCoroutine(DeathTimer());
                        sceneLoader.ChangeScene("LevelCompleted");
                        SoundEffectsManager.instance.PlaySuccessSound();
                    }
                    
                    


                }

            }
        }
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(2.2f);
        Destroy(gameObject);
    }
}
