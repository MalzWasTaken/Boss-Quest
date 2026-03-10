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

    private float currentYaw = 0f;
    private float targetYaw = 0f;


    void LateUpdate()
    {
        if (target == null) return;

        //rotating camera when holding rmb
        if (Mouse.current.rightButton.isPressed)
        {
            //free look
            
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            currentYaw += mouseDelta.x * mouseSensitivity;
        }
        else
        {
            //player moving check
            Rigidbody rb = target.GetComponent<Rigidbody>();
            bool playerMoving = rb != null && rb.linearVelocity.magnitude > 0.1f;

            if (playerMoving)
            {
                
                //smooth snap back to facing direction
                targetYaw = Mathf.Atan2(target.forward.x, target.forward.z) * Mathf.Rad2Deg;
                currentYaw = Mathf.LerpAngle(currentYaw,targetYaw,transitionSpeed * Time.deltaTime);       
            }
        }
        //position calculation
        Quaternion rotation = Quaternion.Euler(0f, currentYaw,0f);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0f, height, -distance);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}