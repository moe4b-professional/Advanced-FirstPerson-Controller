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
	public class AnimatorEventRewind : MonoBehaviour
	{
        Dictionary<string, Action> Dispatcher;

        protected virtual void Awake()
        {
            Dispatcher = new Dictionary<string, Action>();
        }

        public event Action<string> OnEvent;
        protected virtual void Event(string ID)
        {
            Invoke(ID);

            if (OnEvent != null)
                OnEvent(ID);
        }

        protected virtual void Invoke(string ID)
        {
            if (!Dispatcher.ContainsKey(ID))
                return;
            if (Dispatcher[ID] == null)
                return;

            Dispatcher[ID].Invoke();
        }

        public virtual void AddHandler(string ID, Action action)
        {
            if(Dispatcher.ContainsKey(ID))
                Dispatcher[ID] += action;
            else
                Dispatcher.Add(ID, action);
        }

        public virtual void RemoveHandler(string ID, Action action)
        {
            if (Dispatcher.ContainsKey(ID))
                Dispatcher[ID] -= action;
        }

        public virtual void ClearHandler(string ID)
        {
            if (Dispatcher.ContainsKey(ID))
                Dispatcher[ID] = null;
        }

        public virtual void ClearHandlers(string ID)
        {
            Dispatcher.Clear();
        }
    }
}