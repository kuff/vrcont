using UnityEngine;

public class VRMovement : MonoBehaviour
{
    public enum TurnMode
    {
        SnapTurn,
        ContinuousTurn
    }
    
    public Transform playerCamera;
    public TurnMode turnMode = TurnMode.SnapTurn;
    public OVRInput.Controller continuousMovementController = OVRInput.Controller.LTouch;
    public OVRInput.Controller turnController = OVRInput.Controller.RTouch;
    public float movementSpeed = 3.0f;
    public float turningSpeed = 45.0f;
    public float snapTurnAngle = 45.0f;

    private void Update()
    {
        // Handle continuous movement
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, continuousMovementController).y != 0)
        {
            // Get the camera's forward and right directions
            Vector3 forwardDirection = Camera.main.transform.forward;
            Vector3 rightDirection = Camera.main.transform.right;

            forwardDirection.y = 0;
            rightDirection.y = 0;
            forwardDirection.Normalize();
            rightDirection.Normalize();

            // Move the object based on thumbstick input for movement and strafing
            Vector3 movement = (forwardDirection * OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, continuousMovementController).y +
                                rightDirection * OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, continuousMovementController).x) * (movementSpeed * Time.deltaTime);

            transform.Translate(movement, Space.World);
        }

        // Handle turning based on selected turn mode
        float rotationAmount = 0;

        if (turnMode == TurnMode.ContinuousTurn)
        {
            rotationAmount = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, turnController).x * turningSpeed * Time.deltaTime;
        }
        else if (turnMode == TurnMode.SnapTurn && OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, turnController))
        {
            rotationAmount = snapTurnAngle;
        }
        else if (turnMode == TurnMode.SnapTurn && OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, turnController))
        {
            rotationAmount = -snapTurnAngle;
        }

        transform.Rotate(0, rotationAmount, 0);
    }
}