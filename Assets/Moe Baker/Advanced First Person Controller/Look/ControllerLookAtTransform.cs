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
	public class ControllerLookAtTransform : FPController.Module
	{
		[SerializeField]
        protected Transform target;
        public Transform Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        public ControllerLookTarget LookTarget { get { return Controller.Look.LookTarget; } }

        protected virtual void Update()
        {
            if (target == null)
                LookTarget.ClearPosition();
            else
                LookTarget.Position = target.position;
        }
    }
}