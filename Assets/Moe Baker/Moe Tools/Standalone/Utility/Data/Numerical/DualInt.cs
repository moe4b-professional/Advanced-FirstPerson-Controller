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
    [Serializable]
    public struct DualInt : IIntRange
    {
        [SerializeField]
        public int x;
        int IRange<int>.Min { get { return x; } }

        [SerializeField]
        public int y;
        int IRange<int>.Max { get { return y; } }

        Vector2 vector2;
        public Vector2 Vector2
        {
            get
            {
                vector2.x = x;
                vector2.y = y;

                return vector2;
            }
        }

        public DualInt(int x, int y)
        {
            this.x = x;
            this.y = y;

            vector2 = new Vector2(x, y);
        }
    }
}