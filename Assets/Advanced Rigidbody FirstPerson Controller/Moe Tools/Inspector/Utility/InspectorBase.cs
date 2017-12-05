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
    public class InspectorBase<InspectedType> : Editor where InspectedType : Object
    {
        new protected InspectedType target;
        new protected InspectedType[] targets;

        protected virtual void OnEnable()
        {
            target = (InspectedType)base.target;

            Selection.selectionChanged += UpdateTargets;

            UpdateTargets();
        }

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
    }
}
#endif