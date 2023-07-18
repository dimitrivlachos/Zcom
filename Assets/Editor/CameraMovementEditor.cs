#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraController cameraController = (CameraController)target;
        cameraController.moveSpeed = EditorGUILayout.Slider("Move Speed", cameraController.moveSpeed, 1f, 100f);
        cameraController.rotateSpeed = EditorGUILayout.Slider("Rotate Speed", cameraController.rotateSpeed, 1f, 100f);
        cameraController.zoomSpeed = EditorGUILayout.Slider("Zoom Speed", cameraController.zoomSpeed, 1f, 100f);
    }
}
#endif
