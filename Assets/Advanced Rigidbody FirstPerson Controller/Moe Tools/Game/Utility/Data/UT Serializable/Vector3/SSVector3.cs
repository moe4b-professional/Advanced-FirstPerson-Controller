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
    public struct SSVector3
    {
        public Vector3 UValue { get { return new Vector3(x, y, z); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float x, y, z;

        public SSVector3(Vector3 uValue) : this()
        {
            Assign(uValue);
        }
        public void Assign(Vector3 uValue)
        {
            Assign(uValue.x, uValue.y, uValue.z);
        }

        public SSVector3(float x, float y, float z) : this()
        {
            Assign(x, y, z);
        }
        public void Assign(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
                return Equals((SSVector3)obj);

            return false;
        }
        public bool Equals(SSVector3 obj)
        {
            return x == obj.x &&
                y == obj.y &&
                z == obj.z;
        }

        public static bool operator ==(SSVector3 obj1, SSVector3 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(SSVector3 obj1, SSVector3 obj2)
        {
            return !obj1.Equals(obj2);
        }
    }
}