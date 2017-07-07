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

[CreateAssetMenu(menuName = "Sound/Controller/States")]
public class ControllerSoundStates : ControllerStatesScritableObjectBase<ControllerSoundStates.SetData>
{
    [Space]
    [SerializeField]
    SoundSet jump;
    public SoundSet Jump { get { return jump; } }

    [SerializeField]
    SoundSet landing;
    public SoundSet Landing { get { return landing; } }

    [Serializable]
    public class SetData
    {
        [SerializeField]
        float stepInterval = 1f;
        public float StepInterval { get { return stepInterval; } }

        [SerializeField]
        MovementSoundSet set;
        public MovementSoundSet Set { get { return set; } }
    }
}