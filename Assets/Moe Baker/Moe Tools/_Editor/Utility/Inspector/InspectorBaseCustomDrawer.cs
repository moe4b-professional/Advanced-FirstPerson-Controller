#if UNITY_EDITOR
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
    public class InspectorBaseCustomDrawer<TInspectedType> : InspectorBase<TInspectedType>
        where TInspectedType : Object
    {
        public InspectorCustomGUI gui;

        public virtual bool DrawScriptsField { get { return true; } }

        protected override void OnEnable()
        {
            base.OnEnable();

            gui = new InspectorCustomGUI(serializedObject, DrawScriptsField);
        }

        public override void OnInspectorGUI()
        {
            gui.Draw();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif