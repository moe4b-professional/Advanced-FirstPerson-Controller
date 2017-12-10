#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public partial class BoundsCalculator
    {
        [CustomEditor(typeof(BoundsCalculator))]
        [CanEditMultipleObjects]
        public class Inspector : InspectorBase<BoundsCalculator>
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Calculate"))
                {
                    ForAllTargets((BoundsCalculator bc) => bc.Calculate());
                }
            }
        }
    }
}
#endif