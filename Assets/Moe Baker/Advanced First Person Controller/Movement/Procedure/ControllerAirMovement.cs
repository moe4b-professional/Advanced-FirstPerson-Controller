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
    public abstract partial class ControllerAirMovementBase : ControllerMovementProcedure.Module
    {
        public override bool UpdateMaxSpeed { get { return false; } }

        [SerializeField]
        bool updateDirection = true;
        public override bool UpdateDirection { get { return updateDirection; } }

        [SerializeField]
        protected bool processStateTransitions = true;
        public override bool ProcessStateTransitions { get { return processStateTransitions; } }

        [SerializeField]
        protected float acceleration = 5f;
        public float Acceleration { get { return acceleration; } }

        [SerializeField]
        protected float deAcceleration = 5f;
        public float DeAcceleration { get { return deAcceleration; } }

        [SerializeField]
        protected float maxMovementSpeed = 1f;
        public float MaxMovementSpeed { get { return maxMovementSpeed; } }

        public override float Friction { get { return 0f; } }


        public override void Process()
        {
            base.Process();

            Speed.Calculate(Control.AbsoluteScale, Speed.Magnitude > maxMovementSpeed ? 0f : acceleration, deAcceleration, true);
        }

        public override void FixedProcess()
        {
            base.FixedProcess();

            Vector3 velocity;

            if (GroundCheck.Slope <= GroundCheck.MaxSlope)
            {
                velocity = Direction.Forward * Speed.Value.y +
                Direction.Right * Speed.Value.x;
            }
            else
                velocity = rigidbody.velocity;

            velocity = Vector3.ClampMagnitude(velocity, Speed.MaxValue);
            velocity.y = rigidbody.velocity.y;

            SetVelocity(velocity);

            Gravity.Apply();
        }
    }

    public partial class ControllerAirMovement : ControllerAirMovementBase
    {

    }
}