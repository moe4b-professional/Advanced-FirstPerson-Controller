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
	public abstract partial class ControllerGroundMovementBase : ControllerMovementProcedure.Module
    {
        [SerializeField]
        protected ControllerSpeed.Modifiers speedModifiers = new ControllerSpeed.Modifiers(20f, true);
        public ControllerSpeed.Modifiers SpeedModifiers { get { return speedModifiers; } }

        public override float Friction { get { return Speed.IsStale ? 1f : 0f; } }
        public override float StateTransitionSpeed
        {
            get
            {
                if (Slide.Active)
                    return Slide.TransitionSpeed;

                return base.StateTransitionSpeed;
            }
        }
        public override bool Step
        {
            get
            {
                return !Slide.Active;
            }
        }

        public ControllerSlide Slide { get; protected set; }
        protected virtual void InitSlide()
        {
            Slide = Controller.Modules.Find<ControllerSlide>();
        }

        public override void Init(FPController link)
        {
            base.Init(link);

            GroundCheck.OnLeave += OnLeftGround;

            InitSlide();
        }

        protected virtual void OnLeftGround()
        {
            if (Slide.Active)
                Slide.End();
        }

        Vector3 velocity = Vector3.zero;
        public override void Process()
        {
            base.Process();

            if (Slide.Active)
                Slide.Process();
            else
            {
                Speed.Calculate(Control.AbsoluteScale, speedModifiers);
                Direction.Calculate();
            }

            Jump.Process();

            GroundCheck.Do();

            velocity = Direction.Forward * Speed.Value.y + Direction.Right * Speed.Value.x;
            velocity = Vector3.ProjectOnPlane(velocity, GroundCheck.Resault.hit.normal);
        }

        public override void FixedProcess()
        {
            base.FixedProcess();

            ApplyMove();
            ApplyY();

            Gravity.Apply();
        }

        protected virtual void ApplyMove()
        {
            SetVelocity(new Vector3(velocity.x, rigidbody.velocity.y, velocity.z));
        }
        protected virtual void ApplyY()
        {
            if (velocity.y >= 0f) return;

            rigidbody.AddForce(Vector3.up * velocity.y * 1.2f, ForceMode.VelocityChange);
        }
    }

    public partial class ControllerGroundMovement : ControllerGroundMovementBase
    {

    }
}