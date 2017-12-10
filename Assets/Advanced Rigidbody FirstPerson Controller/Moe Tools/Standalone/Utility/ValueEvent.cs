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
    public abstract partial class ValueEvent
    {

    }
    [Serializable]
    public class ValueEvent<TValue> : ValueEvent
    {
        [SerializeField]
        TValue value;
        public TValue Value { get { return value; } set { SetValue(value); } }

        public delegate void ValueChangedSimple(TValue value);
        public event ValueChangedSimple OnValueChangedSimple;

        public delegate void ValueChangedComplex(TValue newValue, TValue oldValue);
        public event ValueChangedComplex OnValueChangedComplex;

        protected virtual void SetValue(TValue newValue)
        {
            if (OnValueChangedSimple != null)
                OnValueChangedSimple(newValue);

            if (OnValueChangedComplex != null)
                OnValueChangedComplex(newValue, value);

            value = newValue;
        }

        public virtual void ClearSimple()
        {
            OnValueChangedSimple = null;
        }
        public virtual void ClearComplex()
        {
            OnValueChangedComplex = null;
        }
        public virtual void ClearAll()
        {
            ClearSimple();
            ClearComplex();
        }

        public ValueEvent() : this(default(TValue))
        {

        }
        public ValueEvent(TValue value)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class BoolValueEvent : ValueEvent<bool> { }

    [Serializable]
    public class IntValueEvent : ValueEvent<int> { }

    [Serializable]
    public class FloatValueEvent : ValueEvent<float> { }

    [Serializable]
    public class CharValueEvent : ValueEvent<char> { }

    [Serializable]
    public class StringValueEvent : ValueEvent<string> { }
}