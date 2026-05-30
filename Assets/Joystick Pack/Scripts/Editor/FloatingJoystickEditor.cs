using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FloatingJoystick))]
public class FloatingJoystickEditor : JoystickEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty backgroundProp = serializedObject.FindProperty("background");

        if (backgroundProp != null && backgroundProp.objectReferenceValue != null)
        {
            RectTransform backgroundRect = (RectTransform)backgroundProp.objectReferenceValue;

            backgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.anchoredPosition = Vector2.zero;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
