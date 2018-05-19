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
    public abstract partial class ControllerStateTraverserBase : FPController.Module
    {
        public ControllerState State { get { return Controller.Movement.State; } }

        public ControllerMovement Movement { get { return Controller.Movement; } }

        public ControllerRoofCheck RoofChek { get { return Movement.RoofCheck; } }


        public ControllerState.IData Previous { get; protected set; }

        public TransitionData Transition { get; protected set; }
        [Serializable]
        public partial struct TransitionData : ControllerState.IData
        {
            public float Height { get; private set; }
            public float Radius { get; private set; }
            public float Speed { get; private set; }

            public static TransitionData Lerp(ControllerState.IData state1, ControllerState.IData state2, float t)
            {
                return new TransitionData()
                {
                    Height = Mathf.Lerp(state1.Height, state2.Height, t),
                    Radius = Mathf.Lerp(state1.Radius, state2.Radius, t),
                    Speed = Mathf.Lerp(state1.Speed, state2.Speed, t),
                };
            }

            public TransitionData(ControllerState.IData state)
            {
                if (state == null)
                    throw new NullReferenceException();

                this.Height = state.Height;
                this.Radius = state.Radius;
                this.Speed = state.Speed;
            }
        }
        protected virtual void UpdateTransition()
        {
            Transition = TransitionData.Lerp(Previous, Target, Lerp);
        }

        public ControllerState.IData Target { get; protected set; }


        [SerializeField]
        protected float speed = 5;
        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if(value < 0f)
                {
                    Debug.LogWarning("Cannot set " + typeof(ControllerStateTraverser).Name + "'s speed to the negative value of " + value.ToString());
                    return;
                }

                speed = value;
            }
        }
        public float Lerp { get; protected set; }

        public delegate void ChangeDelegate(ControllerState.IData state);
        public event ChangeDelegate OnChangeStart;
        public event ChangeDelegate OnChangeEnd;

        public override void Init(FPController link)
        {
            base.Init(link);

            Previous = Target = State.StartStateData;
            Transition = new TransitionData(Target);

            Lerp = 1f;
        }

        public virtual void Process()
        {
            Process(speed);
        }
        public virtual void Process(float speed)
        {
            if (Lerp != 1f)
            {
                ProcessRoofCheck();

                if (Lerp != 1f)
                {
                    Lerp = Mathf.MoveTowards(Lerp, 1f, speed * Time.deltaTime);

                    UpdateTransition();

                    if (Lerp == 1f)
                    {
                        if (OnChangeEnd != null)
                            OnChangeEnd(Target);
                    }
                }

                Apply();
            }
        }

        protected virtual void ProcessRoofCheck()
        {
            RoofChek.Do();

            if (RoofChek.HasResault && Target.Height > Previous.Height)
                GoTo(GetSafeState());
        }
        protected virtual ControllerState.IData GetSafeState()
        {
            if (State.Crouch.Height < Transition.Height)
                return State.Crouch;

            return State.Prone;
        }

        protected virtual void Apply()
        {
            collider.height = Transition.Height;
            collider.center = Vector3.up * collider.height / 2f;

            collider.radius = Transition.Radius;
        }

        public virtual void GoTo(ControllerState.Type type)
        {
            GoTo(State.GetData(type));
        }

        public virtual void GoTo(ControllerState.IData state)
        {
            GoTo(state, Mathf.InverseLerp(Target.Height, state.Height, Transition.Height));
        }
        protected virtual void GoTo(ControllerState.IData state, float lerpScale)
        {
            lerpScale = Mathf.Clamp01(lerpScale);

            Previous = Target;
            Target = state;

            Lerp = lerpScale;

            UpdateTransition();

            if (OnChangeStart != null)
                OnChangeStart(state);
        }
    }

    public partial class ControllerStateTraverser : ControllerStateTraverserBase
    {
        
    }
}