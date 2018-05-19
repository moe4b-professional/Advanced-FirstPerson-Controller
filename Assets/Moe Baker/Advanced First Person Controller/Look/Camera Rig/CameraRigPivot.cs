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
	public abstract partial class CameraRigPivotBase : CameraRig.Part
	{
        public ControllerLean Lean { get; protected set; }
        protected virtual void InitLean()
        {
            Lean = CameraRig.Controller.Modules.Find<ControllerLean>();
        }


        public override void Init(CameraRig link)
        {
            base.Init(link);

            InitLean();
        }


        public override void Process()
        {
            base.Process();

            Lean.Process();
        }

        protected override void ApplyState()
        {
            base.ApplyState();

            transform.localPosition = Vector3.up * (TransitionState.Height * CameraRig.HeightScale);
        }
    }

    public partial class CameraRigPivot : CameraRigPivotBase
    {

    }
}