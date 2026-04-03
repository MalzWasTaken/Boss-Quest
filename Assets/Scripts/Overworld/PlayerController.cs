using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    public Camera mainCamera;

    private Animator animator;
    private bool isGrounded = true;

    public string runBool = "isRunning";
    public string walkBool = "isWalking";
    public string jumpBool = "isJumping";

    // Ground check
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
            

        float horizontal = Keyboard.current.dKey.isPressed ? 1 : Keyboard.current.aKey.isPressed ? -1 : 0;
        float vertical = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;

        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * vertical + camRight * horizontal).normalized;

        bool isAnyWASDPressed = Keyboard.current.dKey.isPressed ||
                                Keyboard.current.aKey.isPressed ||
                                Keyboard.current.sKey.isPressed ||
                                Keyboard.current.wKey.isPressed;

        if (!isGrounded)
        {
            SetJumping(true);
        }
        else
        {
            SetJumping(false);

            if (isAnyWASDPressed && Keyboard.current.leftShiftKey.isPressed)
            {
                SetRunning(true);
                SetWalking(false);
            }
            else if (isAnyWASDPressed)
            {
                SetWalking(true);
                SetRunning(false);
            } 
            else
            {
                SetRunning(false);
                SetWalking(false);
            }
        
        }
           
        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            SetJumping(true);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if (moveDirection.magnitude > 0)
        {
            bool isRunning = Keyboard.current.leftShiftKey.isPressed;
            float currentSpeed = isRunning ? runSpeed : moveSpeed;
            rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }


    public void SetJumping(bool value) => animator?.SetBool(jumpBool, value);
    public void SetRunning(bool value) => animator?.SetBool(runBool, value);
    public void SetWalking(bool value) => animator?.SetBool(walkBool, value);
}