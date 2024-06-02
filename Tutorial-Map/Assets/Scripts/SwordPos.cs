using UnityEngine;
using UnityEngine.InputSystem;

public class SwordHandler : MonoBehaviour
{
    public Transform backPosition; // Reference to the sword's position on the back
    public Transform handTransform; // Reference to the player's hand transform
    public GameObject sword; // Reference to the sword GameObject

    private ConfigurableJoint swordJoint;
    private bool isSwordGrabbed = false;

    // Input Action References
    public InputActionReference grabAction;

    void Start()
    {
        // Place the sword on the player's back initially
        sword.transform.position = backPosition.position;
        sword.transform.rotation = backPosition.rotation;

        // Enable the grab action and set up callbacks
        grabAction.action.performed += OnGrab;
        grabAction.action.canceled += OnRelease;
        grabAction.action.Enable();
    }

    void OnDestroy()
    {
        // Clean up the callbacks
        grabAction.action.performed -= OnGrab;
        grabAction.action.canceled -= OnRelease;
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        GrabSword();
    }

    private void OnRelease(InputAction.CallbackContext context)
    {
        ReleaseSword();
    }

    void GrabSword()
    {
        isSwordGrabbed = true;

        // Move the sword to the hand
        sword.transform.position = handTransform.position;
        sword.transform.rotation = handTransform.rotation;

        // Add and configure the joint
        swordJoint = sword.AddComponent<ConfigurableJoint>();
        swordJoint.connectedBody = handTransform.GetComponent<Rigidbody>();

        // Configure the joint
        ConfigureJoint(swordJoint);
    }

    void ReleaseSword()
    {
        isSwordGrabbed = false;

        // Destroy the joint
        if (swordJoint != null)
        {
            Destroy(swordJoint);
        }

        // Move the sword back to the player's back
        sword.transform.position = backPosition.position;
        sword.transform.rotation = backPosition.rotation;
    }

    void ConfigureJoint(ConfigurableJoint joint)
    {
        // Configure anchor points
        joint.anchor = Vector3.zero; // Adjust to the handle's position if necessary
        joint.connectedAnchor = Vector3.zero;

        // Allow linear motion within limits
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        // Allow angular motion within limits
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        // Configure linear limit
        SoftJointLimit linearLimit = new SoftJointLimit
        {
            limit = 0.1f // Small limit to prevent large positional deviation
        };
        joint.linearLimit = linearLimit;

        // Configure angular limit
        SoftJointLimit angularLimit = new SoftJointLimit
        {
            limit = 45f // Adjust as needed for the desired angular limit
        };
        joint.lowAngularXLimit = angularLimit;
        joint.highAngularXLimit = angularLimit;
        joint.angularYLimit = angularLimit;
        joint.angularZLimit = angularLimit;

        // Configure linear drive (strong spring force)
        JointDrive linearDrive = new JointDrive
        {
            positionSpring = 20000f, // Stronger spring force for faster return
            positionDamper = 300f, // Adjusted damping to balance speed and stability
            maximumForce = Mathf.Infinity
        };
        joint.xDrive = linearDrive;
        joint.yDrive = linearDrive;
        joint.zDrive = linearDrive;

        // Configure angular drive (weaker spring force)
        JointDrive angularDrive = new JointDrive
        {
            positionSpring = 2000f, // Weaker spring force for smoother angular motion
            positionDamper = 300f, // Adjusted damping to balance speed and stability
            maximumForce = Mathf.Infinity
        };
        joint.angularXDrive = angularDrive;
        joint.angularYZDrive = angularDrive;
        joint.slerpDrive = angularDrive;
    }
}
