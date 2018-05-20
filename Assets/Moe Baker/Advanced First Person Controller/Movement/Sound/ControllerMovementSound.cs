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
        public ControllerGroundCheck GroundCheck { get { return Controller.Movement.GroundCheck; } }


        [SerializeField]
        protected MovementSoundSet defaultSet;
        public MovementSoundSet DefaultSet { get { return defaultSet; } }

        public MovementSoundSet OverrideSet { get; protected set; }

        public MovementSoundSet CurrentSet
        {
            get
            {
                if (OverrideSet != null)
                    return OverrideSet;

                return defaultSet;
            }
        }


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
            CheckGround();
        }

        public virtual void CheckGround()
        {
            if (GroundCheck.HasResault)
            {
                var soundSurface = GroundCheck.Resault.hit.collider.GetComponent<SoundSurface>();

                if (soundSurface != null)
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
    }

    public partial class ControllerMovementSound : ControllerMovementSoundBase
    {
        
    }
}