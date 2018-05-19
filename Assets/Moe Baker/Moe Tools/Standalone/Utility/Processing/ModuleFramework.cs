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
    public abstract class MoeModule<TLink> : MonoBehaviour
    {
        public TLink Link { get; protected set; }

        public virtual void Init(TLink link)
        {
            this.Link = link;
        }
    }

    [Serializable]
    public abstract class MoeModulesManager<TModule, TLink>
        where TModule : MoeModule<TLink>
    {
        List<TModule> list = new List<TModule>();
        public List<TModule> List { get { return list; } }
        public virtual void ForAll(Action<TModule> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    throw new NullReferenceException("Module At Index " + i + " Inside A " + GetType().Name + " Is Null");

                action(list[i]);
            }
        }


        public TLink Link { get; protected set; }

        public virtual void Init(TLink link)
        {
            this.Link = link;

            ForAll(InitModule);
        }
        protected virtual void InitModule(TModule module)
        {
            module.Init(Link);
        }


        public virtual void Add(TModule module)
        {
            if (list.Contains(module))
                throw new ArgumentException("Trying to add module of type " + module.GetType().Name + " But It Was Already Added To The Modules List");

            list.Add(module);
        }
        public virtual void Add<T>(IList<T> modules)
            where T : TModule
        {
            if (modules == null) return;
            if (modules.Count == 0) return;

            for (int i = 0; i < modules.Count; i++)
                Add(modules[i]);
        }

        public virtual void AddAll(GameObject gameObject)
        {
            AddAll<TModule>(gameObject);
        }
        public virtual void AddAll<TType>(GameObject gameObject)
            where TType : TModule
        {
            AddRecursive<TType>(gameObject.transform);
        }
        protected virtual void AddRecursive<TType>(Transform transform)
            where TType : TModule
        {
            Add(transform.gameObject.GetComponents<TType>());

            for (int i = 0; i < transform.childCount; i++)
                AddRecursive<TType>(transform.GetChild(i));
        }


        public virtual T Find<T>()
            where T : class
        {
            T component = null;

            for (int i = 0; i < list.Count; i++)
            {
                component = list[i].GetComponent<T>();

                if (component != null)
                    break;
            }

            return component;
        }
        public virtual List<T> FindAll<T>()
            where T : class
        {
            List<T> resault = new List<T>();

            for (int i = 0; i < list.Count; i++)
                resault.AddRange(list[i].GetComponents<T>());

            resault = resault.Distinct().ToList();

            return resault;
        }


        public MoeModulesManager()
        {
            list = new List<TModule>();
        }
    }
}