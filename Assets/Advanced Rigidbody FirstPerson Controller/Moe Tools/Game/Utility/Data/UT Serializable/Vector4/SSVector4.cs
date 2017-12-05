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
    public struct SSVector4
    {
        public Vector4 UValue { get { return new Vector4(x, y, z, w); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float x, y, z, w;

        public SSVector4(Vector4 uValue) : this()
        {
            Assign(uValue);
        }
        public void Assign(Vector4 uValue)
        {
            Assign(uValue.x, uValue.y, uValue.z, uValue.w);
        }

        public SSVector4(float x, float y, float z, float w) : this()
        {
            Assign(x, y, z, w);
        }
        public void Assign(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
                return Equals((SSVector4)obj);

            return false;
        }
        public bool Equals(SSVector4 obj)
        {
            return x == obj.x &&
                y == obj.y &&
                z == obj.z &&
                w == obj.w;
        }

        public static bool operator ==(SSVector4 obj1, SSVector4 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(SSVector4 obj1, SSVector4 obj2)
        {
            return !obj1.Equals(obj2);
        }
    }
}