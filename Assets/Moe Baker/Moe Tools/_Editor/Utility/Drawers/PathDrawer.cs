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
    public class PathDrawer
    {
        public SerializedProperty Property { get; protected set; }
        public virtual string Path
        {
            get
            {
                return Property.stringValue;
            }
            set
            {
                Property.stringValue = value;
            }
        }

        public TargetType Target { get; protected set; }
        public bool IsFile { get { return Target == TargetType.File; } }
        public bool IsFolder { get { return Target == TargetType.Folder; } }

        public string Title { get; set; }

        public const string LocalPathName = "LOCAL";

        public virtual void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 80f;

                if (Path == string.Empty)
                {
                    string tempPath = EditorGUILayout.TextField(Property.displayName, LocalPathName);

                    if (tempPath == LocalPathName)
                        Path = "";
                    else
                        Path = tempPath;
                }
                else
                    Path = EditorGUILayout.TextField(Property.displayName, Path);

                if (GUILayout.Button("Select Path"))
                    DrawSelect();
            }
            EditorGUILayout.EndHorizontal();
        }
        protected virtual void DrawSelect()
        {
            if (Target == TargetType.File)
                Path = EditorUtility.OpenFilePanel(Title, Path, "");
            else if (Target == TargetType.Folder)
                Path = EditorUtility.OpenFolderPanel(Title, Path, "");

            Path = MoeTools.Editor.ToUnityPath(Path);
        }

        public PathDrawer(SerializedProperty property) : this(property, TargetType.File)
        {

        }
        public PathDrawer(SerializedProperty property, TargetType target)
        {
            this.Property = property;
            this.Target = target;

            this.Title = "Select " + target.ToString();
        }

        public enum TargetType
        {
            File, Folder
        }
    }
}
#endif