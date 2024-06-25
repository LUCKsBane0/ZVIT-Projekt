using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ChallengeSystem : MonoBehaviour
{
    public InputActionReference challengeAction; // Reference to the challenge action (button press)
    public Transform swordTip; // Reference to the tip of the sword for raycasting
    public GameObject progressRingPrefab; // Prefab for the progress ring
    public Transform handTransform; // Reference to the hand holding the sword

    public PlayerStates playerStates;
   
    private GameObject currentRing;
    private Image ringImage;
    private GameObject enemyObject;
    private float challengeTime = 2f; // Time it takes to complete the challenge
    private float challengeProgress = 0f;
    private bool isChallenging = false;
	private GameObject ChallengeColliderObject;
    void Start()
    {
        
        challengeAction.action.Enable();
    }

    void Update()
    {
        // Always draw a debug ray from the (0, 0, 0) position into the sky

        // Check if the button is pressed
        if (challengeAction.action.ReadValue<float>() > 0.5f && !playerStates.inCombat)
        {
            RaycastHit hit;
            Vector3 rayDirection = swordTip.forward;
            float rayLength = Mathf.Infinity;
            Debug.Log("Casting Ray!");
            Debug.DrawRay(swordTip.position, rayDirection * 10f, Color.red); // Draw the ray

            if (Physics.Raycast(swordTip.position, rayDirection, out hit, rayLength))
            {
                if (hit.collider.CompareTag("ChallengeHitbox"))
                {
                    
                    
                    if (!isChallenging)
                    {
                        
                        StartChallenge();
						ChallengeColliderObject = hit.collider.gameObject;
                        enemyObject = hit.collider.gameObject.transform.parent.gameObject;
                       
                    }
                }
                else
                {
                    if (isChallenging)
                    {
                        ResetChallenge();
                    }
                }
            }
            else
            {
                if (isChallenging)
                {
                    ResetChallenge();
                }
            }
        }
        else
        {
            if (isChallenging)
            {
                ResetChallenge();
            }
        }

        if (isChallenging)
        {
            UpdateChallenge();
        }
    }

    void StartChallenge()
    {
        if (currentRing != null)
        {
            Destroy(currentRing);
        }

        currentRing = Instantiate(progressRingPrefab, handTransform.position, Quaternion.Euler(0, 90, 0), handTransform);
        ringImage = currentRing.GetComponentInChildren<Image>();
        ringImage.color = Color.red; // Set the color of the ring to red
        ringImage.fillAmount = 0f;
        challengeProgress = 0f;
        isChallenging = true;
    }

    void UpdateChallenge()
    {
        challengeProgress += Time.deltaTime;
        ringImage.fillAmount = challengeProgress / challengeTime;

        if (challengeProgress >= challengeTime)
        {
            CompleteChallenge();
        }
    }

    void CompleteChallenge()
    {
        playerStates.inCombat = true;
        ChallengeColliderObject.GetComponent<BoxCollider>().enabled = false;
        Destroy(currentRing);
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
        isChallenging = false;
        
        
        
    }

    void ResetChallenge()
    {
        if (currentRing != null)
        {
            Destroy(currentRing);
        }
        challengeProgress = 0f;
        isChallenging = false;
    }
}
