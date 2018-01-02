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

namespace ARFC
{
    [CreateAssetMenu(menuName = FPControllerInputModule.MenuPath + "Unity Input Module")]
	public class FPControllerUnityInput : FPControllerInputModule
    {
        [Header("Keys")]
        [SerializeField]
        string walkAxis = "Vertical";
        public string WalkAxis { get { return walkAxis; } }

        [SerializeField]
        string strafeAxis = "Horizontal";
        public string StrafeAxis { get { return strafeAxis; } }

        [SerializeField]
        string leanAxis = "Lean";
        public string LeanAxis { get { return leanAxis; } }

        [SerializeField]
        string jumpButton = "Jump";
        public string JumpButton { get { return jumpButton; } }

        [SerializeField]
        string sprintButton = "Sprint";
        public string SprintButton { get { return sprintButton; } }

        [SerializeField]
        string crouchButton = "Crouch";
        public string CrouchButton { get { return crouchButton; } }

        [SerializeField]
        string proneButton = "Prone";
        public string ProneButton { get { return proneButton; } }

        public override void UpdateInput()
        {
            Walk = Input.GetAxisRaw(walkAxis);
            Strafe = Input.GetAxisRaw(strafeAxis);

            lean = Input.GetAxisRaw(leanAxis);

            look.x = Input.GetAxis("Mouse X");
            look.y = Input.GetAxis("Mouse Y");

            jump = Input.GetButtonDown(jumpButton);
            sprint = Input.GetButton(sprintButton);

            crouch = Input.GetButtonDown(crouchButton);
            prone = Input.GetButtonDown(proneButton);
        }
    }
}
