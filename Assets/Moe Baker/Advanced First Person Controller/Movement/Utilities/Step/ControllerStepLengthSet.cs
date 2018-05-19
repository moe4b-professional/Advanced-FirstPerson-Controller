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
    [CreateAssetMenu(menuName = FPController.MenuPath + "Step Length Set")]
	public class ControllerStepLengthSet : ControllerState.DataSet<float>
	{
        protected virtual void Reset()
        {
            walk = 1.5f;
            crouch = 0.65f;
            prone = 0.35f;
            sprint = 1.8f;
        }
    }
}