using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

using Moe.Tools;

namespace ARFC
{
    [CreateAssetMenu(menuName = MenuPath + "Data")]
    public class HeadBobData : ScriptableObject
    {
        public const string MenuPath = FPController.MenuPath + "Headbob/";

        [SerializeField]
        float scale = 1f;
        public float Scale { get { return scale; } }

        [SerializeField]
        float delta = 2f;
        public float Delta { get { return delta; } }

        [SerializeField]
        float gravity = 4f;
        public float Gravity { get { return gravity; } }

        [SerializeField]
        AnimationCurve curve;
        public AnimationCurve Curve { get { return curve; } }

        public float Evaluate(float time)
        {
            return curve.Evaluate(time) * scale;
        }
    }
}