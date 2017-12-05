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
    public class SCVector4 : SerializableUCType<Vector4>
    {
        public override Vector4 UValue { get { return new Vector4(x, y, z, w); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float x, y, z, w;

        public SCVector4(Vector4 uValue) : base(uValue)
        {

        }
        public override void Assign(Vector4 uValue)
        {
            Assign(uValue.x, uValue.y, uValue.z, uValue.w);
        }

        public SCVector4(float x, float y, float z, float w)
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