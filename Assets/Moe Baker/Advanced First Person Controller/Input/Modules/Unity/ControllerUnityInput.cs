﻿using System;
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
    [CreateAssetMenu(menuName = MenuPath + "Unity")]
	public class ControllerUnityInput : ControllerInputModule
	{
		[SerializeField]
        protected CombinedAxisData moveAxis = new CombinedAxisData("Horizontal", "Vertical");
        public CombinedAxisData MoveAxis { get { return moveAxis; } }

        [SerializeField]
        protected string jumpButton = "Jump";
        public string JumpButton { get { return jumpButton; } }

        [SerializeField]
        protected string sprintButton = "Sprint";
        public string SprintButton { get { return sprintButton; } }

        [SerializeField]
        protected string crouchButton = "Crouch";
        public string CrouchButton { get { return crouchButton; } }

        [SerializeField]
        protected string proneButton = "Prone";
        public string ProneButton { get { return proneButton; } }


        [SerializeField]
        protected CombinedAxisData lookAxis = new CombinedAxisData("Mouse");
        public CombinedAxisData LookAxis { get { return lookAxis; } }

        [SerializeField]
        protected AxisData leanAxis = new AxisData("Lean");
        public AxisData LeanAxis { get { return leanAxis; } }

        public override void Process()
        {
            base.Process();

            Move = moveAxis.RawValue;

            Jump = Input.GetButtonDown(jumpButton);
            Sprint = Input.GetButton(sprintButton);

            Crouch = Input.GetButtonDown(crouchButton);
            Prone = Input.GetButtonDown(proneButton);


            Look = lookAxis.Value;

            Lean = (int)leanAxis.RawValue;
        }
    }
}