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

namespace Moe.Tools
{
    [Serializable]
    public abstract class SerializableUCType<TUType>
    {
        public abstract TUType UValue { get; }

        public SerializableUCType()
        {

        }
        public SerializableUCType(TUType uValue)
        {
            Assign(uValue);
        }

        public abstract void Assign(TUType uValue);
    }
}