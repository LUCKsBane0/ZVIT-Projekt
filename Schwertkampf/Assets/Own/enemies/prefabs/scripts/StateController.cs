using UnityEngine;

public class StateController : MonoBehaviour
{
    public Collider leftCollider;
    public Collider topCollider;
    public Collider rightCollider;

    public bool isBlocking;
    public bool isAttacking;
    public bool inCombat = false;
    // Method to enable a specific collider
    public void EnableCollider(string direction)
    {
        DisableAllColliders();

        switch (direction)
        {
            case "left":
                leftCollider.enabled = true;
                break;
            case "top":
                topCollider.enabled = true;
                break;
            case "right":
                rightCollider.enabled = true;
                break;
        }
    }

    // Method to disable all colliders
    public void DisableAllColliders()
    {
        leftCollider.enabled = false;
        topCollider.enabled = false;
        rightCollider.enabled = false;
    }
}
