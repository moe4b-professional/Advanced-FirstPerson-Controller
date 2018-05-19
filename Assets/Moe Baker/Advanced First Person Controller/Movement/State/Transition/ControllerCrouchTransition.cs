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
    public abstract partial class ControllerCrouchTransitionBase : ControllerStateTransition.ToggleModule
    {
        public ControllerSlideTransition Slide { get; protected set; }
        protected virtual void InitSlide()
        {
            Slide = Controller.Modules.Find<ControllerSlideTransition>();
        }

        public override ControllerState.IData Data { get { return State.Crouch; } }

        public override bool Input { get { return InputModule.Crouch; } }

        public ControllerMovement Movement { get { return Controller.Movement; } }

        public override void Init(FPController link)
        {
            base.Init(link);

            InitSlide();
        }

        public override bool Process()
        {
            if (Slide.Process())
                return true;

            return base.Process();
        }
    }

    public partial class ControllerCrouchTransition : ControllerCrouchTransitionBase
    {

    }
}