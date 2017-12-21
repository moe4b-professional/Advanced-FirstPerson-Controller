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

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public class AdvancedAssetPostprocessorToolkit : AssetPostprocessor
    {
        public class PostPorcessor : AssetPostprocessor
        {
            public virtual Settings BaseSettings { get { throw new NotImplementedException(); } }
            public virtual Window BaseWindow { get { throw new NotImplementedException(); } }

            public virtual bool Active
            {
                get
                {
                    if (BaseWindow && BaseWindow.Target && assetPath == AssetDatabase.GetAssetPath(BaseWindow.Target))
                        return true;

                    if (!BaseSettings.IsValidPath(assetPath))
                        return false;

                    if (BaseSettings.Activation == ActivationMode.FirstImport)
                        if (File.Exists(AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath)))
                            return false;

                    return true;
                }
            }
        }

        public abstract partial class Window : EditorWindow
        {
            public abstract EditorWindow BaseInstance { get; protected set; }
            
            public abstract Settings BaseSettings { get; }

            public virtual float MaxHeight { get { return 65f; } }
            public virtual void InitSize(Vector2 width, float height)
            {
                minSize = new Vector2(width.x, height);
                maxSize = new Vector2(width.y, height);
            }

            public abstract Type TargetType { get; }
            public Object Target { get; protected set; }
            protected virtual void OnGUI()
            {
                DrawSettings();

                DrawTarget();
            }

            protected virtual void DrawSettings()
            {
                if (BaseSettings == null)
                    GUI.enabled = false;

                if (GUILayout.Button("Settings"))
                {
                    if (BaseSettings)
                    {
                        Selection.activeObject = BaseSettings;
                    }
                    else
                        EditorApplication.Beep();
                }

                GUI.enabled = true;
            }
            protected virtual void DrawTarget()
            {
                Target = EditorGUILayout.ObjectField("Object", Target, TargetType, false);

                if (Target == null)
                    GUI.enabled = false;

                if (GUILayout.Button("Import") && Target)
                    Import();

                GUI.enabled = true;
            }
            protected virtual void Import()
            {
                if (Target == null)
                    throw new NullReferenceException();

                if (BaseInstance == null)
                    BaseInstance = GetWindow<EditorWindow>();

                string targetPath = AssetDatabase.GetAssetPath(Target);

                AssetDatabase.ImportAsset(targetPath, ImportAssetOptions.ForceUpdate);
            }
        }
        public abstract partial class Window<TWindow> : Window
            where TWindow : Window<TWindow>
        {
            public static TWindow Instance { get; protected set; }
            public override EditorWindow BaseInstance
            {
                get
                {
                    return Instance;
                }
                protected set
                {
                    Instance = value as TWindow;
                }
            }

            public static void Init()
            {
                Instance = GetWindow<TWindow>();

                Instance.Show();

                Instance.InitSize(new Vector2(250, 500), Instance.MaxHeight);
            }
        }
        
        public class Settings : ScriptableObject
        {
            [SerializeField]
            protected ActivationMode activation = ActivationMode.FirstImport;
            public ActivationMode Activation { get { return activation; } }

            [SerializeField]
            protected string[] pathsConstraints;
            public string[] PathsConstraints { get { return pathsConstraints; } }

            public bool IsValidPath(string path)
            {
                for (int i = 0; i < pathsConstraints.Length; i++)
                {
                    if (path.Contains(pathsConstraints[i]))
                        return true;
                }

                return false;
            }

            [CustomEditor(typeof(Settings), true, isFallback = true)]
            public class Inspector : InspectorBaseCustomDrawer<Settings>
            {
                InspectorList pathsConstraints;

                protected override void OnEnable()
                {
                    base.OnEnable();

                    pathsConstraints = new InspectorList(serializedObject.FindProperty("pathsConstraints"));

                    gui.Overrides.Add(pathsConstraints.serializedProperty.name, DrawFoldersConstraints);
                }

                protected virtual void DrawFoldersConstraints()
                {
                    pathsConstraints.Draw();
                }
            }
        }
        public class Settings<TSettings> : Settings
            where TSettings : Settings<TSettings>
        {
            protected static TSettings _instance;
            public static TSettings Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        string GUIID = AssetDatabase.FindAssets("t:" + typeof(TSettings).Name, null).FirstOrDefault();

                        _instance = AssetDatabase.LoadAssetAtPath<TSettings>(AssetDatabase.GUIDToAssetPath(GUIID));
                    }

                    return _instance;
                }
            }
        }

        public enum ActivationMode
        {
            Always, FirstImport
        }
    }
}
#endif