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

namespace AFPC
{
	public class ControllerStepSoundBase : FPController.Module
	{
        [SerializeField]
        protected bool apply = true;
        public bool Apply { get { return apply; } }

        public ControllerStep Step { get { return Controller.Movement.Step; } }

        public ControllerMovementSound MovementSound { get { return Controller.Movement.Sound; } }
        public MovementSoundSet Set { get { return MovementSound.CurrentSet; } }
        public AudioSource AudioSource { get { return MovementSound.audio; } }

        public override void Init(FPController link)
        {
            base.Init(link);

            Step.OnComplete += Play;
        }

        protected virtual void Play()
        {
            if(apply && Set != null)
                AudioSource.PlayOneShot(Set.Get(Controller).RandomClip);
        }
    }

    public partial class ControllerStepSound : ControllerStepSoundBase
    {

    }
}