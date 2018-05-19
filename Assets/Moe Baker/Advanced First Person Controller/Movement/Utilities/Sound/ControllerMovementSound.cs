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
    [RequireComponent(typeof(AudioSource))]
    public abstract partial class ControllerMovementSoundBase : FPController.Module
    {
        new public AudioSource audio { get; protected set; }
        protected virtual void InitAudio()
        {
            audio = GetComponent<AudioSource>();
        }

        public ControllerStepSound Step { get; protected set; }
        protected virtual void InitStep()
        {
            Step = Controller.Modules.Find<ControllerStepSound>();
        }

        public override void Init(FPController link)
        {
            base.Init(link);

            InitAudio();

            InitStep();
        }

        public virtual void Process()
        {
            Step.Process();
        }
    }

    public partial class ControllerMovementSound : ControllerMovementSoundBase
    {
        
    }
}