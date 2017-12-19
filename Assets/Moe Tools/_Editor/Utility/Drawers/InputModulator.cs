#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEditorInternal;

using Moe.Tools;

namespace Moe.Tools
{
    public abstract partial class InputModulator
    {
        [CustomEditor(typeof(InputModulator), true, isFallback = true)]
        public class Inspector : InspectorBaseCustomDrawer<InputModulator>
        {
            DualInspectorList platformsList;

            protected override void OnEnable()
            {
                base.OnEnable();

                gui.Overrides.Add("platforms", DrawPlatforms);

                platformsList = new DualInspectorList(serializedObject.FindProperty("platforms"));

                platformsList.GetPreviewProperty = delegate (SerializedProperty prop)
                {
                    return prop.FindPropertyRelative("supportedPlatforms");
                };
                platformsList.InitPreview = InitPlatformsPreview;

                platformsList.main.elementHeight = 40f;
                platformsList.main.drawElementCallback = DrawPlatformsElement;
            }

            void DrawPlatformsElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                SerializedProperty name = platformsList.main.GetPropertyOfIndex(index, "name");
                SerializedProperty module = platformsList.main.GetPropertyOfIndex(index, "module");

                GUIArea.ProgressLayout(ref rect);

                name.stringValue = EditorGUI.TextField(GUIArea.ProgressLine(ref rect), "Name", name.stringValue);

                module.objectReferenceValue = EditorGUI.ObjectField(GUIArea.ProgressLine(ref rect), platformsList.main.serializedProperty.displayName, module.objectReferenceValue, target.ModuleType, false);
            }

            void InitPlatformsPreview(InspectorList list)
            {
                list.drawElementCallback = DrawPlatformsPreviewElement;
            }
            void DrawPlatformsPreviewElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                SerializedProperty platform = platformsList.preview.GetArrayElement(index);

                GUIArea.Progress(ref rect, GUIArea.LayoutOffset / 2);

                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), platform, new GUIContent("Platform " + (index + 1)));
            }

            public void DrawPlatforms(SerializedProperty property)
            {
                platformsList.Draw();
            }

            public static ScriptableObject CreateAsset(Type type, string path)
            {
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                ScriptableObject asset = CreateInstance(type);

                AssetDatabase.CreateAsset(asset, path);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Selection.activeObject = asset;

                return asset;
            }
        }
    }
}
#endif