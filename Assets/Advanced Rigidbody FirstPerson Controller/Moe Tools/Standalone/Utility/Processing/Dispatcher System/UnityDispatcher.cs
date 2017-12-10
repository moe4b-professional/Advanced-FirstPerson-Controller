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
    public abstract class BaseUnityDispatcher : MonoBehaviour
    {
        protected virtual void Configure()
        {
            ConfigureEvents();
        }
        protected virtual void ConfigureEvents()
        {
            UpdateEvent = new UpdateEventController();
            FixedUpdateEvent = new FixedUpdateEventController();
            LateUpdateEvent = new LateUpdateEventController();
            OnGUIEvent = new OnGUIEventController();
        }

        #region Events
        //Update
        public UpdateEventController UpdateEvent { get; protected set; }
        protected virtual void Update()
        {
            UpdateEvent.Invoke();
        }
        public class UpdateEventController : EventController<IUpdate>
        {
            protected override Action GetMethod(IUpdate obj)
            {
                return obj.Process;
            }
        }
        public interface IUpdate
        {
            void Process();
        }

        //Fixed Update
        public FixedUpdateEventController FixedUpdateEvent { get; protected set; }
        protected virtual void FixedUpdate()
        {
            FixedUpdateEvent.Invoke();
        }
        public class FixedUpdateEventController : EventController<IFixedUpdate>
        {
            protected override Action GetMethod(IFixedUpdate obj)
            {
                return obj.Process;
            }
        }
        public interface IFixedUpdate
        {
            void Process();
        }

        //Late Update
        public LateUpdateEventController LateUpdateEvent { get; protected set; }
        protected virtual void LateUpdate()
        {
            LateUpdateEvent.Invoke();
        }
        public class LateUpdateEventController : EventController<ILateUpdate>
        {
            protected override Action GetMethod(ILateUpdate obj)
            {
                return obj.Process;
            }
        }
        public interface ILateUpdate
        {
            void Process();
        }

        //On GUI
        public OnGUIEventController OnGUIEvent { get; protected set; }
        protected virtual void OnGUI()
        {
            OnGUIEvent.Invoke();
        }
        public class OnGUIEventController : EventController<IOnGUI>
        {
            protected override Action GetMethod(IOnGUI obj)
            {
                return obj.Process;
            }
        }
        public interface IOnGUI
        {
            void Process();
        }

        public abstract class EventController<TInterface>
        {
            public event Action Event;

            protected abstract Action GetMethod(TInterface obj);

            public virtual void Add(TInterface obj)
            {
                Add(GetMethod(obj));
            }
            public virtual void Add(Action action)
            {
                Event += action;
            }

            public virtual void Remove(TInterface obj)
            {
                Remove(GetMethod(obj));
            }
            public virtual void Remove(Action action)
            {
                Event -= action;
            }

            public virtual void Invoke()
            {
                if (Event != null)
                    Event();
            }

            public EventController()
            {

            }
        }
        #endregion

        #region Coroutine
        //Coroutine
        public Coroutine YieldForTime(Action method, float time)
        {
            return YieldForTime(method, time, false);
        }
        public Coroutine YieldForTime(Action method, float time, bool realTime)
        {
            if (realTime)
                return StartCoroutine(YieldProcedure(method, new WaitForSecondsRealtime(time)));
            else
                return StartCoroutine(YieldProcedure(method, new WaitForSeconds(time)));
        }

        public Coroutine YieldForFrame(Action method)
        {
            return StartCoroutine(YieldProcedure(method, new WaitForEndOfFrame()));
        }

        public IEnumerator YieldProcedure(Action method, YieldInstruction instruction)
        {
            yield return instruction;

            method();
        }
        public IEnumerator YieldProcedure(Action method, IEnumerator ienumerator)
        {
            yield return StartCoroutine(ienumerator);

            method();
        }
        public IEnumerator YieldProcedure(Action action, Func<IEnumerator> method)
        {
            yield return StartCoroutine(method());

            action();
        }
        #endregion
    }

    public class UnityDispatcher : BaseUnityDispatcher
    {
        public UnityDispatcher Current { get; protected set; }

        protected virtual void Awake()
        {
            Configure();   
        }
        protected override void Configure()
        {
            if (Current == null)
            {
                ConfigureSingelton();

                base.Configure();
            }
            else
            {
                if (this != Current)
                    Destroy(this.gameObject);
            }
        }
        protected virtual void ConfigureSingelton()
        {
            Current = this;

            DontDestroyOnLoad(this.gameObject);
        }
    }
}