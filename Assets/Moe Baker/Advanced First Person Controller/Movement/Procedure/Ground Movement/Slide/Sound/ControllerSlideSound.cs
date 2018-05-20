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
	public abstract partial class ControllerSlideSoundBase : FPController.Module
	{
        public ControllerMovementSound MovementSound { get { return Controller.Movement.Sound; } }

        new public AudioSource audio { get { return MovementSound.audio; } }


        public ControllerSlide Slide { get { return Controller.Movement.Procedure.Ground.Slide; } }


        public override void Init(FPController link)
        {
            base.Init(link);

            Slide.OnBeggining += OnBeggining;
            Slide.OnEnd += OnEnd;
        }


        protected virtual void OnBeggining()
        {
            if(MovementSound.CurrentSet != null)
            {
                audio.clip = MovementSound.CurrentSet.Slide;
                audio.Play();
            }
        }


        public virtual void Process()
        {

        }


        protected virtual void OnEnd()
        {
            audio.clip = null;
            audio.Stop();
        }
    }

    public partial class ControllerSlideSound : ControllerSlideSoundBase
    {

    }
}