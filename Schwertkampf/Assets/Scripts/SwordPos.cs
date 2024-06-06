using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SwordHandler : MonoBehaviour
{
    public Transform backPosition; // Reference to the sword's position on the back
    public GameObject sword; // Reference to the sword GameObject
    public Transform wristTransform; // Reference to the wrist transform

    private ConfigurableJoint swordJoint;
    private bool isSwordGrabbed = false;

    // Reference to the XR Grab Interactable
    private XRGrabInteractable grabInteractable;
    private Rigidbody swordRigidbody;

    void Start()
    {
        // Get the Rigidbody component on the sword
        swordRigidbody = sword.GetComponent<Rigidbody>();

        if (swordRigidbody == null)
        {
            Debug.LogError("No Rigidbody component found on the sword.");
            return;
        }

        // Place the sword on the player's back initially
        PlaceSwordOnBack();

        // Get the XRGrabInteractable component on the sword
        grabInteractable = sword.GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("No XRGrabInteractable component found on the sword.");
            return;
        }

        // Register for interaction events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        Debug.Log("SwordHandler initialized. Sword placed at back position.");
    }

    void OnDestroy()
    {
        // Clean up the callbacks
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void Update()
    {
        // Update the sword's position to stay on the back if not grabbed
        if (!isSwordGrabbed)
        {
            PlaceSwordOnBack();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor)
        {
            Debug.Log("Sword grabbed.");
            Transform handTransform = args.interactorObject.transform;
            GrabSword(handTransform);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor)
        {
            Debug.Log("Sword released.");
            ReleaseSword();
        }
    }

    void GrabSword(Transform handTransform)
    {
        if (isSwordGrabbed)
        {
            Debug.LogWarning("Sword is already grabbed.");
            return;
        }

        isSwordGrabbed = true;

        // Enable sword physics
        swordRigidbody.useGravity = true;
        swordRigidbody.isKinematic = false;

        // Detach the sword from any parent
        sword.transform.SetParent(null);

        // Move the sword to the hand's position and rotation
        sword.transform.position = handTransform.position;
        sword.transform.rotation = handTransform.rotation;

        // Add and configure the joint
        swordJoint = sword.AddComponent<ConfigurableJoint>();
        ConfigureJoint(swordJoint);

        Debug.Log("Sword joint created and configured.");

        // Disable the XRGrabInteractable to prevent it from interfering with the joint
        grabInteractable.enabled = false;
    }

    void ReleaseSword()
    {
        if (!isSwordGrabbed)
        {
            Debug.LogWarning("Sword is not grabbed.");
            return;
        }

        isSwordGrabbed = false;

        // Destroy the joint
        if (swordJoint != null)
        {
            Destroy(swordJoint);
            Debug.Log("Sword joint destroyed.");
        }

        // Place the sword back on the player's back
        PlaceSwordOnBack();

        // Re-enable the XRGrabInteractable
        grabInteractable.enabled = true;
    }

    void PlaceSwordOnBack()
    {
        // Disable sword physics
        swordRigidbody.useGravity = false;
        swordRigidbody.isKinematic = true;

        // Move the sword to the back position
        sword.transform.position = backPosition.position;
        sword.transform.rotation = backPosition.rotation;

        // Ensure the sword is not a child of any object
        sword.transform.SetParent(null);

        //Debug.Log("Sword placed at back position.");
    }

    void ConfigureJoint(ConfigurableJoint joint)
    {
        joint.connectedBody = wristTransform.GetComponent<Rigidbody>();

        // Configure anchor points
        joint.anchor = Vector3.zero; // Adjust to the handle's position if necessary
        joint.connectedAnchor = Vector3.zero;

        // Lock linear motion
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Allow angular motion
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

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
            positionSpring = 2000f, // Stronger spring force for faster return
            positionDamper = 300f, // Adjusted damping to balance speed and stability
            maximumForce = Mathf.Infinity
        };
        joint.angularXDrive = angularDrive;
        joint.angularYZDrive = angularDrive;
        joint.slerpDrive = angularDrive;

        // Configure linear limit spring
        SoftJointLimitSpring linearLimitSpring = new SoftJointLimitSpring
        {
            spring = 2000f, // Adjust as needed
            damper = 300f // Adjust as needed
        };
        joint.linearLimitSpring = linearLimitSpring;

        // Configure angular limit spring
        SoftJointLimitSpring angularLimitSpring = new SoftJointLimitSpring
        {
            spring = 2000f, // Adjust as needed
            damper = 300f // Adjust as needed
        };
        joint.angularXLimitSpring = angularLimitSpring;
        joint.angularYZLimitSpring = angularLimitSpring;

        Debug.Log("Joint configured with linear and angular drives.");
    }
}
