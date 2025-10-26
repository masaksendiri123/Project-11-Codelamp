using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovementScript : MonoBehaviour
{

    //Reference To Other GameComponent
    public CharacterController controller;
    public InputActionReference movementAction;
    public Transform playerCamera;

    //Variable For Movement Stuff
    public float walkSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    // Update is called once per frame
    void Update()
    {
        float horizontalAxis = movementAction.action.ReadValue<Vector2>().x;
        float verticalAxis = movementAction.action.ReadValue<Vector2>().y;
        Vector3 direction = new Vector3(horizontalAxis, 0f, verticalAxis).normalized;

        if (direction.magnitude >= 0.1f && controller.isGrounded)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * walkSpeed * Time.deltaTime);
        }

        if (!controller.isGrounded)
        {
            controller.Move(Physics.gravity * Time.deltaTime);
        }
    }
}
