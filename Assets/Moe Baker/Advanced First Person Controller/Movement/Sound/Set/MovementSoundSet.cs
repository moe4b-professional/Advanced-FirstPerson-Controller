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
using Random = UnityEngine.Random;

using Moe.Tools;

namespace AFPC
{
    [CreateAssetMenu(menuName = FPController.MenuPath + "Step Sounds Set")]
    public class MovementSoundSet : ControllerState.DataSet<SoundSet>
    {
        [SerializeField]
        protected AudioClip slide;
        public AudioClip Slide { get { return slide; } }
    }
}