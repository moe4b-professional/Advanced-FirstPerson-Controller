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
    [RequireComponent(typeof(Text))]
	public class InputLabel : MonoBehaviour
	{
        public FPController Controller { get; protected set; }

        public FPControllerKeyInput InputModule { get; protected set; }

        public Text Label { get; protected set; }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            Controller = FindObjectOfType<FPController>();
            Debug.Assert(Controller);

            InputModule = Controller.InputModule as FPControllerKeyInput;
            if (InputModule == null)
                throw new ArgumentException(GetType().Name + " Only Works If The Character Has An FPControllerKeyInput Module, But A Module Of Type " + Controller.InputModule.GetType().Name + " Was Detected Instead");

            Label = GetComponent<Text>();
            Debug.Assert(Label);

            SetText();
        }

        void SetText()
        {
            Label.text = FormatFunctionality(GetMovementText(), "Move") + " || " +
                "Move Mouse To Look Around" + '\n' +
                FormatFunctionality(InputModule.LeanAxis, "Lean Right & Left") + " || " +
                GetSprintText() + '\n' +
                FormatFunctionality(InputModule.JumpKey, "Jump") + " || " +
                FormatFunctionality(InputModule.CrouchKey, "Crouch") + '\n' +
                FormatFunctionality(InputModule.ProneKey, "Prone (Lie Down)") + " || " +
                "Crouch While Running To Slide";
        }

        string GetSprintText()
        {
            if (Controller.States.Sprint.Input == BaseFPController.ButtonInputMode.Hold)
                return "Hold " + InputModule.SprintKey.ToString() + " To Sprint";
            else
                return FormatFunctionality(InputModule.SprintKey, "Toggle Sprint");
        }

        string GetMovementText()
        {
            return GetKeyCodeAxisText(InputModule.WalkAxis) + ", " + GetKeyCodeAxisText(InputModule.StrafeAxis);
        }

        string FormatFunctionality(KeyCode keyCode, string functionality)
        {
            return FormatFunctionality(keyCode.ToString(), functionality);
        }
        string FormatFunctionality(RawkeyCodeAxis axis, string functionality)
        {
            return FormatFunctionality(GetKeyCodeAxisText(axis), functionality);
        }
        string FormatFunctionality(string inputName, string functionality)
        {
            return "Press " + inputName + " To " + functionality;
        }

        string GetKeyCodeAxisText(RawkeyCodeAxis axis)
        {
            return axis.Negative + ", " + axis.Positive;
        }
	}
}