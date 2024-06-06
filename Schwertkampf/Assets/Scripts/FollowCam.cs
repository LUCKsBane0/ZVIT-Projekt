using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform headTransform; // Reference to the head's transform
    public Vector3 positionOffset = new Vector3(0, -1.8f, 0); // Offset to position the chestmail relative to the head
    public float rotationOffsetY = 0f; // Y-axis rotation offset

    void LateUpdate()
    {
        if (headTransform != null)
        {
            // Calculate the new position by adding the offset to the head's position without applying the head's rotation
            Vector3 newPosition = headTransform.position + positionOffset;

            // Apply the calculated position
            transform.position = newPosition;

            // Extract only the y-rotation component from the head's rotation and apply the y-axis rotation offset
            Quaternion newRotation = Quaternion.Euler(0, headTransform.rotation.eulerAngles.y + rotationOffsetY, 0);

            // Update the rotation of the chestmail to match the head's rotation around the y-axis with an offset
            transform.rotation = newRotation;
        }
    }
}
