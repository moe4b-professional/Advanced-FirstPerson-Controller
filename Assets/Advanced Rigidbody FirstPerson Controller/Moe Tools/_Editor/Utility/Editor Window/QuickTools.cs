#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Moe.Tools;

public class QuickTools : EditorWindow
{
    static QuickTools window;
    [MenuItem("Moe/Tools/Quick Tools")]
    public static void Init()
    {
        window = GetWindow<QuickTools>();
        window.Show();

        window.titleContent.text = "Quick Tools";
        //window.minSize = new Vector2(200, 260f);
        //window.maxSize = new Vector2(500, 260f);

        Selection.selectionChanged = delegate
        {
            if (Selection.activeGameObject && Selection.activeTransform)
            {
                newLocalPosition = Selection.activeTransform.localPosition;
                window.Repaint();
            }
        };
    }

    static Vector2 scrollPosition = Vector2.zero;
    void OnGUI()
    {
        EditorGUIUtility.labelWidth /= 2;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        {
            DrawReplace();
            DrawGroup();
            DrawEditLocalPosition();
        }
        EditorGUILayout.EndScrollView();
    }

    static GameObject replacment;
    void DrawReplace()
    {
        MoeTools.Inspector.StartVertical("Replace");

        replacment = (GameObject)EditorGUILayout.ObjectField("Replacment", replacment, typeof(GameObject), false);

        if (GUILayout.Button("Replace"))
        {
            if (Selection.gameObjects.Length == 0 || replacment == null)
                EditorApplication.Beep();
            else
                Replace(Selection.gameObjects, replacment);
        }

        MoeTools.Inspector.EndVertical();
    }
    public static void Replace(GameObject[] targets, GameObject replacment)
    {
        UnityEngine.Object[] objects = new UnityEngine.Object[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            GameObject replacmentI = (GameObject)PrefabUtility.InstantiatePrefab(replacment);

            objects[i] = replacmentI;

            replacmentI.transform.SetParent(targets[i].transform.parent);
            replacmentI.transform.SetSiblingIndex(targets[i].transform.GetSiblingIndex());

            replacmentI.transform.position = targets[i].transform.position;
            replacmentI.transform.rotation = targets[i].transform.rotation;

            DestroyImmediate(targets[i]);
        }

        Selection.objects = objects;
    }

    static string groupName = "group";
    void DrawGroup()
    {
        MoeTools.Inspector.StartVertical("Group");

        groupName = EditorGUILayout.TextField("Name", groupName);
        if (GUILayout.Button("Group"))
        {
            if (Selection.gameObjects.Length == 0)
                EditorApplication.Beep();
            else
                Group(Selection.gameObjects, groupName);
        }

        MoeTools.Inspector.EndVertical();
    }
    public static void Group(GameObject[] targets, string name)
    {
        GameObject group = new GameObject(name);

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].transform.SetParent(group.transform, false);
        }

        Selection.activeObject = group;
    }

    static Vector3 newLocalPosition;
    void DrawEditLocalPosition()
    {
        MoeTools.Inspector.StartVertical("Edit Local Position");

        newLocalPosition = EditorGUILayout.Vector3Field("Position", newLocalPosition);
        if (GUILayout.Button("Change Local Position"))
        {
            if (Selection.gameObjects.Length == 0)
                EditorApplication.Beep();
            else
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    EditTransformLocalPosition(Selection.gameObjects[i].transform, newLocalPosition);
                }
        }

        MoeTools.Inspector.EndVertical();
    }
    public static void EditTransformLocalPosition(Transform transform, Vector3 newPosition)
    {
        Vector3 childEdit = transform.localPosition - newPosition;

        transform.localPosition = newPosition;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition += childEdit;
        }
    }
}
#endif