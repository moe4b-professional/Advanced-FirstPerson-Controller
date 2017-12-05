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
    public class SCVector3 : SerializableUCType<Vector3>
    {
        public override Vector3 UValue { get { return new Vector3(x, y, z); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float x, y, z;

        public SCVector3(Vector3 uValue) : base(uValue)
        {

        }
        public override void Assign(Vector3 uValue)
        {
            Assign(uValue.x, uValue.y, uValue.z);
        }

        public SCVector3(float x, float y, float z)
        {
            Assign(x, y, z);
        }
        public void Assign(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}