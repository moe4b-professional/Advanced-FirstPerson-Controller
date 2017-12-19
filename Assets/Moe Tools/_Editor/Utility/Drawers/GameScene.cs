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
    public partial class GameScene
    {
        [CustomPropertyDrawer(typeof(GameScene))]
        public class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                Draw(ref position, property);
            }

            public static void Draw(ref Rect rect, SerializedProperty property)
            {
                SerializedProperty name = property.FindPropertyRelative("name");
                SerializedProperty asset = property.FindPropertyRelative("asset");

                asset.objectReferenceValue = EditorGUI.ObjectField(GUIArea.ProgressLine(ref rect), property.displayName, asset.objectReferenceValue, typeof(SceneAsset), false);

                name.stringValue = asset.objectReferenceValue ? asset.objectReferenceValue.name : "";
            }
        }
    }
}
#endif