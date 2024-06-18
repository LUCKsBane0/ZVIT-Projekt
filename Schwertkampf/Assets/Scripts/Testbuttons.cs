using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ControllerButtonListener : MonoBehaviour
{
    public InputActionReference leftHandAction;  // Reference to the left hand action
    public InputActionReference rightHandAction; // Reference to the right hand action

    void OnEnable()
    {
        leftHandAction.action.Enable();
        rightHandAction.action.Enable();

        leftHandAction.action.performed += OnLeftHandButtonPressed;
        rightHandAction.action.performed += OnRightHandButtonPressed;
    }

    void OnDisable()
    {
        leftHandAction.action.performed -= OnLeftHandButtonPressed;
        rightHandAction.action.performed -= OnRightHandButtonPressed;

        leftHandAction.action.Disable();
        rightHandAction.action.Disable();
    }

    private void OnLeftHandButtonPressed(InputAction.CallbackContext context)
    {
        var control = context.control;
        Debug.Log("Left Hand Button Pressed: " + control);
    }

    private void OnRightHandButtonPressed(InputAction.CallbackContext context)
    {
        var control = context.control;
        Debug.Log("Right Hand Button Pressed: " + control);
    }
}