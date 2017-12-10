#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.IO;

namespace Moe.Tools
{
    public static partial class MoeTools
    {
        public static class Editor
        {
            public static string ProjectPath
            {
                get
                {
                    return new DirectoryInfo(Application.dataPath).Parent.FullName;
                }
            }

            public static string GetScriptPath(string scriptName)
            {
                string[] directories = GetDirectories(Application.dataPath);

                for (int i = 0; i < directories.Length; i++)
                {
                    DirectoryInfo dInfo = new DirectoryInfo(directories[i]);

                    FileInfo[] filesInfo = dInfo.GetFiles();
                    for (int x = 0; x < filesInfo.Length; x++)
                    {
                        if (filesInfo[x].Name.Contains(scriptName))
                            return filesInfo[x].Directory.FullName;
                    }
                }

                return string.Empty;
            }

            public static string[] GetDirectories(string path)
            {
                if (!Directory.Exists(path))
                    throw new ArgumentException();

                List<string> directories = new List<string>();

                directories.Add(path);

                DirectoryInfo[] directoriesInfo = new DirectoryInfo(path).GetDirectories();
                for (int x = 0; x < directoriesInfo.Length; x++)
                {
                    directories.Add(directoriesInfo[x].FullName);

                    string[] subDirectories = GetDirectories(directoriesInfo[x].FullName);

                    for (int y = 0; y < subDirectories.Length; y++)
                    {
                        directories.Add(subDirectories[y]);
                    }
                }

                return directories.ToArray();
            }

            public static string ToSystemPath(string unityAssetPath)
            {
                return Path.Combine(ProjectPath, unityAssetPath.Replace('/', '\\'));
            }

            public static string ToUnityPath(string systemPath)
            {
                string unityPath = systemPath.Replace(ProjectPath, "");

                if (unityPath == systemPath)
                    throw new ArgumentException(systemPath + " Not A Unity Path");
                else
                {
                    if (unityPath[0] == '/')
                        unityPath = systemPath.Replace('\\', '/');
                    return unityPath.Remove(0, 1);
                }
            }

            public static ScriptableObject CreateAsset(Type type, string path)
            {
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                ScriptableObject asset = ScriptableObject.CreateInstance(type);

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