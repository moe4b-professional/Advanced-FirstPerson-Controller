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
    public struct SSVector2
    {
        public Vector2 UValue { get { return new Vector2(x, y); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float x, y;

        public SSVector2(Vector2 uValue) : this()
        {
            Assign(uValue);
        }
        public void Assign(Vector2 uValue)
        {
            Assign(uValue.x, uValue.y);
        }

        public SSVector2(float x, float y) : this()
        {
            Assign(x, y);
        }
        public void Assign(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
                return Equals((SSVector2)obj);

            return false;
        }
        public bool Equals(SSVector2 obj)
        {
            return x == obj.x &&
                y == obj.y;
        }

        public static bool operator ==(SSVector2 obj1, SSVector2 obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(SSVector2 obj1, SSVector2 obj2)
        {
            return !obj1.Equals(obj2);
        }
    }
}