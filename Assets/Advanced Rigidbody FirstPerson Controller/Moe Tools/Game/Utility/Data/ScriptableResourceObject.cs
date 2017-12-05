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
	public abstract class ScriptableResourceObject<TResource> : ScriptableObject
        where TResource : ScriptableResourceObject<TResource>
    {
		public static string ResourcePath { get; protected set; }

        static TResource current;
        public static TResource Current
        {
            get
            {
                if (current == null)
                    current = Resources.Load<TResource>(ResourcePath);

                return current;
            }
        }

        public static bool AssetValid { get { return Current != null; } }

        public ScriptableResourceObject(string path)
        {
            ResourcePath = path;
        }
	}
}