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

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Random
        {
            public static bool Bool
            {
                get
                {
                    return URandom.Range(0, 100) > 50 ? true : false;
                }
            }

            public static int GetInt(int min, int max)
            {
                return URandom.Range(min, max);
            }
            public static float GetFloat(float min, float max)
            {
                return URandom.Range(min, max);
            }
        }
    }
}