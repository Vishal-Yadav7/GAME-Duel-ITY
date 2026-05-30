using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Joystick), true)]
public class JoystickEditor : Editor
{
    private SerializedProperty handleRange;
    private SerializedProperty deadZone;
    private SerializedProperty axisOptions;
    private SerializedProperty snapX;
    private SerializedProperty snapY;
    protected SerializedProperty background;
    private SerializedProperty handle;

    protected Vector2 center = new Vector2(0.5f, 0.5f);

    protected virtual void OnEnable()
    {
        handleRange = serializedObject.FindProperty("handleRange");
        deadZone = serializedObject.FindProperty("deadZone");
        axisOptions = serializedObject.FindProperty("axisOptions");
        snapX = serializedObject.FindProperty("snapX");
        snapY = serializedObject.FindProperty("snapY");
        background = serializedObject.FindProperty("background");
        handle = serializedObject.FindProperty("handle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawValues();
        EditorGUILayout.Space();
        DrawComponents();

        serializedObject.ApplyModifiedProperties();

        if (handle != null && handle.objectReferenceValue != null)
        {
            RectTransform handleRect = (RectTransform)handle.objectReferenceValue;
            handleRect.anchorMax = center;
            handleRect.anchorMin = center;
            handleRect.pivot = center;
            handleRect.anchoredPosition = Vector2.zero;
        }
    }

    protected virtual void DrawValues()
    {
        EditorGUILayout.PropertyField(handleRange, new GUIContent("Handle Range"));
        EditorGUILayout.PropertyField(deadZone, new GUIContent("Dead Zone"));
        EditorGUILayout.PropertyField(axisOptions, new GUIContent("Axis Options"));
        EditorGUILayout.PropertyField(snapX, new GUIContent("Snap X"));
        EditorGUILayout.PropertyField(snapY, new GUIContent("Snap Y"));
    }

    protected virtual void DrawComponents()
    {
        EditorGUILayout.ObjectField(background, new GUIContent("Background"));
        EditorGUILayout.ObjectField(handle, new GUIContent("Handle"));
    }
}
