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
    [RequireComponent(typeof(Camera))]
	public abstract partial class CameraRigCameraBase : CameraRig.Part
	{
        public ControllerLookTarget LookTarget { get { return Look.LookTarget; } }

        [SerializeField]
        protected Camera component;
        public Camera Component { get { return component; } }

        [SerializeField]
        protected float offset = 0.1f;
        public float Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if(value < 0f)
                {
                    return;
                }

                offset = value;
            }
        }

        [SerializeField]
        protected RangeData range = new RangeData(80f, 80f);
        public RangeData Range { get { return range; } }
        [Serializable]
        public class RangeData
        {
            public const float Min = 0f;
            public const float Max = 90f;

            [SerializeField]
            [Range(Min, Max)]
            protected float up;
            public float Up
            {
                get
                {
                    return up;
                }
                set
                {
                    up = Mathf.Clamp(value, Min, Max);
                }
            }

            [SerializeField]
            [Range(Min, Max)]
            protected float down;
            public float Down
            {
                get
                {
                    return down;
                }
                set
                {
                    down = Mathf.Clamp(value, Min, Max);
                }
            }

            public RangeData(float up, float down)
            {
                this.Up = up;
                this.Down = down;
            }
        }


        float targetLookScale;
        public float Sensitivity
        {
            get
            {
                return Look.Sensitivity.Vertical * targetLookScale * Look.Control.AbsoluteScale;
            }
        }

        public virtual float Input { get { return -InputModule.Look.y; } }


        public ControllerHeadbob Headbob { get; protected set; }
        protected virtual void InitHeadbob()
        {
            Headbob = CameraRig.Controller.Modules.Find<ControllerHeadbob>();
        }


        protected virtual void Reset()
        {
            component = GetComponent<Camera>();
        }


        public override void Init(CameraRig link)
        {
            base.Init(link);

            InitHeadbob();
        }


        public override void Process()
        {
            base.Process();

            ProcessLookTarget();

            ProcessInput();

            Headbob.Process();
        }

        protected override void ApplyState()
        {
            base.ApplyState();

            transform.localPosition = Vector3.up * (TransitionState.Height * MoeTools.Math.InvertScale(CameraRig.HeightScale) - offset);
        }

        protected virtual void ProcessLookTarget()
        {
            if (LookTarget.Position.HasValue)
            {
                var target = GetRotationTo(LookTarget.Position.Value);

                RotateTowards(target, LookTarget.Speed);

                targetLookScale = MoeTools.Math.InvertScale(Quaternion.Angle(target, transform.localRotation) / LookTarget.Range);
            }
            else
                targetLookScale = 1f;
        }

        protected virtual void ProcessInput()
        {
            var angles = transform.localEulerAngles;

            if (CameraRig.Look.Control.AbsoluteToggle)
            {
                if (Input > 0f)
                    angles.x = Mathf.MoveTowardsAngle(angles.x, range.Down, Mathf.Abs(Input) * Sensitivity);
                else if (Input < 0f)
                    angles.x = Mathf.MoveTowardsAngle(angles.x, -range.Up, Mathf.Abs(Input) * Sensitivity);
            }

            if(angles.x < 180f)
            {
                if (angles.x > range.Down)
                    angles.x = range.Down;
            }
            else
            {
                if (angles.x < 360f - range.Up)
                    angles.x = 360f - range.Up;
            }

            transform.localEulerAngles = angles;
        }


        public virtual Quaternion GetRotationTo(Vector3 position)
        {
            var direction = (position - transform.position);

            var angles = Quaternion.LookRotation(direction).eulerAngles;

            angles.x = angles.y = 0f;

            return Quaternion.Euler(angles);
        }
        public virtual void LookAt(Vector3 position, float speed)
        {
            RotateTowards(GetRotationTo(position), speed);
        }
        public virtual void RotateTowards(Quaternion target, float speed)
        {
            transform.localRotation =
                Quaternion.RotateTowards(transform.localRotation, target, speed * Time.deltaTime);
        }
    }

    public partial class CameraRigCamera : CameraRigCameraBase
    {

    }
}