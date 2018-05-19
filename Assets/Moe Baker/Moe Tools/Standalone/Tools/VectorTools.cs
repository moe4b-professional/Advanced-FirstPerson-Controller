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

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Vector
        {
            public static Vector3 Divide(Vector3 a, Vector3 b)
            {
                a.x /= b.x;
                a.y /= b.y;
                a.z /= b.z;

                return a;
            }

            public static Vector3 MoveTowardsAngle(Vector3 current, Vector3 target, float speed)
            {
                return new Vector3()
                {
                    x = Mathf.MoveTowardsAngle(current.x, target.x, speed),
                    y = Mathf.MoveTowardsAngle(current.y, target.y, speed),
                    z = Mathf.MoveTowardsAngle(current.z, target.z, speed),
                };
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static Vector3 MoveTowardsAngle(this Vector3 current, Vector3 target, float speed)
        {
            return MoeTools.Vector.MoveTowardsAngle(current, target, speed);
        }
    }
}