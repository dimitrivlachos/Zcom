#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    private SerializedProperty moveSpeedProp;
    private SerializedProperty rotateSpeedProp;
    private SerializedProperty zoomSpeedProp;
    private SerializedProperty minZoomDistanceProp;
    private SerializedProperty maxZoomDistanceProp;
    private SerializedProperty mouseZoomSpeed;

    private void OnEnable()
    {
        moveSpeedProp = serializedObject.FindProperty("moveSpeed");
        rotateSpeedProp = serializedObject.FindProperty("rotateSpeed");
        zoomSpeedProp = serializedObject.FindProperty("zoomSpeed");
        minZoomDistanceProp = serializedObject.FindProperty("minZoomDistance");
        maxZoomDistanceProp = serializedObject.FindProperty("maxZoomDistance");
        mouseZoomSpeed = serializedObject.FindProperty("mouseZoomSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Slider(moveSpeedProp, 1f, 100f, "Move Speed");
        EditorGUILayout.Slider(rotateSpeedProp, 1f, 100f, "Rotate Speed");
        EditorGUILayout.Slider(zoomSpeedProp, 1f, 100f, "Zoom Speed");
        EditorGUILayout.Slider(mouseZoomSpeed, 1f, 10f, "Mouse Zoom Speed");

        EditorGUILayout.PropertyField(minZoomDistanceProp);
        EditorGUILayout.PropertyField(maxZoomDistanceProp);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
