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
    public abstract partial class ControllerStateTransitionBase : FPController.Module
    {
        public ControllerCrouchTransition Crouch { get; protected set; }
        protected virtual void InitCrocuh()
        {
            Crouch = Controller.Modules.Find<ControllerCrouchTransition>();
        }

        public ControllerProneTransition Prone { get; protected set; }
        protected virtual void InitProne()
        {
            Prone = Controller.Modules.Find<ControllerProneTransition>();
        }

        public ControllerSprintTransition Sprint { get; protected set; }
        protected virtual void InitSprint()
        {
            Sprint = Controller.Modules.Find<ControllerSprintTransition>();
        }

        public abstract partial class ModuleBase : FPController.Module
        {
            [SerializeField]
            protected bool _control = true;
            public virtual bool Control
            {
                get
                {
                    return _control && State.Control;
                }
                set
                {
                    _control = value;
                }
            }

            public abstract ControllerState.IData Data { get; }

            public ControllerState State { get { return Controller.Movement.State; } }
            public ControllerStateTraverser Traverser { get { return State.Traverser; } }
            public ControllerState.IData TargetState { get { return Traverser.Target; } }

            public abstract bool Process();
        }

        public abstract partial class ToggleModuleBase : ControllerStateTransition.Module
        {
            [SerializeField]
            protected bool toggle = true;
            public virtual bool Toggle
            {
                get
                {
                    return toggle;
                }
                set
                {
                    toggle = value;
                }
            }

            public abstract bool Input { get; }

            public virtual ControllerState.IData NeutralState { get { return State.Walk; } }

            public override bool Process()
            {
                if (Input && Control)
                {
                    if (TargetState == Data)
                    {
                        if (toggle)
                        {
                            Traverser.GoTo(NeutralState);
                            return true;
                        }
                    }
                    else
                    {
                        Traverser.GoTo(Data);
                        return true;
                    }
                }

                return false;
            }
        }


        public override void Init(FPController link)
        {
            base.Init(link);

            InitCrocuh();
            InitProne();
            InitSprint();
        }


        public virtual void Process()
        {
            if(Crouch.Process())
            {

            }
            else if(Prone.Process())
            {

            }
            else if(Sprint.Process())
            {

            }
        }
    }

    public partial class ControllerStateTransition : ControllerStateTransitionBase
    {
        public abstract partial class Module : ModuleBase
        {

        }

        public abstract partial class ToggleModule : ToggleModuleBase
        {

        }
    }
}