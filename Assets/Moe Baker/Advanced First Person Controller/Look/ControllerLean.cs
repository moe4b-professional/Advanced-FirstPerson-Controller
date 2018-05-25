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
	public abstract partial class ControllerLeanBase : FPController.Module
	{
        [SerializeField]
        protected ControlConstraint control;
        public ControlConstraint Control { get { return control; } }

        public const float MaxRange = 80f;
        [SerializeField]
        [Range(0f, MaxRange)]
        protected float range = MaxRange / 2f;
        public float Range
        {
            get
            {
                return range;
            }
            set
            {
                range = Mathf.Clamp(value, 0f, MaxRange);

                range = value;
            }
        }

        public virtual float Value { get; protected set; }

        [SerializeField]
        protected float speed = 200f;
        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (value < 0f)
                {
                    return;
                }

                speed = value;
            }
        }

        [SerializeField]
        [Range(0f, 1f)]
        protected float cameraAlignment = 0.8f;
        public float CameraAlignment
        {
            get
            {
                return cameraAlignment;
            }
            set
            {
                cameraAlignment = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        protected LayerMask mask = Physics.DefaultRaycastLayers;
        public LayerMask Mask { get { return mask; } }

        [SerializeField]
        protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        public QueryTriggerInteraction TriggerInteraction { get { return triggerInteraction; } }

        [SerializeField]
        protected float offset = 0.1f;
        public float Offset { get { return offset; } }

        public CameraRig CameraRig { get { return Controller.Look.CameraRig; } }
        public CameraRigPivot CameraRigPivot { get { return CameraRig.Pivot; } }
        public CameraRigCamera CameraRigCamera { get { return CameraRig.camera; } }

        public override void Init(FPController link)
        {
            base.Init(link);

            control.SetContext(Controller.Look.Control);
        }

        public virtual void Process()
        {
            if (InputModule.Lean != 0f)
            {
                if (LeanCheck(offset))
                    UpdateValue(0f);
                else if (!LeanCheck(offset + 0.1f))
                    UpdateValue(InputModule.Lean * control.AbsoluteScale);
            }
            else
                UpdateValue(0f);

            Apply();
        }

        public virtual void UpdateValue(float input)
        {
            Value = Mathf.MoveTowards(Value, range * -Mathf.Sign(input) * Mathf.Abs(input), speed * Time.deltaTime);
        }

        protected virtual void Apply()
        {
            ApplyPivot();
            ApplyCamera();
        }
        protected virtual void ApplyPivot()
        {
            var angles = CameraRigPivot.transform.localEulerAngles;

            angles.z = Value;

            CameraRigPivot.transform.localEulerAngles = angles;
        }
        protected virtual void ApplyCamera()
        {
            var angles = CameraRigCamera.transform.localEulerAngles;

            angles.z = -Value * cameraAlignment;

            CameraRigCamera.transform.localEulerAngles = angles;
        }


        public virtual Vector3 Start
        {
            get
            {
                return CameraRigPivot.transform.position;
            }
        }
        public virtual Vector3 End
        {
            get
            {
                return CameraRigCamera.transform.position;
            }
        }
        public virtual Vector3 Direction
        {
            get
            {
                if (InputModule.Lean > 0f)
                    return CameraRigCamera.transform.right;
                else if (InputModule.Lean < 0f)
                    return -CameraRigCamera.transform.right;
                else
                    return Vector3.zero;
            }
        }

        protected virtual bool LeanCheck(float offset)
        {
            Debug.DrawLine(Start, End + (Direction * offset));

            if (Physics.Linecast(Start, End + (Direction * offset), mask, triggerInteraction))
            {
                return true;
            }

            return false;
        }
    }

    public partial class ControllerLean : ControllerLeanBase
    {

    }
}