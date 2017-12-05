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
    public struct SSResolution
    {
        public Resolution UValue
        {
            get
            {
                return new Resolution() { height = height, width = width, refreshRate = refreshRate };
            }
        }

        [SerializeField]
        [DataMember(IsRequired = true)]
        public int width, height, refreshRate;

        public SSResolution(Resolution uValue) : this()
        {
            Assign(uValue);
        }
        public void Assign(Resolution uValue)
        {
            Assign(uValue.width, uValue.height, uValue.refreshRate);
        }

        public SSResolution(int width, int height, int refreshRate) : this()
        {
            Assign(width, height, refreshRate);
        }
        public void Assign(int width, int height, int refreshRate)
        {
            this.width = width;
            this.height = height;
            this.refreshRate = refreshRate;
        }

        public override int GetHashCode()
        {
            return width.GetHashCode() ^ height.GetHashCode() ^ refreshRate.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
                return Equals((SSResolution)obj);

            return false;
        }
        public bool Equals(SSResolution obj)
        {
            return width == obj.width &&
                height == obj.height &&
                refreshRate == obj.refreshRate;
        }

        public static bool operator ==(SSResolution obj1, SSResolution obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(SSResolution obj1, SSResolution obj2)
        {
            return !obj1.Equals(obj2);
        }
    }
}