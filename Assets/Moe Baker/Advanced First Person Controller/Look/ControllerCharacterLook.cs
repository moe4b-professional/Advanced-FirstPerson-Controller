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
	public abstract partial class ControllerCharacterLookBase : FPController.Module
	{
        public ControllerLook Look { get { return Controller.Look; } }

        public float Sensitivity { get { return Look.Sensitvity.Horizontal; } }

        public virtual void Process()
        {
            if(Look.Control)
                Controller.transform.localRotation *= Quaternion.Euler(0f, InputModule.Look.x * Sensitivity, 0f);
        }

        public virtual void LookAt(Vector3 position, float speed)
        {
            var direction = (position - Controller.transform.position).normalized;
            direction.y = 0f;

            Controller.transform.rotation = Quaternion.RotateTowards(Controller.transform.rotation, Quaternion.LookRotation(direction), speed * Time.deltaTime);
        }
	}

    public partial class ControllerCharacterLook : ControllerCharacterLookBase
    {

    }
}