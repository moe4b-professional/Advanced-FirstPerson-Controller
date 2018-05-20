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
    [CreateAssetMenu(menuName = MenuPath + "Key")]
	public class ControllerKeyInput : ControllerInputModule
	{
		[SerializeField]
        protected KeyCodeAxis walkAxis = new KeyCodeAxis(KeyCode.W, KeyCode.S);
        public KeyCodeAxis WalkAxis { get { return walkAxis; } }

        [SerializeField]
        protected KeyCodeAxis strafeAxis = new KeyCodeAxis(KeyCode.D, KeyCode.A);
        public KeyCodeAxis StrafeAxis { get { return strafeAxis; } }

        [SerializeField]
        protected KeyCode jumpKey = KeyCode.Space;
        public KeyCode JumpKey { get { return jumpKey; } }

        [SerializeField]
        protected KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode SprintKey { get { return sprintKey; } }

        [SerializeField]
        protected KeyCode crouchKey = KeyCode.C;
        public KeyCode CrouchKey { get { return crouchKey; } }

        [SerializeField]
        protected KeyCode pronekey = KeyCode.LeftControl;
        public KeyCode ProneKey { get { return pronekey; } }


        [SerializeField]
        protected CombinedAxisData lookAxis = new CombinedAxisData("Mouse");
        public CombinedAxisData LookAxis { get { return lookAxis; } }

        [SerializeField]
        protected KeyCodeAxis leanAxis = new KeyCodeAxis(KeyCode.E, KeyCode.Q);
        public KeyCodeAxis LeanAxis { get { return leanAxis; } }

        public override void Process()
        {
            base.Process();

            walkAxis.Process();
            strafeAxis.Process();
            Move = new Vector2(strafeAxis.RawValue, walkAxis.RawValue);

            Jump = Input.GetKey(jumpKey);
            Sprint = Input.GetKey(sprintKey);

            Crouch = Input.GetKeyDown(crouchKey);
            Prone = Input.GetKeyDown(pronekey);


            Look = lookAxis.Value;

            leanAxis.Process();
            Lean = leanAxis.RawValue;
        }
    }
}