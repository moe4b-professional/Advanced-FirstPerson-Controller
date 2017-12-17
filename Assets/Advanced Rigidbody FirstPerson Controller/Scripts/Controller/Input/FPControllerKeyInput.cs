using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace ARFC
{
    [CreateAssetMenu(menuName = FPControllerInputModule.MenuPath + "Key Module")]
    public class FPControllerKeyInput : FPControllerInputModule
    {
        [Header("Keys")]
        [SerializeField]
        RawkeyCodeAxis walkAxis = new RawkeyCodeAxis(KeyCode.W, KeyCode.S);
        public RawkeyCodeAxis WalkAxis { get { return walkAxis; } }

        [SerializeField]
        RawkeyCodeAxis strafeAxis = new RawkeyCodeAxis(KeyCode.D, KeyCode.A);
        public RawkeyCodeAxis StrafeAxis { get { return strafeAxis; } }

        [SerializeField]
        RawkeyCodeAxis leanAxis = new RawkeyCodeAxis(KeyCode.E, KeyCode.Q);
        public RawkeyCodeAxis LeanAxis { get { return leanAxis; } }

        [SerializeField]
        KeyCode jumpKey = KeyCode.Space;
        public KeyCode JumpKey { get { return jumpKey; } }

        [SerializeField]
        KeyCode sprintKey = KeyCode.LeftShift;
        public KeyCode SprintKey { get { return sprintKey; } }

        [SerializeField]
        KeyCode crouchKey = KeyCode.C;
        public KeyCode CrouchKey { get { return crouchKey; } }

        [SerializeField]
        KeyCode proneKey = KeyCode.LeftControl;
        public KeyCode ProneKey { get { return proneKey; } }

        public override void UpdateInput()
        {
            walkAxis.Update();
            strafeAxis.Update();

            Walk = walkAxis.Value;
            Strafe = strafeAxis.Value;

            leanAxis.Update();
            lean = leanAxis.Value;

            look.x = Input.GetAxis("Mouse X");
            look.y = Input.GetAxis("Mouse Y");

            jump = Input.GetKeyDown(jumpKey);
            sprint = Input.GetKey(sprintKey);

            crouch = Input.GetKeyDown(crouchKey);
            prone = Input.GetKeyDown(proneKey);
        }
    }

    [Serializable]
    public class RawkeyCodeAxis
    {
        [SerializeField]
        KeyCode positive;
        public KeyCode Positive { get { return positive; } }

        [SerializeField]
        KeyCode negative;
        public KeyCode Negative { get { return negative; } }

        [SerializeField]
        [Range(-1, 1)]
        int value;
        public int Value { get { return value; } }

        public void Update()
        {
            if (Input.GetKey(positive))
                value = 1;
            else if (Input.GetKey(negative))
                value = -1;
            else
                value = 0;
        }

        public RawkeyCodeAxis(KeyCode positive, KeyCode negative)
        {
            this.positive = positive;
            this.negative = negative;
        }
    }
}