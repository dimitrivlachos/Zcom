using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private Controls input = null;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 zoomRotateVector = Vector2.zero;

    [SerializeField] private float minZoomDistance = 5f;
    [SerializeField] private float maxZoomDistance = 50f;

    /*
     * The target is the position that the camera track will look at.
     * It is public so that the camera track can access it.
     */
    [HideInInspector] public Vector3 target;

    [SerializeField] private float moveSpeed = 25f; // Range of 1 to 100, default of 25
    [SerializeField] private float rotateSpeed = 50f; // Range of 1 to 100, default of 50
    [SerializeField] private float zoomSpeed = 50f; // Range of 1 to 100, default of 25

    private void Awake()
    {
        input = new Controls();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        target = transform.position;
    }

    private void OnEnable()
    {
        input.Enable();
        
        // Register callbacks for input actions

        // Camera movement event handlers
        input.Camera.Movement.performed += ctx => moveVector = ctx.ReadValue<Vector2>();
        input.Camera.Movement.canceled += ctx => moveVector = Vector2.zero;
        // Camera zoom/rotate event handlers
        input.Camera.ZoomRotate.performed += ctx => zoomRotateVector += ctx.ReadValue<Vector2>();
        input.Camera.ZoomRotate.canceled += ctx => zoomRotateVector = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Disable();

        // Unregister callbacks for input actions

        // Camera movement event handlers
        input.Camera.Movement.performed -= ctx => moveVector = ctx.ReadValue<Vector2>();
        input.Camera.Movement.canceled -= ctx => moveVector = Vector2.zero;
        // Camera zoom/rotate event handlers
        input.Camera.ZoomRotate.performed -= ctx => zoomRotateVector += ctx.ReadValue<Vector2>();
        input.Camera.ZoomRotate.canceled -= ctx => zoomRotateVector = Vector2.zero;
    }

    private void Update()
    {
        // Update target so camera track can look at it
        target = transform.position;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f; // Set the y-component to zero so that movement stays on the XZ plane.
        right.y = 0f;   // Set the y-component to zero to maintain horizontal movement.

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveVector.y + right * moveVector.x;
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void HandleZoom()
    {
        float zoomAmount = zoomRotateVector.y;

        Vector3 zoomVector = mainCamera.transform.forward * zoomAmount;

        // Calculate camera's new position
        Vector3 newCameraPos = mainCamera.transform.position;
        newCameraPos += Time.deltaTime * zoomSpeed * zoomVector;

        // Calculate distance to new position
        float projectedDistance = Vector3.Distance(newCameraPos, transform.position);

        // Guard statements to keep zoom within bounds
        if (projectedDistance < minZoomDistance) return;
        if (projectedDistance > maxZoomDistance) return;

        // If we make it past the guard statements, then move the camera
        mainCamera.transform.position += Time.deltaTime * zoomSpeed * zoomVector;
    }

    private void HandleRotation()
    {
        // Rotate transform
        float rotation = zoomRotateVector.x;

        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime * rotation);
    }
}
