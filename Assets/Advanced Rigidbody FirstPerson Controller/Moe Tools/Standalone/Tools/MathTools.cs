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
using SEnum = System.Enum;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Math
        {
            public static float DeltaAngle360(float current, float target)
            {
                float angle = Mathf.DeltaAngle(current, target);

                if (angle < 0f)
                    angle += 360f;

                return angle;
            }

            public static float ClampRewind(float value, float min, float max)
            {
                if (value > max)
                    value = min;
                else if (value < min)
                    value = max;

                return value;
            }
            public static int ClampRewind(int value, int min, int max)
            {
                if (value > max)
                    value = min;
                else if (value < min)
                    value = max;

                return value;
            }

            public static float Vector2Angle(Vector2 vector2)
            {
                return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
            }
        }
    }
}