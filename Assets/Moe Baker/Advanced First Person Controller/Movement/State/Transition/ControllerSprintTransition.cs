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
	public abstract partial class ControllerSprintTransitionBase : ControllerStateTransition.Module
    {
        ControllerSprintTransition This { get { return this as ControllerSprintTransition; } }

        public override bool Control
        {
            get
            {
                return base.Control 
                    && Speed.Value.y >= 0f && 
                    InputModule.Move.magnitude * Movement.Control.AbsoluteScale >= minimumInput;
            }
            set
            {
                base.Control = value;
            }
        }

        public override ControllerState.IData Data { get { return State.Sprint; } }

        [SerializeField]
        [Range(0f, 1f)]
        protected float minimumInput = 0.75f;
        public float MinimumInput
        {
            get
            {
                return minimumInput;
            }
            set
            {
                minimumInput = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        protected InputMode input = InputMode.Hold;
        public InputMode Input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
            }
        }
        public enum InputMode
        {
            Hold, Toggle
        }

        public ControllerMovement Movement { get { return Controller.Movement; } }
        public ControllerSpeed Speed { get { return Movement.Speed; } }


        public override bool Process()
        {
            if (input == InputMode.Hold)
                return ProcessHoldInput();
            else if (input == InputMode.Toggle)
                return ProcessToggleInput();

            return false;
        }

        protected virtual bool ProcessHoldInput()
        {
            if (InputModule.Sprint && Control)
            {
                if (TargetState != Data)
                {
                    Traverser.GoTo(Data);
                    return true;
                }
            }
            else
            {
                if (TargetState == Data)
                {
                    Traverser.GoTo(State.Walk);
                    return true;
                }
            }

            return false;
        }

        bool ToggleLock = false;
        protected virtual bool ProcessToggleInput()
        {
            if(InputModule.Sprint && Control)
            {
                if (!ToggleLock)
                {
                    ToggleLock = true;

                    if (TargetState == Data)
                        Traverser.GoTo(State.Walk);
                    else
                        Traverser.GoTo(Data);

                    return true;
                }
            }
            else
                ToggleLock = false;

            if(!Control && TargetState == Data)
            {
                Traverser.GoTo(State.Walk);
                return true;
            }

            return false;
        }
    }

    public partial class ControllerSprintTransition : ControllerSprintTransitionBase
    {

    }
}