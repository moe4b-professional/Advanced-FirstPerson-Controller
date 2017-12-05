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
    [Serializable]
    public abstract class ValueRange<T> : IRange<T>
    {
        [SerializeField]
        T min;
        public virtual T Min { get { return min; } set { min = value; } }

        [SerializeField]
        T max;
        public virtual T Max { get { return max; } set { max = value; } }

        public ValueRange(T min, T max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [Serializable]
    public class IntValueRange : ValueRange<int>, IIntRange
    {
        public IntValueRange(int min, int max) : base(min, max)
        {

        }
    }

    [Serializable]
    public class FloatValueRange : ValueRange<float>, IFloatRange
    {
        public FloatValueRange(float min, float max) : base(min, max)
        {

        }
    }
}