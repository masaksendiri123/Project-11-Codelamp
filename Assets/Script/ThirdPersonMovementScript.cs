using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ThirdPersonMovementScript : MonoBehaviour
{

    //Reference To Other GameComponent
    [Header("Component References")]
    public CharacterController controller;
    public Animator animator;

    [Header("Input Actions")]
    public InputActionReference movementAction;
    public InputActionReference runningAction;
    public InputActionReference jumpAction;
    public InputActionReference aimingAction;
    public InputActionReference shootAction;
    public InputActionReference reloadAction;

    [Header("Camera Reference")]
    public Transform playerCamera;

    //Variable For Movement Stuff
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 9f;
    public float jumpHeight = 2f;
    public float turnSmoothTime = 0.1f;
    public float customGravityValue = 9.8f;
    float turnSmoothVelocity;

    private void Start()
    {
        //Menghilangkan Cursor Saat Game Dimulai
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //Membaca Data Movement Dari Input System
        float horizontalAxis = movementAction.action.ReadValue<Vector2>().x;
        float verticalAxis = movementAction.action.ReadValue<Vector2>().y;
        Vector3 direction = new Vector3(horizontalAxis, 0f, verticalAxis).normalized;

        bool aiming = aimingAction.action.ReadValue<float>() > 0.1f;

        if (aiming)
        {
            bool shooting = shootAction.action.ReadValue<float>() > 0.1f;
            bool reloading = reloadAction.action.ReadValue<float>() > 0.1f;
            animator.SetBool("isAiming", true);
            if (shooting){ animator.SetTrigger("OnShot"); }
            if (reloading){ animator.SetTrigger("OnReload"); }
        }
        else
        {
            animator.SetBool("isAiming", false);
        }
        if (direction.magnitude >= 0.1f && controller.isGrounded)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            Animation(moveDirection, direction);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }

        if (!controller.isGrounded)
        {
            controller.Move(new Vector3(0f, -customGravityValue, 0f) * Time.deltaTime);
        }
    }

    private void Animation(Vector3 m_moveDirection, Vector3 m_direction)
    {
        bool isRunning = runningAction.action.ReadValue<float>() > 0.1f;

        if (isRunning)
        {
            controller.Move(m_moveDirection.normalized * runSpeed * Time.deltaTime);
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", true);
        }
        else if (!isRunning && m_direction.magnitude >= 0.1f)
        {
            controller.Move(m_moveDirection.normalized * walkSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }
    }
}
