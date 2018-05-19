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
        protected float acceleration = 5f;
        public float Acceleration { get { return acceleration; } }

        [SerializeField]
        protected float deAcceleration = 2f;
        public float DeAcceleration { get { return deAcceleration; } }


        public override void Process()
        {
            base.Process();

            CalculateSpeed();
        }

        protected virtual void CalculateSpeed()
        {
            Speed.CalculateAcceleration(ControlScale, acceleration, false);
            Speed.CalculateDeAcceleration(deAcceleration);
        }


        public override void FixedProcess()
        {
            base.FixedProcess();

            var velocity = Direction.Forward * Speed.Value.y + 
                Direction.Right * Speed.Value.x + 
                Vector3.up * rigidbody.velocity.y;

            SetVelocity(velocity);

            Gravity.Apply();
        }
    }

    public partial class ControllerAirMovement : ControllerAirMovementBase
    {

    }
}