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

namespace ARFC
{
    public class SoundSurface : MonoBehaviour
    {
        [SerializeField]
        ControllerSoundStates soundData;
        public ControllerSoundStates SoundData { get { return soundData; } }
    }
}