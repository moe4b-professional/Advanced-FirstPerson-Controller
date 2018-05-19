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
    public abstract partial class ControllerHeadbobBase : FPController.Module
    {
        [SerializeField]
        protected bool apply = true;
        public bool Apply
        {
            get
            {
                return apply;
            }
            set
            {
                apply = value;
            }
        }

        [SerializeField]
        protected AnimationCurve curve;
        public AnimationCurve Curve { get { return curve; } }

        [SerializeField]
        protected float scale = 0.4f;
        public float Scale { get { return scale; } }

        public ControllerStep Step { get { return Controller.Movement.Step; } }

        public float Value { get { return curve.Evaluate(Step.Rate) * scale; } }
        public Vector3 Offset { get { return Vector3.up * Value; } }

        public virtual void Process()
        {
            if(apply)
                Controller.CameraRig.camera.transform.localPosition += Offset;
        }
    }

    public partial class ControllerHeadbob : ControllerHeadbobBase
    {

    }
}