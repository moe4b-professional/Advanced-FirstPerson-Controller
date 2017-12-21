using System;
using System.IO;
using System.Linq;
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
using URandom = UnityEngine.Random;
using System.Runtime.Serialization.Formatters.Binary;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Serialization
        {
            public static class Binary
            {
                public static byte[] GetBytes(object obj)
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, obj);

                        return ms.ToArray();
                    }
                }

                public static T GetObject<T>(byte[] data)
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        object obj = bf.Deserialize(ms);

                        return (T)obj;
                    }
                }
            }
        }
    }
}