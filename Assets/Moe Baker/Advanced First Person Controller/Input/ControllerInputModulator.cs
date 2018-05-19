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
    [CreateAssetMenu(menuName = MenuPath + "Modulator")]
	public class ControllerInputModulator : InputModulator<ControllerInputModule>
	{
        public const string MenuPath = FPController.MenuPath + "Input/";
    }
}