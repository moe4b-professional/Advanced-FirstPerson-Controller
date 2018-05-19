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
    public class CreateScriptableObjectButton<TScriptableObject>
        where TScriptableObject : ScriptableObject
    {
        public string Label { get; set; }

        public string FileName { get; set; }
        public virtual string GetFullFileName()
        {
            return FileName + ".asset";
        }

        public string FolderPath { get; set; }
        public virtual string GetFullFolderPath()
        {
            return FolderPath + ".asset";
        }

        public virtual string GetPath()
        {
            return Path.Combine(GetFullFolderPath(), GetFullFileName());
        }

        public virtual void Draw()
        {
            if (GUILayout.Button(Label))
                Create();
        }
        protected virtual void Create()
        {
            if (!AssetDatabase.IsValidFolder(GetFullFolderPath()))
                AssetDatabase.CreateFolder("Assets/", FolderPath);

            MoeTools.Editor.CreateAsset(typeof(TScriptableObject), GetPath());
        }

        public CreateScriptableObjectButton(string fileName, string folderPath) : this(GetLabel(fileName), fileName, folderPath)
        {
            
        }
        public CreateScriptableObjectButton(string label, string fileName, string folderPath)
        {
            this.Label = label;
            this.FileName = fileName;
            this.FolderPath = folderPath;
        }

        public static string GetLabel(string fileName)
        {
            return "Create " + fileName;
        }
    }
}
#endif