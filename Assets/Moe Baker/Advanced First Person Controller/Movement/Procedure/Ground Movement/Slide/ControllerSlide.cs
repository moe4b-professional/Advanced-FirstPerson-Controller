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
	public class ControllerSlide : FPController.Module
	{
        public ControllerMovement Movement { get { return Controller.Movement; } }

        public ControllerSpeed Speed { get { return Movement.Speed; } }

        public ControllerGroundCheck GroundCheck { get { return Movement.GroundCheck; } }

        public ControllerState State { get { return Movement.State; } }

        public ControllerStateTraverser StateTraverser { get { return State.Traverser; } }

        [SerializeField]
        protected StateData data;
        public StateData Data { get { return data; } }
        [Serializable]
        public class StateData : ControllerState.IData
        {
            public ControllerState.IData CrouchData { get; protected set; }
            public virtual void SetCrouchData(ControllerState.IData crouchData)
            {
                this.CrouchData = crouchData;
            }

            public virtual float Height { get { return CrouchData.Height; } }
            public virtual float Radius { get { return CrouchData.Radius; } }

            [SerializeField]
            protected float speed = 10f;
            public float Speed { get { return speed; } }
        }

        [SerializeField]
        protected float transitionSpeed = 6f;
        public float TransitionSpeed
        {
            get
            {
                return transitionSpeed;
            }
            set
            {
                if(value < 0f)
                {
                    return;
                }

                transitionSpeed = value;
            }
        }

        [SerializeField]
        protected float deAcceleration = 5;
        public float DeAcceleration
        {
            get
            {
                return deAcceleration;
            }
            set
            {
                if(value < 0f)
                {
                    return;
                }

                deAcceleration = value;
            }
        }

        [SerializeField]
        protected float stoppingSpeed = 1f;
        public float StoppingSpeed { get { return stoppingSpeed; } }

        [SerializeField]
        protected ControllerState.Type endState = ControllerState.Type.Crouch;
        public ControllerState.Type EndState { get { return endState; } }

        public virtual bool Active
        {
            get
            {
                return State.Traverser.Target == data;
            }
        }

        public ControllerSlideSound Sound { get; protected set; }
        protected virtual void InitSound()
        {
            Sound = Controller.Modules.Find<ControllerSlideSound>();
        }


        public override void Init(FPController link)
        {
            base.Init(link);

            data.SetCrouchData(State.Crouch);

            InitSound();
        }


        public event Action OnBeggining;
        public virtual void Begin()
        {
            if(!GroundCheck.Grounded)
            {
                Debug.LogWarning("Trying to slide but controller is not grounded");
                return;
            }

            if(State.Traverser.Target != State.Sprint)
            {
                Debug.LogWarning("Trying to slide but controller is not sprinting");
                return;
            }

            State.Traverser.GoTo(data);

            if (OnBeggining != null)
                OnBeggining();
        }


        public virtual void Process()
        {
            State.Traverser.Process(transitionSpeed);

            CalculateSpeed();

            if (Speed.Magnitude <= stoppingSpeed)
                End();

            Sound.Process();
        }
        protected virtual void CalculateSpeed()
        {
            Speed.CalculateDeAcceleration(deAcceleration);
        }


        public event Action OnEnd;
        public virtual void End()
        {
            if (endState == ControllerState.Type.Sprint && !State.Transition.Sprint.Control)
                State.Traverser.GoTo(ControllerState.Type.Crouch);
            else
                State.Traverser.GoTo(endState);

            if (OnEnd != null)
                OnEnd();
        }
    }
}