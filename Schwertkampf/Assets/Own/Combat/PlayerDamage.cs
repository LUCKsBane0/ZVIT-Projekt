using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour
{
    public int HealthPoints = 100;
    public Material handMaterial; // Reference to the hand material
    public Image vignetteImage; // Reference to the vignette image

    private Color originalColor;
    private Vector3 initialSpawnPosition;

    void Start()
    {
        // Store the original color of the hands
        if (handMaterial != null)
        {
            originalColor = Color.white;
        }
        else
        {
            Debug.LogError("Hand material is not assigned.");
        }

        // Ensure the vignette image is initially transparent
        if (vignetteImage != null)
        {
            vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, 0);
        }
        else
        {
            Debug.LogError("Vignette image is not assigned.");
        }

        // Store the initial spawn position
        initialSpawnPosition = transform.position;
    }

    void Update()
    {
        // Update the color of the hands based on health
        UpdateHandColor();
    }

    public void TakeDamage(int amount)
    {
        HealthPoints -= amount;
        HealthPoints = Mathf.Clamp(HealthPoints, 0, 100); // Ensure HealthPoints stay within bounds

        if (HealthPoints <= 20)
        {
            SoundEffectsManager.instance.PlayLowHealthSound();
        }

        if (HealthPoints <= 0)
        {
            Die();
        }

        // Trigger vignette effect
        if (vignetteImage != null)
        {
            StartCoroutine(ShowVignetteEffect());
        }
    }

    void Die()
    {
        SoundEffectsManager.instance.PlayDyingSound();
        StartCoroutine(HandleDeath());
    }

    void UpdateHandColor()
    {
        if (handMaterial != null)
        {
            // Calculate the color based on the health
            float healthPercentage = (float)HealthPoints / 100;
            Color newColor = Color.Lerp(Color.red, originalColor, healthPercentage);

            // Apply the new color to the material
            handMaterial.color = newColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemySword"))
        {
            Debug.Log("Taking Damage");
            TakeDamage(10);
        }
    }

    IEnumerator ShowVignetteEffect()
    {
        float duration = 0.5f; // Duration of the vignette effect
        float maxAlpha = 0.3f; // Maximum alpha value for the vignette effect (adjust for transparency)
        float elapsed = 0f;

        // Fade in the vignette effect
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, Mathf.Lerp(0, maxAlpha, elapsed / duration));
            yield return null;
        }

        elapsed = 0f;

        // Fade out the vignette effect
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, Mathf.Lerp(maxAlpha, 0, elapsed / duration));
            yield return null;
        }

        // Ensure the vignette is fully transparent after the effect
        vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, 0);
    }

    IEnumerator HandleDeath()
    {
        float grayVignetteDuration = 1f;
        float blackScreenDuration = 1f;
        float elapsed = 0f;

        // Fade to gray vignette
        while (elapsed < grayVignetteDuration)
        {
            elapsed += Time.deltaTime;
            vignetteImage.color = new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0, 1, elapsed / grayVignetteDuration));
            yield return null;
        }

        // Ensure the vignette is fully gray
        vignetteImage.color = new Color(0.5f, 0.5f, 0.5f, 1);

        elapsed = 0f;

        // Fade to full black screen
        while (elapsed < blackScreenDuration)
        {
            elapsed += Time.deltaTime;
            vignetteImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 1, elapsed / blackScreenDuration));
            yield return null;
        }

        // Ensure the screen is fully black
        vignetteImage.color = new Color(0, 0, 0, 1);

        // Teleport the player back to the initial spawn position
        transform.position = initialSpawnPosition;

        // Reset health
        HealthPoints = 100;

        // Fade out the black screen back to transparent
        elapsed = 0f;
        while (elapsed < blackScreenDuration)
        {
            elapsed += Time.deltaTime;
            vignetteImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, elapsed / blackScreenDuration));
            yield return null;
        }

        // Ensure the vignette is fully transparent
        vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, 0);
    }
}
