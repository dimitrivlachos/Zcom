using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Private class variables
    private Camera mainCamera;
    private Controls input = null;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 zoomRotateVector = Vector2.zero;
    private Vector3 rotateDragStartPosition;        // The position of the mouse when the drag starts
    private Vector3 rotateDragCurrentPosition;      // The position of the mouse as it is dragged
    private bool isHoldingMMB = false;              // Is the user holding the middle mouse button
    private Vector3 panDragStartPosition;           // The position of the mouse when the drag starts
    private Vector3 panDragCurrentPosition;         // The position of the mouse as it is dragged
    private bool isHoldingRMB = false;              // Is the user holding the right mouse button

    // Camera settings
    [SerializeField] private float minZoomDistance = 5f;    // Range of 1 to 100, default of 5
    [SerializeField] private float maxZoomDistance = 50f;   // Range of 1 to 100, default of 50
    [SerializeField] private float moveSpeed = 25f;         // Range of 1 to 100, default of 25
    [SerializeField] private float rotateSpeed = 50f;       // Range of 1 to 100, default of 50
    [SerializeField] private float zoomSpeed = 50f;         // Range of 1 to 100, default of 50
    [SerializeField] private float mouseZoomSpeed = 5f;     // Range of 1 to 100, default of 5

    /*
     * The target is the position that the camera track will look at.
     * The pitch is the up and down rotation of the camera.
     * These are public so that script can access them.
     * We use HideInInspector so that they don't show up in the inspector.
     */
    [HideInInspector] public Vector3 target;
    [HideInInspector] public float pitch;

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
        // Fast camera movement event handlers
        input.Camera.FastPan.performed += ctx => moveSpeed *= 2f;
        input.Camera.FastPan.canceled += ctx => moveSpeed /= 2f;
        // Mouse rotation event handlers
        input.Camera.MouseRotate.performed += OnMouseRotatePerformed;
        input.Camera.MouseRotate.canceled += ctx => isHoldingMMB = false;
        // Mouse pan event handlers
        input.Camera.MousePan.performed += OnMousePanPerformed;
        input.Camera.MousePan.canceled += ctx => isHoldingRMB = false;
        // Mouse zoom event handlers
        input.Camera.MouseZoom.performed += ctx => zoomRotateVector += ctx.ReadValue<Vector2>() * mouseZoomSpeed;
        input.Camera.MouseZoom.canceled += ctx => zoomRotateVector = Vector2.zero;
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
        // Fast camera movement event handlers
        input.Camera.FastPan.performed -= ctx => moveSpeed *= 2f;
        input.Camera.FastPan.canceled -= ctx => moveSpeed /= 2f;
        // Mouse rotation event handlers
        input.Camera.MouseRotate.performed -= OnMouseRotatePerformed;
        input.Camera.MouseRotate.canceled -= ctx => isHoldingMMB = false;
        // Mouse pan event handlers
        input.Camera.MousePan.performed -= OnMousePanPerformed;
        input.Camera.MousePan.canceled -= ctx => isHoldingRMB = false;
        // Mouse zoom event handlers
        input.Camera.MouseZoom.performed -= ctx => zoomRotateVector += ctx.ReadValue<Vector2>();
        input.Camera.MouseZoom.canceled -= ctx => zoomRotateVector = Vector2.zero;
    }

    private void Update()
    {
        // Update target so camera track can look at it
        target = transform.position;

        // Draw a line from the camera to the target
        Debug.DrawLine(mainCamera.transform.position, target, Color.red);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
        
        HandleMousePan();
        HandleMouseZoom();
        HandleMouseRotation();
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

        // Vector from camera to target
        Vector3 zoomVector = Vector3.Normalize(target - mainCamera.transform.position) * zoomAmount;

        // Calculate camera's new position
        Vector3 newCameraPos = mainCamera.transform.position;
        newCameraPos += Time.deltaTime * zoomSpeed * zoomVector;

        // Calculate distance to new position
        float projectedDistance = Vector3.Distance(newCameraPos, transform.position);

        // Guard statements
        if (projectedDistance < minZoomDistance) return;
        if (projectedDistance > maxZoomDistance) return;

        // Calculate the clamped distance for zoom
        float clampedDistance = Mathf.Clamp(projectedDistance, minZoomDistance, maxZoomDistance);

        // Calculate the ratio of how far we have zoomed between the minZoomDistance and maxZoomDistance
        float zoomRatio = (clampedDistance - minZoomDistance) / (maxZoomDistance - minZoomDistance);

        // Calculate the pitch (up and down rotation) based on the zoomRatio
        float pitchAngle = ExponentialInterpolation(30f, 45f, zoomRatio);

        // Apply the pitch rotation to the camera's transform
        pitch = pitchAngle;

        // Move the camera to the new position
        mainCamera.transform.position = newCameraPos;
    }

    private void HandleMouseZoom()
    {
        // Get the scroll amount
        float scrollAmount = Input.mouseScrollDelta.y * mouseZoomSpeed;

        // Position on the ground we are scrolling towards
        Vector3 zoomTarget = ScreenToGroundRay();

        Vector3 targetVector = Vector3.Normalize(zoomTarget - transform.position) * scrollAmount;

        // Vector from camera to target
        Vector3 zoomVector = Vector3.Normalize(target - mainCamera.transform.position) * scrollAmount;

        // Calculate camera's new position
        Vector3 newCameraPos = mainCamera.transform.position;
        newCameraPos += Time.deltaTime * zoomSpeed * zoomVector;

        // Calculate distance to new position
        float projectedDistance = Vector3.Distance(newCameraPos, transform.position);

        // Guard statements
        if (projectedDistance < minZoomDistance) return;
        if (projectedDistance > maxZoomDistance) return;

        // Calculate the clamped distance for zoom
        float clampedDistance = Mathf.Clamp(projectedDistance, minZoomDistance, maxZoomDistance);

        // Calculate the ratio of how far we have zoomed between the minZoomDistance and maxZoomDistance
        float zoomRatio = (clampedDistance - minZoomDistance) / (maxZoomDistance - minZoomDistance);

        // Calculate the pitch (up and down rotation) based on the zoomRatio
        float pitchAngle = ExponentialInterpolation(30f, 45f, zoomRatio);

        // Apply the pitch rotation to the camera's transform
        pitch = pitchAngle;

        // Move the camera to the new position
        mainCamera.transform.position = newCameraPos;

        // Move transform towards the target
        transform.position += targetVector;
    }

    private void HandleRotation()
    {
        // Rotate transform
        float rotation = zoomRotateVector.x;

        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime * rotation);
    }

    private float ExponentialInterpolation(float min, float max, float t)
    {
        return Mathf.Pow(max / min, t) * min;
    }

    private void OnMouseRotatePerformed(InputAction.CallbackContext ctx)
    {
        // Set the flag to true
        isHoldingMMB = true;

        rotateDragStartPosition = ScreenToGroundRay();
    }

    private void HandleMouseRotation()
    {
        // Only rotate if the flag is true
        if (!isHoldingMMB) return;

        rotateDragCurrentPosition = ScreenToGroundRay();

        //Calculate the unit vectors of the two vectors from the transform
        Vector3 from = Vector3.Normalize(rotateDragStartPosition - transform.position);
        Vector3 to = Vector3.Normalize(rotateDragCurrentPosition - transform.position);

        // Calculate the angle between the two vectors
        float angle = Vector3.SignedAngle(to, from, Vector3.up);

        // Rotate the transform
        transform.Rotate(Vector3.up, angle);

        // Update the start position
        rotateDragStartPosition = ScreenToGroundRay();
    }

    private void OnMousePanPerformed(InputAction.CallbackContext ctx)
    {
        // Set the flag to true
        isHoldingRMB = true;

        panDragStartPosition = ScreenToGroundRay();
    }

    private void HandleMousePan()
    {
        // Only pan if the flag is true
        if (!isHoldingRMB) return;

        panDragCurrentPosition = ScreenToGroundRay();

        // Calculate the difference between the two vectors
        Vector3 newPosition = transform.position + panDragStartPosition - panDragCurrentPosition;

        // Move the camera to the new position
        transform.position = newPosition;
    }

    private Vector3 ScreenToGroundRay()
    {
        // Create a plane at the origin whose normal points up
        Plane plane = new(Vector3.up, Vector3.zero);

        // Create a ray out from the camera at the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Find the point where the ray intersects the plane
        if (plane.Raycast(ray, out float entry))
        {
            // Get the point on the ray where the intersection occurs
            return ray.GetPoint(entry);
        }

        return Vector3.zero;
    }
}
