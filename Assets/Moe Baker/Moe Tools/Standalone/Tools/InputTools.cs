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

using UInput = UnityEngine.Input;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Input
        {
            public static bool ForAny(IList<KeyCode> list, Func<KeyCode, bool> action)
            {
                return list.Any(action);
            }

            public static bool GetInput(IList<KeyCode> list)
            {
                return ForAny(list, delegate (KeyCode keyCode)
                {
                    return UInput.GetKey(keyCode);
                });
            }
            public static bool GetInputUp(IList<KeyCode> list)
            {
                return ForAny(list, delegate (KeyCode keyCode)
                {
                    return UInput.GetKeyUp(keyCode);
                });
            }
            public static bool GetInputDown(IList<KeyCode> list)
            {
                return ForAny(list, delegate (KeyCode keyCode)
                {
                    return UInput.GetKeyDown(keyCode);
                });
            }

            public static int GetInputAxis(IList<KeyCode> positive, IList<KeyCode> negative)
            {
                if (GetInput(positive))
                    return 1;
                else if (GetInput(negative))
                    return -1;

                return 0;
            }
            public static int GetInputAxis(KeyCode positive, KeyCode negative)
            {
                if (UInput.GetKey(positive))
                    return 1;
                else if (UInput.GetKey(negative))
                    return -1;

                return 0;
            }
            public static int GetInputAxis(bool positive, bool negative)
            {
                if (positive)
                    return 1;
                else if (negative)
                    return -1;

                return 0;
            }
        }
    }
}