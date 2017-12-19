#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public partial class KeyCodeList
    {
        [CustomPropertyDrawer(typeof(KeyCodeList))]
        public class Drawer : PropertyDrawer
        {
            public SerializedProperty GetList(SerializedProperty property)
            {
                return property.FindPropertyRelative("list");
            }
            InspectorList list;

            protected virtual void InitList(SerializedProperty property, GUIContent label)
            {
                if (list == null)
                {
                    list = new InspectorList(GetList(property));
                    list.label = label.text;
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                InitList(property, label);

                return EditorGUIUtility.singleLineHeight + (list.serializedProperty.isExpanded ? EditorGUI.GetPropertyHeight(list.serializedProperty) + EditorGUIUtility.singleLineHeight : 0);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                InitList(property, label);

                list.Draw(position);
            }
        }
    }
}
#endif