#if UNITY_EDITOR
using System;
using System.IO;
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
    public static class GUIArea
    {
        public const float LayoutOffset = 2;
        public static Rect ProgressLine(ref Rect rect)
        {
            return ProgressLayout(ref rect, EditorGUIUtility.singleLineHeight);
        }

        public static Rect ProgressLayout(ref Rect rect)
        {
            return ProgressLayout(ref rect, 0);
        }
        public static Rect ProgressLayout(ref Rect rect, float height)
        {
            return Progress(ref rect, height, LayoutOffset);
        }

        public static Rect Progress(ref Rect rect, float height)
        {
            return Progress(ref rect, height, 0f);
        }
        public static Rect Progress(ref Rect rect, float height, float offset)
        {
            Rect newRect = new Rect(rect.x, rect.y, rect.width, height);

            rect.y += height + offset;

            return newRect;
        }
    }
}
#endif