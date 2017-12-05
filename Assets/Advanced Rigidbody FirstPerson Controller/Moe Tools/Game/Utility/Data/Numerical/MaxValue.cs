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
    public abstract class MaxValue<TData> : IRange<TData>
    {
        [SerializeField]
        protected TData value;
        public virtual TData Value
        {
            get
            {
                return value;
            }
            set
            {
                SetValue(value);
            }
        }
        public abstract TData Min { get; }

        [SerializeField]
        protected TData max;
        public virtual TData Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
            }
        }
        TData IRange<TData>.Max { get { return max; } }

        public virtual bool Full { get { return value.Equals(max); } }
        public virtual bool Empty { get { return value.Equals(Min); } }

        public virtual void Fill()
        {
            SetValue(max);
        }

        public virtual void SetValue(TData newValue)
        {
            newValue = Clamp(newValue);

            value = newValue;
        }
        public abstract TData Clamp(TData newValue);

        public MaxValue(TData value) : this(value, value)
        {

        }
        public MaxValue(TData value, TData max)
        {
            this.value = value;
            this.max = max;
        }
    }

    [Serializable]
    public class MaxIntValue : MaxValue<int>, IIntRange
    {
        public override int Min { get { return 0; } }

        public override int Clamp(int newValue)
        {
            return Mathf.Clamp(newValue, Min, Max);
        }

        public MaxIntValue(int value) : base(value)
        {

        }
        public MaxIntValue(int value, int max) : base(value, max)
        {

        }
    }

    [Serializable]
    public class MaxFloatValue : MaxValue<float>, IFloatRange
    {
        public override float Min { get { return 0f; } }

        public override float Clamp(float newValue)
        {
            return Mathf.Clamp(newValue, Min, Max);
        }

        public MaxFloatValue(float value) : base(value)
        {

        }
        public MaxFloatValue(float value, float max) : base(value, max)
        {

        }
    }
}