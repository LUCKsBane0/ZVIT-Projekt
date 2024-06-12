using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class SwordHandler2 : MonoBehaviour
{
    public Transform backPosition; // Reference to the sword's position on the back
    public GameObject sword; // Reference to the sword GameObject
    public InputActionReference grabAction; // Reference to the grab action
    public XRBaseInteractor rightHandInteractor; // Reference to the XR interactor for the right hand
    public GameObject rightHand;
    public GameObject Wrist;
    private ConfigurableJoint wristJoint;
    private Rigidbody swordRigidbody;
    private bool isSwordGrabbed = false;
    private bool isHandTouchingSword = false;
    private GameObject WristInstatiated;

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
        Debug.Log($"Grab button pressed: {isGrabButtonPressed}");

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
        swordRigidbody.useGravity = false;
        swordRigidbody.isKinematic = false;

        // Set sword position and rotation to match the hand
       
        WristInstatiated = Instantiate(Wrist,rightHand.GetComponent<Transform>().position, rightHand.GetComponent<Transform>().rotation);
        sword.transform.SetParent(WristInstatiated.transform);
        wristJoint = WristInstatiated.GetComponent<ConfigurableJoint>();
        wristJoint.connectedBody = swordRigidbody;
        WristInstatiated.transform.SetParent(rightHand.transform);
        //sword.transform.localPosition = swordHandPosition.localPosition;
        //sword.transform.localRotation = swordHandPosition.localRotation;

        // Attach the sword to the wrist
        CreateConfigurableJoint();
    }

    void ReleaseSword()
    {
        isSwordGrabbed = false;

        // Destroy the joint
        

        // Detach the sword from the wrist
        sword.transform.SetParent(null);
        DestroyImmediate(WristInstatiated,true);
        // Place the sword back on the player's back
        PlaceSwordOnBack();
    }

    void PlaceSwordOnBack()
    {
        Debug.Log("Placing sword on back");

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


        // Configure anchor points
     

       
       
    }
}
