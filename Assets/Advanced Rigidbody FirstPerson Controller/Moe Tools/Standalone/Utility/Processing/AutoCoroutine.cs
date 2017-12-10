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
    public class AutoCoroutine
    {
        public MonoBehaviour Behaviour { get; protected set; }
        public Coroutine Coroutine { get; protected set; }
        public Coroutine InternalCoroutine { get; protected set; }

        public Func<IEnumerator> Function { get; protected set; }

        bool FirstRunCheck;
        public bool Running
        {
            get
            {
                return Coroutine != null || FirstRunCheck;
            }
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    End();
                }
            }
        }

        public void Start()
        {
            if (Running)
                End();

            FirstRunCheck = true;
            Coroutine = Behaviour.StartCoroutine(Function());
        }

        IEnumerator Procedure()
        {
            InternalCoroutine = Behaviour.StartCoroutine(Function());

            yield return InternalCoroutine;

            End();
        }

        public void End()
        {
            if (Coroutine != null)
                Behaviour.StopCoroutine(Coroutine);
                
            if (InternalCoroutine != null)
                Behaviour.StopCoroutine(InternalCoroutine);

            FirstRunCheck = false;
            Coroutine = null;
        }

        public AutoCoroutine(MonoBehaviour behaviour, Func<IEnumerator> function)
        {
            this.Behaviour = behaviour;
            this.Function = function;
        }
    }
}