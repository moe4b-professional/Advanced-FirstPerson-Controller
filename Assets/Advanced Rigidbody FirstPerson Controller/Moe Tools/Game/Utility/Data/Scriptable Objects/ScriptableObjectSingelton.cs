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
	public class ScriptableObjectSingelton<TObject> : ScriptableObject
        where TObject : ScriptableObject
	{
        protected static SingeltonResource<TObject> Resource = new SingeltonResource<TObject>();

        public static TObject Current { get { return Resource.Current; } }
        public static bool AssetValid { get { return Resource.AssetValid; } }

        protected static TObject GetCurrent()
        {
            return Resources.LoadAll<TObject>("").First();
        }
	}

    public class SingeltonResource<TObject>
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

        protected TObject GetCurrent()
        {
            TObject[] objects = Resources.LoadAll<TObject>("");

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name.ToLower().Contains("override"))
                    return objects[i];
            }

            return objects.First();
        }
    }
}