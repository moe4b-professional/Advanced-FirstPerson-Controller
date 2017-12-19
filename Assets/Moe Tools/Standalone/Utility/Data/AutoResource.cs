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
    public class AutoResource<T> where T : Object
    {
        string path;
        T value;
        public T Value
        {
            get
            {
                if (value == null)
                    GetResource();

                return value;
            }
        }

        public T GetResource()
        {
            value = Resources.Load<T>(path);
            return value;
        }

        public AutoResource(string path)
        {
            this.path = path;
        }
    }
}