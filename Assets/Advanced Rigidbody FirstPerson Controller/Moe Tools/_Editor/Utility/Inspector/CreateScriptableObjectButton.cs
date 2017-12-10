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
    public class CreateScriptableObjectButton<TScriptableObject> where TScriptableObject : ScriptableObject
    {
        public string buttonName;

        public string fileName;
        public virtual string FullFileName { get { return fileName + ".asset"; } }

        public string folderPath;
        public virtual string FullFolderPath { get { return "Assets/" + folderPath; } }

        public CreateScriptableObjectButton(string fileName, string folderPath)
        {
            buttonName = "Create " + fileName;

            this.fileName = fileName;
            this.folderPath = folderPath;
        }

        public virtual void Draw()
        {
            if (GUILayout.Button(buttonName))
            {
                if (!AssetDatabase.IsValidFolder(FullFolderPath))
                    AssetDatabase.CreateFolder("Assets/", folderPath);

                MoeTools.Editor.CreateAsset(typeof(TScriptableObject), FullFolderPath + '/' + FullFileName);
            }
        }
    }
}
#endif