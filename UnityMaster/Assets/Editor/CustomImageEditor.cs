using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CustomImage), true)]
[CanEditMultipleObjects]
public class CustomImageEditor : ImageEditor
{
    SerializedProperty _offset;

    SerializedProperty _isShowMesh;
    protected override void OnEnable()
    {
        base.OnEnable();

        _offset = serializedObject.FindProperty("Offset");

        _isShowMesh = serializedObject.FindProperty("IsShowMesh");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_offset);
        EditorGUILayout.PropertyField(_isShowMesh);

        var customImage = (CustomImage)(target);
        var content = EditorGUIUtility.TrTextContent("Create Polygon2D Collider Points", "", (Texture)null);


        if (GUILayout.Button(content, EditorStyles.miniButton))
        {
            customImage.CreatePolygon2DColliderPoints();
        }



        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
