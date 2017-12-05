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
using System.Runtime.Serialization;

namespace Moe.Tools
{
    [Serializable]
    [DataContract]
    public struct SSColor
    {
        public Color UValue { get { return new Color(r, g, b, a); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float r, g, b, a;

        public SSColor(Color uValue) : this()
        {
            Assign(uValue);
        }
        public void Assign(Color uValue)
        {
            Assign(uValue.r, uValue.g, uValue.b, uValue.a);
        }

        public SSColor(float r, float g, float b, float a) : this()
        {
            Assign(r, g, b, a);
        }
        public void Assign(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override int GetHashCode()
        {
            return r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode() ^ b.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
                return Equals((SSColor)obj);

            return false;
        }
        public bool Equals(SSColor obj)
        {
            return r == obj.r &&
                g == obj.g &&
                b == obj.b &&
                a == obj.a;
        }

        public static bool operator ==(SSColor obj1, SSColor obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(SSColor obj1, SSColor obj2)
        {
            return !obj1.Equals(obj2);
        }
    }
}