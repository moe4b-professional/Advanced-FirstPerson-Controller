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

            public static string ToSystemPath(string relativePath)
            {
                return Path.GetFullPath(relativePath);
            }

            public static string ToUnityPath(string absolutepath)
            {
                if (string.IsNullOrEmpty(absolutepath))
                    return absolutepath;

                string newPath = absolutepath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Replace(ProjectPath, "");

                if (newPath.Length > 0 && newPath[0] == Path.DirectorySeparatorChar)
                    newPath = newPath.Remove(0, 1);

                return newPath;
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