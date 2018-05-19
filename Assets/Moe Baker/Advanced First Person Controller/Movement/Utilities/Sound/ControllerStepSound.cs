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

        public ControllerGroundCheck GroundCheck { get { return Controller.Movement.GroundCheck; } }

        public ControllerStep Step { get { return Controller.Movement.Step; } }

        public ControllerMovementSound MovementSound { get { return Controller.Movement.Sound; } }
        public AudioSource AudioSource { get { return MovementSound.audio; } }

        [SerializeField]
        protected StepSoundSet defaultSet;
        public StepSoundSet DefaultSet { get { return defaultSet; } }

        public StepSoundSet OverrideSet { get; protected set; }

        public StepSoundSet CurrentSet
        {
            get
            {
                if (OverrideSet != null)
                    return OverrideSet;

                return defaultSet;
            }
        }


        public override void Init(FPController link)
        {
            base.Init(link);

            Step.OnComplete += Play;
        }

        public virtual void Process()
        {
            UpdateOverrideSet();
        }
        public virtual void UpdateOverrideSet()
        {
            if (GroundCheck.HasResault)
            {
                var soundSurface = GroundCheck.Resault.hit.collider.GetComponent<SoundSurface>();

                if(soundSurface != null)
                {
                    OverrideSet = soundSurface.Set;
                }
                else
                {
                    var terrainSoundSurface = GroundCheck.Resault.hit.collider.GetComponent<TerrainSoundSurface>();

                    if (terrainSoundSurface != null)
                        OverrideSet = terrainSoundSurface.GetSet(Controller.transform.position);
                }
            }
            else
                OverrideSet = null;
        }

        protected virtual void Play()
        {
            if(apply && CurrentSet != null)
                AudioSource.PlayOneShot(CurrentSet.Get(Controller).RandomClip);
        }
    }

    public partial class ControllerStepSound : ControllerStepSoundBase
    {

    }
}