#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Moe.Tools
{
    public partial class MultiTag
    {
        [CustomEditor(typeof(MultiTag))]
        public class Inspector : InspectorBase<MultiTag>
        {
            InspectorList tags;

            protected override void OnEnable()
            {
                base.OnEnable();

                tags = new InspectorList(serializedObject.FindProperty("tags"));
            }

            public override void OnInspectorGUI()
            {
                EditorGUILayout.Space();
                tags.Draw();
                EditorGUILayout.Space();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif