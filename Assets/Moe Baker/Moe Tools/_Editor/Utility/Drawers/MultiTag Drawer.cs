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
        public class Inspector : MoeInspector<MultiTag>
        {
            InspectorList elements;

            protected override void OnEnable()
            {
                base.OnEnable();

                elements = new InspectorList(serializedObject.FindProperty("elements"));

                CustomGUI.Overrides.Add(elements.serializedProperty, DrawElements);
            }

            protected virtual void DrawElements()
            {
                elements.Draw();
            }
        }
    }
}
#endif