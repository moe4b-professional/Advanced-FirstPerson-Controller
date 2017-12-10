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
        public static class Convertion
        {
            public static bool IntToBool(int value)
            {
                return value == 0 ? false : true;
            }
            public static int BoolToInt(bool value)
            {
                return value ? 1 : 0;
            }
        }
    }
}