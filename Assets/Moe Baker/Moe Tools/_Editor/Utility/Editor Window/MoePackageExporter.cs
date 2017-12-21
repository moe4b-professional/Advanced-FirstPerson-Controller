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
	public class MoePackageExporter : EditorWindow
	{
		public static MoePackageExporter Instance { get; protected set; }
        public static void GetInstance()
        {
            Instance = GetWindow<MoePackageExporter>();
        }

        public Object Folder { get; protected set; }
        public string FolderPath { get { return AssetDatabase.GetAssetPath(Folder); } }
        public bool IsSelecting { get; protected set; }

        public string FileName { get; protected set; }
        public const string FileExtension = ".unitypackage";

        public string DestinationPath { get; protected set; }
        public bool DestinationValid { get { return Directory.Exists(DestinationPath); } }

        public ExportOptionsData ExportOptions { get; protected set; }
        public class ExportOptionsData
        {
            public bool IncludeDependencies { get; protected set; }
            public bool IncludeLibraryAssets { get; protected set; }
            public bool Interactive { get; protected set; }
            public bool Recurse { get; protected set; }

            public ExportPackageOptions Options { get; protected set; }

            public virtual void Draw()
            {
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        IncludeDependencies = EditorGUILayout.ToggleLeft("Include Dependencies", IncludeDependencies);
                        IncludeLibraryAssets = EditorGUILayout.ToggleLeft("Include Library Settings", IncludeLibraryAssets);
                        Interactive = EditorGUILayout.ToggleLeft("Interactive", Interactive);
                        Recurse = EditorGUILayout.ToggleLeft("Recurse", Recurse);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (EditorGUI.EndChangeCheck())
                    UpdateOptions();
            }

            public virtual void UpdateOptions()
            {
                if (!IncludeDependencies && !IncludeLibraryAssets && !Interactive && !Recurse)
                    Options = ExportPackageOptions.Default;
                else
                {
                    if (IncludeDependencies) Options |= ExportPackageOptions.IncludeDependencies;
                    if (IncludeLibraryAssets) Options |= ExportPackageOptions.IncludeLibraryAssets;
                    if (Interactive) Options |= ExportPackageOptions.Interactive;
                    if (Recurse) Options |= ExportPackageOptions.Recurse;
                }
            }

            public ExportOptionsData()
            {
                Interactive = true;
                Recurse = true;
            }
        }

        [MenuItem(MoeTools.Constants.Paths.Tools + "Package Exporter")]
        protected static void Init()
        {
            GetInstance();

            Instance.Show();
        }

        public const float MinWidth = 500f;
        public const float MaxWidth = 800;
        protected virtual void SetSize(float height)
        {
            minSize = new Vector2(MinWidth, height);
            maxSize = new Vector2(MaxWidth, height);
        }

        protected virtual void OnEnable()
        {
            Folder = null;
            IsSelecting = false;

            FileName = "";

            DestinationPath = MoeTools.Editor.ProjectPath;

            ExportOptions = new ExportOptionsData();

            SetSize(100);
            GetInstance();
        }

        protected virtual void OnGUI()
        {
            DrawFolder();

            DrawFileName();

            DrawDestination();

            ExportOptions.Draw();

            DrawExport();
        }

        protected virtual void DrawFolder()
        {
            EditorGUIUtility.labelWidth = 80f;

            EditorGUILayout.BeginHorizontal();
            {
                if (Folder && !IsFolder(Folder))
                {
                    EditorApplication.Beep();

                    Folder = null;
                }

                EditorGUI.BeginChangeCheck();
                {
                    Folder = EditorGUILayout.ObjectField("Folder", Folder, typeof(Object), false);
                }
                if (EditorGUI.EndChangeCheck())
                    OnFolderChanged();


                if (IsSelecting)
                {
                    if (GUILayout.Button("Cancel Selection"))
                        SelectionEnd();
                }
                else
                {
                    if (GUILayout.Button("Select Folder"))
                        SelectionStart();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        protected virtual void SelectionStart()
        {
            Selection.selectionChanged += OnSelectionChanged;

            IsSelecting = true;
        }
        protected virtual void SelectionEnd()
        {
            UnityEditor.Selection.selectionChanged -= OnSelectionChanged;
            IsSelecting = false;
        }
        protected virtual void OnSelectionChanged()
        {
            Folder = Selection.activeObject;

            OnFolderChanged();

            SelectionEnd();

            Repaint();
        }
        protected virtual void OnFolderChanged()
        {
            if (!IsFolder(Folder))
            {
                Folder = null;

                EditorApplication.Beep();
            }

            FileName = Folder ? Folder.name : "";

            Repaint();
        }
        protected virtual bool IsFolder(Object folder)
        {
            var path = AssetDatabase.GetAssetPath(folder);

            return AssetDatabase.IsValidFolder(path);
        }

        protected virtual void DrawFileName()
        {
            FileName = EditorGUILayout.TextField("File Name", FileName);
        }

        protected virtual void DrawDestination()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                {
                    DestinationPath = EditorGUILayout.TextField("Destination", DestinationPath);

                    if (GUILayout.Button("Select Destination"))
                    {
                        DestinationPath = EditorUtility.OpenFolderPanel("Select Destination", "", "");
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {

                }
            }
            EditorGUILayout.EndHorizontal();

            if (!DestinationValid)
                EditorGUILayout.HelpBox("No Valid Destination Detected, Please Select A Folder Where Your Package Will Be Saved", MessageType.Warning);
        }

        protected virtual void DrawExport()
        {
            if (Folder == null || !DestinationValid)
                GUI.enabled = false;
            {
                if (GUILayout.Button("Package"))
                    ProcessPackaging();
            }
            GUI.enabled = true;
        }
        protected virtual void ProcessPackaging()
        {
            ExportOptions.UpdateOptions();

            AssetDatabase.ExportPackage(FolderPath, FileName + FileExtension, ExportOptions.Options);
        }
    }
}
#endif