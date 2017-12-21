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

using Moe.Tools;

namespace ARFC
{
    public interface IControllerSoundStates : IControllerStatesDataTemplate<SoundSet>
    {
        SoundSet Jump { get; }
        SoundSet Land { get; }
    }

    [CreateAssetMenu(menuName = FPController.MenuPath + "Sound States")]
    public class ControllerSoundStates : ControllerStatesScriptableObjectTemplate<SoundSet>, IControllerSoundStates
    {
        [Space]
        [SerializeField]
        SoundSet jump;
        public SoundSet Jump { get { return jump; } }

        [SerializeField]
        SoundSet land;
        public SoundSet Land { get { return land; } }
    }
}