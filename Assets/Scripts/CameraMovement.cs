using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Controls input = null;
    private Vector2 moveVector = Vector2.zero;

    private void Awake()
    {
        input = new Controls();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Camera.Movement.performed += ctx => OnMovementPerformed(ctx);
        input.Camera.Movement.canceled += ctx => OnMovementCancelled(ctx);
    }

    private void OnDisable()
    {
        input.Disable();
        input.Camera.Movement.performed -= ctx => OnMovementPerformed(ctx);
        input.Camera.Movement.canceled -= ctx => OnMovementCancelled(ctx);
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(moveVector.x, 0f, moveVector.y);
        transform.position += move * Time.deltaTime;
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }
}
