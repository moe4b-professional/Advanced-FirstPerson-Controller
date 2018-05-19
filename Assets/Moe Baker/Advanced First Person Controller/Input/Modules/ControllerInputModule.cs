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
    public abstract class ControllerInputModule : InputModule
	{
        public const string MenuPath = ControllerInputModulator.MenuPath + "Modules/";

        public Vector2 Move { get; protected set; }

        public bool Jump { get; protected set; }

        public bool Sprint { get; protected set; }

        public bool Crouch { get; protected set; }
        public bool Prone { get; protected set; }

        public Vector2 Look { get; protected set; }
        public int Lean { get; protected set; }

        public override void Clear()
        {
            base.Clear();

            Move = Vector2Int.zero;
            Look = Vector2.zero;
        }
    }
}