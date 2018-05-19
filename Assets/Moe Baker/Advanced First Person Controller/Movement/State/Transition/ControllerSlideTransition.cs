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
	public abstract partial class ControllerSlideTransitionBase : ControllerStateTransition.Module
    {
        public ControllerMovement Movement { get { return Controller.Movement; } }

        public override ControllerState.IData Data { get { return Movement.Procedure.Ground.Slide.Data; } }

        public override bool Process()
        {
            if (InputModule.Crouch && Control && TargetState == State.Sprint && Movement.GroundCheck.Grounded)
            {
                Movement.Procedure.Ground.Slide.Begin();
                return true;
            }

            return false;
        }
    }

    public partial class ControllerSlideTransition : ControllerSlideTransitionBase
    {

    }
}