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
using URandom = UnityEngine.Random;

using UScene = UnityEngine.SceneManagement.Scene;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Scene
        {
            public static List<T> GetAllComponents<T>(UScene scene, bool allowAtEditTime = false)
            {
                if (!Application.isPlaying && !allowAtEditTime)
                    throw new ArgumentException("Allow At EditTime arguments is false, please note that changes to components made at runtime will most likely be non-revertable, set the argument as true if you are sure of what you are doing and make a backup of you scene first !");

                List<T> list = new List<T>();

                if (scene.isLoaded)
                {
                    var roots = scene.GetRootGameObjects();

                    for (int y = 0; y < roots.Length; y++)
                    {
                        list.AddRange(roots[y].GetNestedComponents<T>());
                    }
                }
                else
                {
                    Debug.LogError(MoeTools.String.Enclose(scene.name) + " must be loaded to be able to get its components");
                }

                return list;
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static List<T> GetAllComponents<T>(this UScene scene, bool allowAtEditTime = false)
        {
            return MoeTools.Scene.GetAllComponents<T>(scene, allowAtEditTime);
        }
    }
}