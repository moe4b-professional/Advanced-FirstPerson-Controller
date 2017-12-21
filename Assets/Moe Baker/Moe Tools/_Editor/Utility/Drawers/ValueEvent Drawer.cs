#if UNITY_EDITOR
using System;
using System.IO;
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
    public abstract partial class ValueEvent
    {
        [CustomPropertyDrawer(typeof(ValueEvent), true)]
        public class Drawer : PropertyDrawer
        {
            public SerializedProperty GetValueProperty(SerializedProperty parent)
            {
                return parent.FindPropertyRelative("value");
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(GetValueProperty(property), true);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref position), GetValueProperty(property), new GUIContent(property.displayName), true);
            }
        }
    }
}
#endif