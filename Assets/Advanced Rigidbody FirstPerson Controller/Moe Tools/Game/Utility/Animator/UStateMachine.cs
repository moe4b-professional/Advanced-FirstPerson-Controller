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
    public class UStateMachine : StateMachineBehaviour
    {
        [SerializeField]
        string _name;
        new public string name { get { return _name; } }

        public static T GetBehaviour<T>(Animator anim, string stateName)
            where T : UStateMachine
        {
            return GetBehaviour<T>(anim, stateName, anim.GetBehaviours<T>());
        }
        public static T GetBehaviour<T>(Animator anim, string stateName, UStateMachine[] behaviours)
            where T : UStateMachine
        {
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i]._name == stateName)
                    return (T)behaviours[i];
            }

            return null;
        }
    }

    public static class UStateMachineExtensions
    {
        public static T GetBehaviour<T>(this Animator anim, string stateName)
            where T : UStateMachine
        {
            return UStateMachine.GetBehaviour<T>(anim, stateName);
        }

        public static T GetBehaviour<T>(this Animator anim, string stateName, UStateMachine[] behaviours)
            where T : UStateMachine
        {
            return UStateMachine.GetBehaviour<T>(anim, stateName, behaviours);
        }
    }
}