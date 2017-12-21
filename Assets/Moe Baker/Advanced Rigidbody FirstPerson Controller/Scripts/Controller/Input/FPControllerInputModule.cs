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

namespace ARFC
{
    public class BaseFPControllerInputModule : InputModule
    {
        public const string MenuPath = FPController.MenuPath + "Input/";

        [SerializeField]
        protected Vector2 movement;
        public Vector2 Movement { get { return movement; } }

        [SerializeField]
        public float Walk { get { return movement.y; } protected set { movement.y = value; } }

        [SerializeField]
        public float Strafe { get { return movement.x; } protected set { movement.x = value; } }

        [SerializeField]
        protected Vector2 look;
        public Vector2 Look { get { return look; } }

        [SerializeField]
        [Range(-1f, 1f)]
        protected float lean = 0f;
        public float Lean { get { return lean; } }

        [SerializeField]
        protected bool jump;
        public bool Jump { get { return jump; } }

        [SerializeField]
        protected bool sprint;
        public bool Sprint { get { return sprint; } }

        [SerializeField]
        protected bool crouch;
        public bool Crouch { get { return crouch; } }

        [SerializeField]
        protected bool prone;
        public bool Prone { get { return prone; } }

        public override void Clear()
        {
            base.Clear();

            movement = Vector2.zero;
            look = Vector2.zero;

            lean = 0f;

            jump = false;
            crouch = false;
            sprint = false;
            prone = false;
        }
    }

    public partial class FPControllerInputModule : BaseFPControllerInputModule
	{
		
	}
}