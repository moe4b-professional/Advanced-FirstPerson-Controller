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
    public class SCResolution : SerializableUCType<Resolution>
    {
        public override Resolution UValue
        {
            get
            {
                return new Resolution() { height = height, width = width, refreshRate = refreshRate };
            }
        }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public int width, height, refreshRate;

        public SCResolution(Resolution uValue) : base(uValue)
        {
            Assign(uValue);
        }
        public override void Assign(Resolution uValue)
        {
            Assign(uValue.width, uValue.height, uValue.refreshRate);
        }

        public SCResolution(int width, int height, int refreshRate)
        {
            Assign(width, height, refreshRate);
        }
        public void Assign(int width, int height, int refreshRate)
        {
            this.width = width;
            this.height = height;
            this.refreshRate = refreshRate;
        }
    }
}