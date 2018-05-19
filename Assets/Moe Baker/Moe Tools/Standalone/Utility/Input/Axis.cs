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
    public class CombinedAxisData
    {
        [SerializeField]
        protected AxisData x;
        public AxisData X { get { return x; } }
        public const string XPrefix = "X";

        [SerializeField]
        protected AxisData y;
        public AxisData Y { get { return y; } }
        public const string YPrefix = "Y";

        public virtual Vector2 Value
        {
            get
            {
                return new Vector2(x.Value, y.Value);
            }
        }
        public virtual Vector2 RawValue
        {
            get
            {
                return new Vector2(x.RawValue, y.RawValue);
            }
        }

        public static string Format(string axis, string prefix)
        {
            return axis + " " + prefix;
        }

        public CombinedAxisData(string axis) : this(Format(axis, XPrefix), Format(axis, YPrefix))
        {

        }
        public CombinedAxisData(string x, string y)
        {
            this.x = new AxisData(x);
            this.y = new AxisData(y);
        }
    }

    [Serializable]
    public class AxisData
    {
        [SerializeField]
        protected string name;
        public string Name { get { return name; } }

        public float RawValue { get { return Input.GetAxisRaw(name); } }
        public float Value { get { return Input.GetAxis(name); } }

        public AxisData(string name)
        {
            this.name = name;
        }
    }
}