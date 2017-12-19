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
    public interface IRange<T>
    {
        T Min { get; }
        T Max { get; }
    }

    public interface IIntRange : IRange<int>
    {

    }

    public interface IFloatRange : IRange<float>
    {

    }

    public static partial class IRangeExtensions
    {
        //Float
        public static float GetRandom(this IFloatRange range)
        {
            return MoeTools.Random.GetFloat(range.Min, range.Max);
        }
        public static float Clamp(this IFloatRange range, float value)
        {
            return Mathf.Clamp(value, range.Min, range.Max);
        }

        //Int
        public static int GetRandom(this IIntRange range)
        {
            return MoeTools.Random.GetInt(range.Min, range.Max);
        }
        public static int Clamp(this IIntRange range, int value)
        {
            return Mathf.Clamp(value, range.Min, range.Max);
        }
    }
}