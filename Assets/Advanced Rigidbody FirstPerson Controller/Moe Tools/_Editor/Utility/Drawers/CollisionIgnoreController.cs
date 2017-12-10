#if UNITY_EDITOR
using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditorInternal;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace Moe.Tools
{
    public partial class CollisionIgnoreController
    {
        [CustomEditor(typeof(CollisionIgnoreController))]
        public class Inspector : InspectorBase<CollisionIgnoreController>
        {
            InspectorList sets;

            protected override void OnEnable()
            {
                base.OnEnable();

                sets = new InspectorList(serializedObject.FindProperty("sets"));
                sets.elementHeight = 60f;
            }

            public override void OnInspectorGUI()
            {
                EditorGUILayout.Space();

                sets.Draw();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif