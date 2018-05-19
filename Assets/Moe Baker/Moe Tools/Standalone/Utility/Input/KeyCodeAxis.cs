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
    public partial class KeyCodeAxis
    {
        [SerializeField]
        protected KeyCode positive;
        public KeyCode Positive { get { return positive; } }

        [SerializeField]
        protected KeyCode negative;
        public KeyCode Negative { get { return negative; } }

        public int RawValue { get; protected set; }

        public void Process()
        {
            if (Input.GetKey(positive))
                RawValue = 1;
            else if (Input.GetKey(negative))
                RawValue = -1;
            else
                RawValue = 0;
        }

        public KeyCodeAxis(KeyCode positive, KeyCode negative)
        {
            this.positive = positive;
            this.negative = negative;
        }
    }
}