using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Controls input = null;
    private Vector2 moveVector = Vector2.zero;

    public float moveSpeed = 5f; // Range of 1 to 25, default of 5

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
        Vector3 move = new(moveVector.x, 0f, moveVector.y);
        transform.position += moveSpeed * Time.deltaTime * move;
    }
}
