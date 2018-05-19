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
    public abstract partial class CameraRigBase : FPController.Module
    {
        CameraRig This { get { return this as CameraRig; } }

        public ControllerLook Look { get { return Controller.Look; } }

        [SerializeField]
        protected CameraRigPivot pivot;
        public CameraRigPivot Pivot { get { return pivot; } }

        [SerializeField]
        protected CameraRigCamera _camera;
        new public CameraRigCamera camera { get { return _camera; } }

        public abstract partial class PartBase : MoeModule<CameraRig>
        {
            public CameraRig CameraRig { get { return Link; } }

            public FPController Controller { get { return CameraRig.Controller; } }

            public ControllerLook Look { get { return Controller.Look; } }

            public ControllerInputModule InputModule { get { return Controller.InputModule; } }

            public virtual void Process()
            {
                ApplyState();
            }

            public ControllerState.IData TransitionState { get { return CameraRig.Controller.Movement.State.Traverser.Transition; } }
            protected virtual void ApplyState()
            {

            }
        }


        [SerializeField]
        [Range(0f, 1f)]
        protected float heightScale = 0.5f;
        public float HeightScale
        {
            get
            {
                return heightScale;
            }
            protected set
            {
                heightScale = Mathf.Clamp01(value);
            }
        }


        public override void Init(FPController link)
        {
            base.Init(link);

            pivot.Init(This);
            camera.Init(This);
        }


        public virtual void Process()
        {
            pivot.Process();
            camera.Process();
        }
    }

	public partial class CameraRig : CameraRigBase
    {
		public abstract partial class Part : PartBase
        {

        }
	}
}