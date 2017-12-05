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
    public class SCVector2 : SerializableUCType<Vector2>
    {
        public override Vector2 UValue { get { return new Vector2(x, y); } }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public float x, y;

        public SCVector2(Vector2 uValue) : base(uValue)
        {
            Assign(uValue);
        }
        public override void Assign(Vector2 uValue)
        {
            Assign(uValue.x, uValue.y);
        }

        public SCVector2(float x, float y)
        {
            Assign(x, y);
        }
        public void Assign(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}