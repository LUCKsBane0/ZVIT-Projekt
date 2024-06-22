using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class SwordHandler : MonoBehaviour
{
    public Transform backPosition; // Reference to the sword's position on the back
    public GameObject sword; // Reference to the sword GameObject
    public Transform wristTransform; // Reference to the wrist transform
    public Transform swordHandPosition; // Reference to the desired sword position in the hand
    public InputActionReference grabAction; // Reference to the grab action
    public XRBaseInteractor rightHandInteractor; // Reference to the XR interactor for the right hand

    private Rigidbody swordRigidbody;
    private bool isSwordGrabbed = false;
    private bool isHandTouchingSword = false;
    private ConfigurableJoint swordJoint;

    void Start()
    {
        swordRigidbody = sword.GetComponent<Rigidbody>();
        if (swordRigidbody == null)
        {
            Debug.LogError("No Rigidbody component found on the sword.");
            return;
        }

        // Ensure grab action is enabled
        grabAction.action.Enable();

        // Place the sword on the player's back initially
        PlaceSwordOnBack();
    }

    void Update()
    {
        if (!isSwordGrabbed)
        {
            PlaceSwordOnBack();
        }

        // Check for grab button press
        bool isGrabButtonPressed = grabAction.action.ReadValue<float>() > 0.5f;
        //Debug.Log($"Grab button pressed: {isGrabButtonPressed}");

        // If the grab button is pressed and the hand is touching the sword, grab the sword
        if (isGrabButtonPressed && isHandTouchingSword && !isSwordGrabbed)
        {
            Debug.Log("Grabbing sword");
            GrabSword();
        }

        // If the grab button is released and the sword is grabbed, release the sword
        if (!isGrabButtonPressed && isSwordGrabbed)
        {
            Debug.Log("Releasing sword");
            ReleaseSword();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") && other is CapsuleCollider)
        {
            Debug.Log("Hand touched sword");
            isHandTouchingSword = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand") && other is CapsuleCollider)
        {
            Debug.Log("Hand left sword");
            isHandTouchingSword = false;
        }
    }

    void GrabSword()
    {
        isSwordGrabbed = true;

        // Enable sword physics
        swordRigidbody.useGravity = true;
        swordRigidbody.isKinematic = false;

        // Set sword position and rotation to match the hand
        sword.transform.SetParent(wristTransform);
        sword.transform.localPosition = swordHandPosition.localPosition;
        sword.transform.localRotation = swordHandPosition.localRotation;

        SoundEffectsManager.instance.PlayGrabSwordSound();

        // Attach the sword to the wrist
        CreateConfigurableJoint();
    }

    void ReleaseSword()
    {
        isSwordGrabbed = false;

        SoundEffectsManager.instance.PlayReleaseSwordSound();

        // Destroy the joint
        if (swordJoint != null)
        {
            Debug.Log("Destroying joint");
            Destroy(swordJoint);
        }

        // Detach the sword from the wrist
        sword.transform.SetParent(null);

        // Place the sword back on the player's back
        PlaceSwordOnBack();
    }

    void PlaceSwordOnBack()
    {
        //Debug.Log("Placing sword on back");

        // Disable sword physics
        swordRigidbody.useGravity = false;
        swordRigidbody.isKinematic = true;

        // Move the sword to the back position
        sword.transform.position = backPosition.position;
        sword.transform.rotation = backPosition.rotation;
    }

    void CreateConfigurableJoint()
    {
        Debug.Log("Creating joint");

        swordJoint = sword.AddComponent<ConfigurableJoint>();
        swordJoint.connectedBody = wristTransform.GetComponent<Rigidbody>();

        // Configure anchor points
        swordJoint.anchor = Vector3.zero; // Adjust to the handle's position if necessary
        swordJoint.connectedAnchor = Vector3.zero;

        // Lock linear motion
        swordJoint.xMotion = ConfigurableJointMotion.Locked;
        swordJoint.yMotion = ConfigurableJointMotion.Locked;
        swordJoint.zMotion = ConfigurableJointMotion.Locked;

        // Allow angular motion
        swordJoint.angularXMotion = ConfigurableJointMotion.Free;
        swordJoint.angularYMotion = ConfigurableJointMotion.Free;
        swordJoint.angularZMotion = ConfigurableJointMotion.Free;

        // Configure linear drive (strong spring force)
        JointDrive linearDrive = new JointDrive
        {
            positionSpring = 20000f, // Stronger spring force for faster return
            positionDamper = 300f, // Adjusted damping to balance speed and stability
            maximumForce = Mathf.Infinity
        };
        swordJoint.xDrive = linearDrive;
        swordJoint.yDrive = linearDrive;
        swordJoint.zDrive = linearDrive;

        // Configure angular drive (weaker spring force)
        JointDrive angularDrive = new JointDrive
        {
            positionSpring = 2000f, // Stronger spring force for faster return
            positionDamper = 300f, // Adjusted damping to balance speed and stability
            maximumForce = Mathf.Infinity
        };
        swordJoint.angularXDrive = angularDrive;
        swordJoint.angularYZDrive = angularDrive;
        swordJoint.slerpDrive = angularDrive;

        // Configure linear limit spring
        SoftJointLimitSpring linearLimitSpring = new SoftJointLimitSpring
        {
            spring = 2000f, // Adjust as needed
            damper = 300f // Adjust as needed
        };
        swordJoint.linearLimitSpring = linearLimitSpring;

        // Configure angular limit spring
        SoftJointLimitSpring angularLimitSpring = new SoftJointLimitSpring
        {
            spring = 2000f, // Adjust as needed
            damper = 300f // Adjust as needed
        };
        swordJoint.angularXLimitSpring = angularLimitSpring;
        swordJoint.angularYZLimitSpring = angularLimitSpring;
    }
}
