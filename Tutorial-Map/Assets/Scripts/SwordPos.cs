using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    public Transform backPosition; // Reference to the sword's position on the back
    public Transform handTransform; // Reference to the player's hand transform
    public GameObject sword; // Reference to the sword GameObject

    private ConfigurableJoint swordJoint;
    private bool isSwordGrabbed = false;

    void Start()
    {
        // Place the sword on the player's back initially
        sword.transform.position = backPosition.position;
        sword.transform.rotation = backPosition.rotation;

        // Get the sword's ConfigurableJoint component
        swordJoint = sword.GetComponent<ConfigurableJoint>();

        // Disable the joint initially
        swordJoint.enabled = false;
    }

    void Update()
    {
        // Detect grab button press
        if (Input.GetButtonDown("Grab"))
        {
            GrabSword();
        }

        // Detect grab button release
        if (Input.GetButtonUp("Grab"))
        {
            ReleaseSword();
        }
    }

    void GrabSword()
    {
        isSwordGrabbed = true;

        // Move the sword to the hand
        sword.transform.position = handTransform.position;
        sword.transform.rotation = handTransform.rotation;

        // Enable the joint and connect it to the hand
        swordJoint.connectedBody = handTransform.GetComponent<Rigidbody>();
        swordJoint.enabled = true;
    }

    void ReleaseSword()
    {
        isSwordGrabbed = false;

        // Disable the joint
        swordJoint.enabled = false;

        // Move the sword back to the player's back
        sword.transform.position = backPosition.position;
        sword.transform.rotation = backPosition.rotation;
    }
}