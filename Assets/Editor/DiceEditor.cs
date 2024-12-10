using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for the Dice class.
/// </summary>
[CustomEditor(typeof(Dice))]
public class DiceEditor : Editor
{
    private Dice dice;
    private SerializedProperty facesProp;
    private SerializedProperty typeProp;
    private SerializedProperty levelProp;
    private PreviewRenderUtility previewRenderUtility;
    private Mesh diceMesh;

    void OnEnable()
    {
        // 获取Dice类实例
        //dice = (Dice)target;

        // 获取序列化字段
        facesProp = serializedObject.FindProperty("faces");
        typeProp = serializedObject.FindProperty("Type");
        levelProp = serializedObject.FindProperty("Level");
    }

    public override void OnInspectorGUI()
    {
        // 更新数据
        serializedObject.Update();

        DrawDiceProperties();
        DrawFacesArray();

        // 更新字段
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDiceProperties()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Basic Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(typeProp);
        EditorGUILayout.PropertyField(levelProp);
        EditorGUILayout.EndVertical();
    }

    private void DrawFacesArray()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Face Configuration", EditorStyles.boldLabel);
        
        // 显示面的预览
        for (int i = 0; i < facesProp.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(facesProp.GetArrayElementAtIndex(i));
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                RemoveFace(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }

    private bool ValidateFaces()
    {
        if (facesProp.arraySize != 6)
        {
            EditorGUILayout.HelpBox("A dice must have exactly 6 faces.", MessageType.Error);
            return false;
        }
        return true;
    }

    private void RemoveFace(int index)
    {
        facesProp.DeleteArrayElementAtIndex(index);
    }
}
