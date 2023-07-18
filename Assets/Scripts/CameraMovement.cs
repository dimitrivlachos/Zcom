using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Controls input = null;
    private Vector2 moveVector = Vector2.zero;

    [SerializeField, Range(1f,25f)] private float moveSpeed = 5f; // Range of 1 to 25, default of 5

    private void OnInspectorGUI()
    {
        // Create a slider for the moveSpeed variable
        moveSpeed = EditorGUILayout.Slider("Move Speed", moveSpeed, 1f, 25f);
    }

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
