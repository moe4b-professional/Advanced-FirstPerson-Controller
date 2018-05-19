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
    public abstract partial class ControllerProneTransitionBase : ControllerStateTransition.ToggleModule
    {
        public override ControllerState.IData Data { get { return State.Prone; } }

        public override bool Input { get { return InputModule.Prone; } }
    }

    public partial class ControllerProneTransition : ControllerProneTransitionBase
    {

    }
}