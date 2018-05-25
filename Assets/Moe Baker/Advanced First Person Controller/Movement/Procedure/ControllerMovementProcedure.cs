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
	public abstract partial class ControllerMovementProcedureBase : FPController.Module
	{
        public ControllerMovement Movement { get { return Controller.Movement; } }

        public ControllerState State { get { return Movement.State; } }

        public ControllerDirection Direction { get { return Movement.Direction; } }
        public ControllerSpeed Speed { get { return Movement.Speed; } }
        public ControllerJump Jump { get { return Movement.Jump; } }
        public ControllerStep Step { get { return Movement.Step; } }


        public ControllerGroundCheck GroundCheck { get { return Movement.GroundCheck; } }
        public ControllerGravity Gravity { get { return Movement.Gravity; } }

        public ControllerGroundMovement Ground { get; protected set; }
        protected virtual void InitGround()
        {
            Ground = Controller.Modules.Find<ControllerGroundMovement>();
        }

        public ControllerAirMovement Air { get; protected set; }
        protected virtual void InitAir()
        {
            Air = Controller.Modules.Find<ControllerAirMovement>();
        }

        ControllerMovementProcedure.Module _override;
        public ControllerMovementProcedure.Module Override
        {
            get
            {
                return _override;
            }
            set
            {
                if(value == null)
                {

                }
                else
                {

                }

                _override = value;
            }
        }

        public virtual ControllerMovementProcedure.Module Current
        {
            get
            {
                if (Override != null) return Override;

                if (GroundCheck.Grounded)
                    return Ground;
                else
                    return Air;
            }
        }

        public abstract partial class ModuleBase : FPController.Module
        {
            public ControllerMovement Movement { get { return Controller.Movement; } }

            public ControllerMovementProcedure Procedure { get { return Movement.Procedure; } }

            public ControllerState State { get { return Movement.State; } }

            public ControllerDirection Direction { get { return Movement.Direction; } }
            public ControllerSpeed Speed { get { return Movement.Speed; } }
            public ControllerJump Jump { get { return Movement.Jump; } }

            public ControllerGroundCheck GroundCheck { get { return Movement.GroundCheck; } }
            public ControllerGravity Gravity { get { return Movement.Gravity; } }


            public virtual ControlConstraint Control { get { return Movement.Control; } }


            public virtual float Friction { get { return 0f; } }
            public virtual bool ProcessStateTransitions { get { return true; } }
            public virtual float StateTransitionSpeed { get { return State.Traverser.Speed; } }
            public virtual bool UpdateMaxSpeed { get { return true; } }
            public virtual bool UpdateDirection { get { return true; } }
            public virtual bool Step { get { return false; } }


            public virtual void Process()
            {

            }

            public virtual void FixedProcess()
            {

            }


            protected virtual void SetVelocity(Vector3 value)
            {
                Movement.SetVelocity(value);
            }
            protected virtual void AddVelocity(Vector3 value)
            {
                Movement.AddVelocity(value);
            }
        }


        public override void Init(FPController link)
        {
            base.Init(link);

            InitGround();
            InitAir();
        }


        public virtual void Process()
        {
            GroundCheck.Do();

            if (Current.UpdateDirection) Direction.Calculate();

            if (Current.ProcessStateTransitions) State.Transition.Process();

            if (Current.UpdateMaxSpeed) Speed.UpdateMaxValue();

            State.Traverser.Process(Current.StateTransitionSpeed);

            Current.Process();

            SetFriction(Current.Friction);

            Step.Process(Current.Step);
        }


        public virtual void FixedProcess()
        {
            Current.FixedProcess();
        }


        public virtual void SetFriction(float value)
        {
            if (collider.material == null) return;

            collider.material.dynamicFriction = value;
            collider.material.staticFriction = value;
        }
    }

    public partial class ControllerMovementProcedure : ControllerMovementProcedureBase
    {
        public abstract partial class Module : ModuleBase
        {

        }
    }
}