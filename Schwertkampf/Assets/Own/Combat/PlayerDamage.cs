using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int HealthPoints = 100;
    public Material handMaterial; // Reference to the hand material

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
}
