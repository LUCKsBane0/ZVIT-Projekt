using UnityEngine;

public class ConfigureSwordJoint : MonoBehaviour
{
    public Transform wristTransform; // Reference to the wrist transform

    void Start()
    {
        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
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
    }
}