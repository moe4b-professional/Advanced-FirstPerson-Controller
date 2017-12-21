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
	public static partial class MoeTools
    {
        public static class Vector
        {
            public static Vector3 Divide(Vector3 a, Vector3 b)
            {
                a.x /= b.x;
                a.y /= b.y;
                a.z /= b.z;

                return a;
            }
        }
    }
}