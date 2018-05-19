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
    [CreateAssetMenu(menuName = MoeTools.Constants.Paths.Tools + "Tag Element")]
	public class TagElement : ScriptableObject, IEquatable<string>, IComparable<string>
	{
        public virtual string Value { get { return name; } set { name = value; } }

        public virtual bool Equals(string other)
        {
            return Value.Equals(other);
        }

        public int CompareTo(string other)
        {
            return Value.CompareTo(other);
        }
    }

    public class PopupTagElementAttribute : PropertyAttribute
    {

    }
}