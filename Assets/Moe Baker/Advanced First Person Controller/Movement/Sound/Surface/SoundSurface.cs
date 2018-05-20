using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace AFPC
{
    public class SoundSurface : MonoBehaviour
    {
        [SerializeField]
        MovementSoundSet set;
        public MovementSoundSet Set { get { return set; } }
    }
}