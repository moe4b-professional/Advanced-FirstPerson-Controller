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

using System.Reflection;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Reflection
        {
            public static List<string> ParsePath(string path)
            {
                return String.SeperateViaChar(path, '.');
            }

            static BindingFlags FieldFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            public static object GetField(object source, string path)
            {
                List<string> names = ParsePath(path);

                object field = source;
                for (int i = 0; i < names.Count; i++)
                {
                    field = field.GetType().GetField(names[i], FieldFlags).GetValue(field);
                }

                return field;
            }

            static BindingFlags MethodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance;
            public static void InvokeMethod(object source, string path, object[] arguments)
            {
                List<string> names = ParsePath(path);

                object field = source;
                for (int i = 0; i < names.Count; i++)
                {
                    if (i == names.Count - 1)
                    {
                        field.GetType().GetMethod(names[i], MethodFlags).Invoke(source, arguments);
                    }
                    else
                    {
                        field.GetType().GetField(names[i], FieldFlags).GetValue(field);
                    }
                }
            }
        }
    }
}