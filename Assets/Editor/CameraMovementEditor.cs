#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraMovement))]
public class CameraMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraMovement cameraMovement = (CameraMovement)target;
        cameraMovement.moveSpeed = EditorGUILayout.Slider("Move Speed", cameraMovement.moveSpeed, 1f, 25f);
        cameraMovement.rotateSpeed = EditorGUILayout.Slider("Rotate Speed", cameraMovement.rotateSpeed, 1f, 25f);
    }
}
#endif
