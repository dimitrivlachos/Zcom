#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraController cameraController = (CameraController)target;
        cameraController.moveSpeed = EditorGUILayout.Slider("Move Speed", cameraController.moveSpeed, 1f, 25f);
        cameraController.rotateSpeed = EditorGUILayout.Slider("Rotate Speed", cameraController.rotateSpeed, 1f, 25f);
    }
}
#endif
