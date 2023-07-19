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
    [HideInInspector] public float pitch;

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
        // Fast camera movement event handlers
        input.Camera.FastPan.performed += ctx => moveSpeed *= 2f;
        input.Camera.FastPan.canceled += ctx => moveSpeed /= 2f;
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

        Debug.Log("Distance: " + projectedDistance);

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
        //mainCamera.transform.localRotation = Quaternion.Euler(pitchAngle, 0f, 0f);

        // Move the camera to the new position
        mainCamera.transform.position = newCameraPos;
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
}
