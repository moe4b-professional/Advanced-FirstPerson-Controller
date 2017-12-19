#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public static partial class MoeTools
    {
        public static class Inspector
        {
            public static void Space()
            {
                Space(1);
            }
            public static void Space(int spaces)
            {
                for (int i = 0; i < spaces; i++)
                {
                    EditorGUILayout.Space();
                }
            }

            public static void StartVertical(string label)
            {
                EditorGUILayout.BeginVertical(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);

                EditorGUILayout.Space();
                {
                    EditorGUILayout.LabelField(label, new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 20 });
                }
                EditorGUILayout.Space();
            }
            public static void EndVertical()
            {
                Space();

                EditorGUILayout.EndVertical();
            }

            public static List<SerializedProperty> GetChildern(SerializedProperty property)
            {
                List<SerializedProperty> properties = new List<SerializedProperty>();

                if (property.isArray)
                    return properties;

                SerializedProperty iterator = property.Copy();

                SerializedProperty endProperty = iterator.GetEndProperty();

                iterator.NextVisible(true);

                while (true)
                {
                    if (SerializedProperty.EqualContents(iterator, endProperty))
                        break;

                    properties.Add(iterator.Copy());

                    iterator.NextVisible(false);
                }

                return properties;
            }

            public static List<SerializedProperty> GetChildern(SerializedObject serializedObject)
            {
                List<SerializedProperty> childern = new List<SerializedProperty>();

                SerializedProperty property = serializedObject.GetIterator();
                property.Next(true);

                while (property.NextVisible(false))
                    childern.Add(property.Copy());

                return childern;
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static List<SerializedProperty> GetChildern(this SerializedProperty property)
        {
            return MoeTools.Inspector.GetChildern(property);
        }

        public static List<SerializedProperty> GetChildern(this SerializedObject serializedObject)
        {
            return MoeTools.Inspector.GetChildern(serializedObject);
        }
    }
}
#endif