#if UNITY_EDITOR
using System;
using System.IO;
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
    public class MoeInspector<InspectedType> : Editor
        where InspectedType : Object
    {
        new public InspectedType target;
        new public InspectedType[] targets;
        protected virtual void UpdateTargets()
        {
            targets = new InspectedType[base.targets.Length];
            for (int i = 0; i < base.targets.Length; i++)
                targets[i] = (InspectedType)base.targets[i];
        }
        protected virtual void ForAllTargets(Action<InspectedType> action)
        {
            for (int i = 0; i < targets.Length; i++)
                action(targets[i]);
        }

        public InspectorCustomGUI CustomGUI;
        public virtual InspectorCustomGUI.ScriptUIType ScriptUI
        {
            get
            {
                return CustomGUI.ScriptUI;
            }
            set
            {
                CustomGUI.ScriptUI = value;
            }
        }

        protected virtual void OnEnable()
        {
            target = (InspectedType)base.target;

            Selection.selectionChanged += UpdateTargets;
            UpdateTargets();

            CustomGUI = new InspectorCustomGUI(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            CustomGUI.Draw();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif