using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    private CameraController cameraController;
    private Vector3 target;
    private float pitchAngle = 45f;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = GetComponentInParent<CameraController>();
        target = cameraController.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate camera to look at target
        target = cameraController.transform.position;
        transform.LookAt(target);

        // Get the pitch angle of the camera
        pitchAngle = cameraController.pitch;
        // Change the pitch of the camera
        transform.eulerAngles = new Vector3(pitchAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
