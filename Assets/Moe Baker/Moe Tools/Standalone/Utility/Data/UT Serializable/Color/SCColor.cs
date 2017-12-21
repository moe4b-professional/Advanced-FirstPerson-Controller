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
    public class SCColor : SerializableUCType<Color>
    {
        public override Color UValue { get { return new Color(r, g, b, a); } }

        [DataMember(IsRequired = true)]
        [SerializeField]
        public float r, g, b, a;

        public SCColor(Color uValue) : base(uValue)
        {
            Assign(uValue);
        }
        public override void Assign(Color uValue)
        {
            Assign(uValue.r, uValue.g, uValue.b, uValue.a);
        }

        public SCColor(float r, float g, float b, float a)
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
    }
}