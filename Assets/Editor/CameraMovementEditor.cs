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
    private SerializedProperty minZoomAngle;
    private SerializedProperty maxZoomAngle;

    private void OnEnable()
    {
        moveSpeedProp = serializedObject.FindProperty("moveSpeed");
        rotateSpeedProp = serializedObject.FindProperty("rotateSpeed");
        zoomSpeedProp = serializedObject.FindProperty("zoomSpeed");
        minZoomDistanceProp = serializedObject.FindProperty("minZoomDistance");
        maxZoomDistanceProp = serializedObject.FindProperty("maxZoomDistance");
        mouseZoomSpeed = serializedObject.FindProperty("mouseZoomSpeed");
        minZoomAngle = serializedObject.FindProperty("minZoomAngle");
        maxZoomAngle = serializedObject.FindProperty("maxZoomAngle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Slider(moveSpeedProp, 1f, 100f, "Move Speed");
        EditorGUILayout.Slider(rotateSpeedProp, 1f, 100f, "Rotate Speed");
        EditorGUILayout.Slider(zoomSpeedProp, 1f, 100f, "Zoom Speed");
        EditorGUILayout.Slider(mouseZoomSpeed, 1f, 10f, "Mouse Zoom Speed");
        EditorGUILayout.Slider(minZoomAngle, 10f, 30f, "Min Zoom Angle");
        EditorGUILayout.Slider(maxZoomAngle, 30f, 60f, "Max Zoom Angle");

        EditorGUILayout.PropertyField(minZoomDistanceProp);
        EditorGUILayout.PropertyField(maxZoomDistanceProp);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
