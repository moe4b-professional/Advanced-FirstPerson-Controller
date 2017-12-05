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
    public class SCQuaternion : SerializableUCType<Quaternion>
    {
        public override Quaternion UValue { get { return new Quaternion(x, y, z, w); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float x, y, z, w;

        public SCQuaternion(Quaternion uValue) : base(uValue)
        {
            Assign(uValue);
        }
        public override void Assign(Quaternion uValue)
        {
            Assign(uValue.x, uValue.y, uValue.z, uValue.w);
        }

        public SCQuaternion(float x, float y, float z, float w)
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
    }
}