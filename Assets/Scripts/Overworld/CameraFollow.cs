using UnityEngine;
using UnityEngine.InputSystem;


public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -6f);
    public float followSpeed = 10f;
    public float mouseSensitivity = 2f;
    public float distance = 6f;
    public float height = 2f;
    public float transitionSpeed = 5f;

    [Header("Camera Collision")]
    public LayerMask collisionLayers;       // assign Ground/Wall layers in Inspector
    public float collisionRadius = 0.3f;    // sphere radius for the cast
    public float collisionBuffer = 0.2f;    // pull camera this far in from wall

    private float currentYaw = 0f;
    private float targetYaw = 0f;
    private Rigidbody targetRb;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (target != null)
            targetRb = target.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        currentYaw += mouseDelta.x * mouseSensitivity;

        bool playerMoving = targetRb != null && targetRb.linearVelocity.magnitude > 0.1f;
        bool isStrafing = Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed;
        bool isForwardBack = Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed;

        if (playerMoving && isForwardBack && !isStrafing)
        {
            targetYaw = Mathf.Atan2(target.forward.x, target.forward.z) * Mathf.Rad2Deg;
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, transitionSpeed * Time.deltaTime);
        }

        // Calculate desired position (where camera WANTS to be)
        Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0f, height, -distance);

        // Camera collision check — SphereCast from player toward desired camera position
        Vector3 lookAtPoint = target.position + Vector3.up; // matches your LookAt target
        Vector3 castDirection = desiredPosition - lookAtPoint;
        float castDistance = castDirection.magnitude;

        Vector3 finalPosition = desiredPosition;
        if (castDistance > 0.01f && Physics.SphereCast(lookAtPoint, collisionRadius, 
                castDirection.normalized, out RaycastHit hit, castDistance, collisionLayers))
        {
            // Wall hit — pull camera in to hit point, minus buffer
            finalPosition = hit.point - castDirection.normalized * collisionBuffer;
        }

        transform.position = Vector3.Lerp(transform.position, finalPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}