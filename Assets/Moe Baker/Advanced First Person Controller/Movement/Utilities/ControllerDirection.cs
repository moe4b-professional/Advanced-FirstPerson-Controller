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
    public abstract partial class ControllerDirectionBase : FPController.Module
    {
        public Vector3 Forward { get; protected set; }
        public Vector3 Right { get; protected set; }

        public override void Init(FPController link)
        {
            base.Init(link);

            Forward = Right = Vector3.zero;
            Calculate();
        }

        public virtual void Calculate()
        {
            Forward = Controller.transform.forward;
            Right = Controller.transform.right;
        }
    }

    public partial class ControllerDirection : ControllerDirectionBase
    {
		
	}
}