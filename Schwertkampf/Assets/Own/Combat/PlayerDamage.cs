using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour
{
    public int HealthPoints = 100;
    public Material handMaterial; // Reference to the hand material
    public Image vignetteImage; // Reference to the vignette image

    private Color originalColor;


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

        if(HealthPoints <= 20)
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
        //handle death

        SoundEffectsManager.instance.PlayDyingSound();
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
        Debug.Log(other);




    }

    IEnumerator ShowVignetteEffect()
    {
        float duration = 0.5f; // Duration of the vignette effect
        float elapsed = 0f;

        // Fade in the vignette effect
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, Mathf.Lerp(0, 0.5f, elapsed / duration));
            yield return null;
        }

        elapsed = 0f;

        // Fade out the vignette effect
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, Mathf.Lerp(0.5f, 0, elapsed / duration));
            yield return null;
        }

        // Ensure the vignette is fully transparent after the effect
        vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, 0);
    }
}
