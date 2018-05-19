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
        public bool IsRunning
        {
            get
            {
                return Coroutine != null || FirstRunCheck;
            }
        }
        public virtual bool CheckIfRunning()
        {
            return IsRunning;
        }

        public void Start()
        {
            if (IsRunning)
                Stop();

            FirstRunCheck = true;
            Coroutine = Behaviour.StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            InternalCoroutine = Behaviour.StartCoroutine(Function());

            yield return InternalCoroutine;

            Stop();
        }

        public void Stop()
        {
            if (InternalCoroutine != null)
                Behaviour.StopCoroutine(InternalCoroutine);

            if (Coroutine != null)
                Behaviour.StopCoroutine(Coroutine);

            FirstRunCheck = false;

            InternalCoroutine = null;
            Coroutine = null;
        }

        public AutoCoroutine(MonoBehaviour behaviour, Func<IEnumerator> function)
        {
            this.Behaviour = behaviour;
            this.Function = function;
        }
    }
}