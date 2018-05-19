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
    public class ResourceSingleton<TObject>
        where TObject : Object
    {
        TObject current;
        public TObject Current
        {
            get
            {
                if (current == null || Application.isPlaying == false)
                    current = GetCurrent();

                return current;
            }
        }
        public bool AssetValid { get { return Current != null; } }

        public static TObject GetCurrent()
        {
            TObject[] objects = Resources.LoadAll<TObject>("");

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name.ToLower().Contains("override"))
                    return objects[i];
            }

            if (objects.Length > 0)
                return objects.First();
            else
                return null;
        }
    }

    public class ScriptableObjectResourceSingleton<TObject> : ScriptableObject
        where TObject : ScriptableObject
	{
        protected static ResourceSingleton<TObject> Resource = new ResourceSingleton<TObject>();

        public static TObject Instance { get { return Resource.Current; } }
        public static bool InstanceAvailable { get { return Resource.AssetValid; } }
	}
}