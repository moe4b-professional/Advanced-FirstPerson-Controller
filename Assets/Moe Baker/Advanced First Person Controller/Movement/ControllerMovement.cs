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
	public abstract partial class ControllerMovementBase : FPController.Module
	{
        [SerializeField]
        protected ControlConstraint control;
        public ControlConstraint Control { get { return control; } }


        public ControllerDirection Direction { get; protected set; }
        protected virtual void InitDirection()
        {
            Direction = Controller.Modules.Find<ControllerDirection>();
        }

        public ControllerSpeed Speed { get; protected set; }
        protected virtual void InitSpeed()
        {
            Speed = Controller.Modules.Find<ControllerSpeed>();
        }

        public ControllerJump Jump { get; protected set; }
        protected virtual void InitJump()
        {
            Jump = Controller.Modules.Find<ControllerJump>();
        }

        public ControllerGroundCheck GroundCheck { get; protected set; }
        protected virtual void InitGroundCheck()
        {
            GroundCheck = Controller.Modules.Find<ControllerGroundCheck>();

            if (GroundCheck == null)
                throw MoeTools.ExceptionTools.Templates.MissingDependacny<ControllerGroundCheck, ControllerMovement>(name);
        }

        public ControllerRoofCheck RoofCheck { get; protected set; }
        protected virtual void InitRoofCheck()
        {
            RoofCheck = Controller.Modules.Find<ControllerRoofCheck>();

            if (RoofCheck == null)
                throw MoeTools.ExceptionTools.Templates.MissingDependacny<ControllerRoofCheck, ControllerMovement>(name);
        }

        public ControllerGravity Gravity { get; protected set; }
        protected virtual void InitGravity()
        {
            Gravity = Controller.Modules.Find<ControllerGravity>();
        }

        public ControllerStep Step { get; protected set; }
        protected virtual void InitStep()
        {
            Step = Controller.Modules.Find<ControllerStep>();
        }

        public ControllerMovementSound Sound { get; protected set; }
        protected virtual void InitSound()
        {
            Sound = Controller.Modules.Find<ControllerMovementSound>();
        }


        public ControllerState State { get; protected set; }
        protected virtual void InitState()
        {
            State = Controller.Modules.Find<ControllerState>();
        }

        public ControllerMovementProcedure Procedure { get; protected set; }
        protected virtual void InitProcedure()
        {
            Procedure = Controller.Modules.Find<ControllerMovementProcedure>();
        }


        public override void Init(FPController link)
        {
            base.Init(link);

            control.SetContext(Controller.Control);

            InitModules();
        }

        protected virtual void InitModules()
        {
            InitState();

            InitDirection();
            InitSpeed();
            InitJump();

            InitGroundCheck();
            InitRoofCheck();
            InitGravity();
            InitStep();
            InitSound();

            InitState();

            InitProcedure();
        }


        public virtual void Process()
        {
            Procedure.Process();

            Sound.Process();
        }


        public virtual void FixedProcess()
        {
            Procedure.FixedProcess();
        }


        public virtual void SetVelocity(Vector3 velocity)
        {
            rigidbody.velocity = velocity;
        }

        public virtual void AddVelocity(Vector3 velocity)
        {
            SetVelocity(rigidbody.velocity + velocity);
        }


        
    }

    public partial class ControllerMovement : ControllerMovementBase
    {
        
    }
}