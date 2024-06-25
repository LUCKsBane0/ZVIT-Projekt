using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

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

    private LineRenderer lineRenderer;
    private Coroutine fadeCoroutine;

    void Start()
    {
        challengeAction.action.Enable();
    }

    void Update()
    {
        // Check if the button is pressed
        if (challengeAction.action.ReadValue<float>() > 0.5f && !playerStates.inCombat)
        {
            if (lineRenderer == null)
            {
                CreateLineRenderer();
                fadeCoroutine = StartCoroutine(FadeLineRenderer(lineRenderer, 0f, 1f, 0.5f));
            }

            RaycastHit hit;
            Vector3 rayDirection = swordTip.forward;
            float rayLength = Mathf.Infinity;
            Debug.Log("Casting Ray!");

            lineRenderer.SetPosition(0, swordTip.position);
            lineRenderer.SetPosition(1, swordTip.position + rayDirection * 20f);

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
            if (lineRenderer != null)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                fadeCoroutine = StartCoroutine(FadeOutAndDestroyLineRenderer(lineRenderer, 1f, 0f, 0.5f));
                lineRenderer = null;
            }

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

    void CreateLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Use a default sprite shader material
        lineRenderer.startColor = new Color(1, 0, 0, 0); // Start with red color and fully transparent
        lineRenderer.endColor = new Color(1, 0, 0, 0);   // Start with red color and fully transparent
    }

    IEnumerator FadeLineRenderer(LineRenderer lr, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            lr.startColor = new Color(1, 0, 0, alpha);
            lr.endColor = new Color(1, 0, 0, alpha);
            yield return null;
        }

        lr.startColor = new Color(1, 0, 0, endAlpha);
        lr.endColor = new Color(1, 0, 0, endAlpha);
    }

    IEnumerator FadeOutAndDestroyLineRenderer(LineRenderer lr, float startAlpha, float endAlpha, float duration)
    {
        yield return FadeLineRenderer(lr, startAlpha, endAlpha, duration);
        Destroy(lr);
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
