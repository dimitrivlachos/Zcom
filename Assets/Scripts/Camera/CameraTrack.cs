using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    private CameraController cameraController;
    private Vector3 target;

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
    }
}
