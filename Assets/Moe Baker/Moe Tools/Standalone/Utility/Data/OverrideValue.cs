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
    [Serializable]
	public abstract partial class OverrideValue
    {
        [SerializeField]
        protected bool enabled;
        public bool Enabled { get { return enabled; } }

        public OverrideValue() : this(false)
        {

        }
        public OverrideValue(bool enabled)
        {
            this.enabled = enabled;
        }
    }

    [Serializable]
	public abstract class OverrideValue<TData> : OverrideValue
    {
        [SerializeField]
        protected TData value;
        public TData Value { get { return value; } }

        public virtual TData GetEnabledOrDefault(TData defaultValue)
        {
            if (enabled)
                return value;

            return defaultValue;
        }

        public OverrideValue() : this(false, default(TData))
        {

        }
        public OverrideValue(bool enabled) : this(enabled, default(TData))
        {

        }
        public OverrideValue(TData value) : this(false, value)
        {

        }
        public OverrideValue(bool enabled, TData value) : base(enabled)
        {
            this.value = value;
        }
    }

    #region Sample Values
    [Serializable]
    public class IntOverrideValue : OverrideValue<int>
    {
        public IntOverrideValue(int value) : base(value) { }
        public IntOverrideValue(bool enabled, int value) : base(enabled, value) { }
    }

    [Serializable]
    public class FloatOverrideValue : OverrideValue<float>
    {
        public FloatOverrideValue(float value) : base(value) { }
        public FloatOverrideValue(bool enabled, float value) : base(enabled, value) { }
    }

    [Serializable]
    public class BoolOverrideValue : OverrideValue<bool>
    {
        public BoolOverrideValue(bool value) : base(value) { }
        public BoolOverrideValue(bool enabled, bool value) : base(enabled, value) { }
    }

    [Serializable]
    public class StringOverrideValue : OverrideValue<string>
    {
        public StringOverrideValue(string value) : base(value) { }
        public StringOverrideValue(bool enabled, string value) : base(enabled, value) { }
    }

    [Serializable]
    public class PlatformOverrideValue : OverrideValue<GameTargetPlatform>
    {

    }
    #endregion
}