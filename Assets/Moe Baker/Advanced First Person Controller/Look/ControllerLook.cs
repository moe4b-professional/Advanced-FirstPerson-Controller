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
    public abstract partial class ControllerLookBase : FPController.Module
    {
        ControllerLook This { get { return this as ControllerLook; } }

        [SerializeField]
        protected bool _control = true;
        public virtual bool Control
        {
            get
            {
                return _control && Controller.Control;
            }
            set
            {
                _control = value;
            }
        }
        
        [SerializeField]
        protected SensitvityData sensitivity = new SensitvityData(4f);
        public SensitvityData Sensitvity { get { return sensitivity; } }
        [Serializable]
        public class SensitvityData
        {
            [SerializeField]
            protected float vertical;
            public float Vertical
            {
                get
                {
                    return vertical;
                }
                set
                {
                    vertical = value;
                }
            }

            [SerializeField]
            protected float horizontal;
            public float Horizontal
            {
                get
                {
                    return horizontal;
                }
                set
                {
                    horizontal = value;
                }
            }

            public SensitvityData(float value) : this(value, value)
            {

            }
            public SensitvityData(float vertical, float horizontal)
            {
                this.vertical = vertical;
                this.horizontal = horizontal;
            }
        }

        [SerializeField]
        protected AccelerationData acceleration;
        public AccelerationData Acceleration { get { return acceleration; } }
        [Serializable]
        public class AccelerationData
        {

        }

        public CameraRig CameraRig { get; protected set; }
        protected virtual void InitCameraRig()
        {
            CameraRig = Controller.Modules.Find<CameraRig>();
        }

        public ControllerCharacterLook CharacterLook { get; protected set; }
        protected virtual void InitCharacterLook()
        {
            CharacterLook = Controller.Modules.Find<ControllerCharacterLook>();
        }

        public ControllerLookTarget LookTarget { get; protected set; }
        protected virtual void InitLookTarget()
        {
            LookTarget = Controller.Modules.Find<ControllerLookTarget>();
        }

        public override void Init(FPController link)
        {
            base.Init(link);

            InitCameraRig();

            InitCharacterLook();

            InitLookTarget();
        }


        public virtual void Process()
        {
            CharacterLook.Process();

            CameraRig.Process();
        }
    }

	public partial class ControllerLook : ControllerLookBase
    {
		
	}
}