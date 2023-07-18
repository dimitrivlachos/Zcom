using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Controls input = null;
    private Vector2 moveVector = Vector2.zero;

    [HideInInspector] public float targetY = 0f; // Target y-position of the camera

    public float moveSpeed = 5f; // Range of 1 to 25, default of 5
    public float rotateSpeed = 5f; // Range of 1 to 25, default of 5

    private void Awake()
    {
        input = new Controls();
    }

    private void OnEnable()
    {
        // Enable input
        input.Enable();
        // Add event listeners for movement
        input.Camera.Movement.performed += ctx => moveVector = ctx.ReadValue<Vector2>(); // Read value of movement vector and store it
        input.Camera.Movement.canceled += ctx => moveVector = Vector2.zero; // Reset movement vector to zero
    }

    private void OnDisable()
    {
        // Disable input
        input.Disable();
        // Remove event listeners for movement
        input.Camera.Movement.performed -= ctx => moveVector = ctx.ReadValue<Vector2>(); // Read value of movement vector and store it
        input.Camera.Movement.canceled -= ctx => moveVector = Vector2.zero; // Reset movement vector to zero
    }

    private void FixedUpdate()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f; // Set the y-component to zero so that movement stays on the XZ plane.
        right.y = 0f;   // Set the y-component to zero to maintain horizontal movement.

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveVector.y + right * moveVector.x;
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }
}
