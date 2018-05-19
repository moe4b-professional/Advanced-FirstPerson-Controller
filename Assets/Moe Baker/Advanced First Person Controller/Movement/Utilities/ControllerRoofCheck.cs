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
	public abstract partial class ControllerRoofCheckBase : ControllerCast
	{
        public override Vector3 Direction { get { return Vector3.up; } }

        public override Vector3 Start { get { return Controller.transform.position + Vector3.up * collider.height; } }
    }

    public partial class ControllerRoofCheck : ControllerRoofCheckBase
    {
        
    }
}