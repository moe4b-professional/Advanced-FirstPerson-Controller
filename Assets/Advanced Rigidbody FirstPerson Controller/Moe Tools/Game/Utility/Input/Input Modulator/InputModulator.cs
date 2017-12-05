using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Moe.Tools;

namespace Moe.Tools
{
    public interface IInputModulator
    {

    }
    public abstract class InputModulator : MonoBehaviour
    {
        public abstract Type ModuleType
        {
            get;
        }

        public Type[] Modules;

#if UNITY_EDITOR
        public string Path
        {
            get
            {
                return AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
            }
        }
        public string DirectoryPath
        {
            get
            {
                return Path.Remove(Path.LastIndexOf('/'));
            }
        }
#endif

        [SerializeField]
        protected DeployablePlatform[] platforms;
        public DeployablePlatform[] Platforms { get { return platforms; } }

        [Serializable]
        public class DeployablePlatform
        {
            [SerializeField]
            string name;
            public string Name { get { return name; } }

            [SerializeField]
            InputModule module;
            public InputModule Module { get { return module; } }

            [SerializeField]
            RuntimePlatform[] supportedPlatforms;

            public bool IsCurrentPlatform
            {
                get
                {
                    return supportedPlatforms.Contains(Application.platform);
                }
            }

            public DeployablePlatform(string name, RuntimePlatform[] supportedPlatforms)
            {
                this.name = name;
                this.supportedPlatforms = supportedPlatforms;
            }
        }

        public InputModulator()
        {
            platforms = new DeployablePlatform[]
            {
            new DeployablePlatform("PC", new RuntimePlatform[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer, RuntimePlatform.OSXEditor, RuntimePlatform.OSXPlayer, RuntimePlatform.LinuxEditor, RuntimePlatform.LinuxPlayer }),
            new DeployablePlatform("Mobile", new RuntimePlatform[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer, RuntimePlatform.TizenPlayer }),
            new DeployablePlatform("XBOX", new RuntimePlatform[] { RuntimePlatform.XboxOne }),
            new DeployablePlatform("PS4", new RuntimePlatform[] { RuntimePlatform.PS4 })
            };
        }
    }

    public interface IInputModulator<T>
        where T : InputModule
    {

    }
    public class InputModulator<T> : InputModulator where T : InputModule
    {
        public override Type ModuleType
        {
            get
            {
                return typeof(T);
            }
        }

        public virtual T GetCurrentModule()
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i].IsCurrentPlatform)
                {
                    platforms[i].Module.Init();

                    return (T)platforms[i].Module;
                }
            }

            return null;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InputModulator), true, isFallback = true)]
    public class InputModulatorInspector : Editor
    {
        InputModulator modulator;

        InspectorCustomGUI gui;

        DualInspectorList platformsList;

        void OnEnable()
        {
            modulator = (InputModulator)target;

            gui = new InspectorCustomGUI(serializedObject, true);
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

            name.stringValue = EditorGUI.TextField(GUIArea.ProgressLine(ref rect), "Name", name.stringValue);

            module.objectReferenceValue = EditorGUI.ObjectField(GUIArea.ProgressLine(ref rect), platformsList.main.serializedProperty.displayName, module.objectReferenceValue, modulator.ModuleType, false);
        }

        void InitPlatformsPreview(InspectorList list)
        {
            list.drawElementCallback = DrawPlatformsPreviewElement;
        }
        void DrawPlatformsPreviewElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty platform = platformsList.preview.GetArrayElement(index);

            EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), platform, new GUIContent("Platform " + (index + 1)));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            gui.Draw();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            CreateModulesUI();
        }

        public void DrawPlatforms(SerializedProperty property)
        {
            platformsList.Draw();
        }

        void DrawModuleField(SerializedProperty property, string label)
        {

        }

        void CreateModulesUI()
        {
            if (modulator.Modules == null)
                return;

            for (int i = 0; i < modulator.Modules.Length; i++)
            {
                if (GUILayout.Button("Create " + modulator.Modules[i].Name))
                {
                    if (!AssetDatabase.IsValidFolder(modulator.DirectoryPath + "/Modules"))
                        AssetDatabase.CreateFolder(modulator.DirectoryPath, "Modules");

                    CreateAsset(modulator.Modules[i], modulator.DirectoryPath + "/Modules/" + modulator.Modules[i].Name + ".asset");
                }
            }
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
#endif
}