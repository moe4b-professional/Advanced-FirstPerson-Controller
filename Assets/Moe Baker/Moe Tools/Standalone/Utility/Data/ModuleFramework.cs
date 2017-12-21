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
    //Basic
    [Serializable]
    public abstract class MoeModule
    {

    }

    [Serializable]
    public abstract class MoeModuleManager<TModule>
        where TModule : MoeModule
    {
        [SerializeField]
        List<TModule> list;
        public List<TModule> List { get { return list; } }

        public virtual void Add(TModule module)
        {
            list.Add(module);
        }

        public virtual void ForAll(Action<TModule> action)
        {
            list.ForEach(action);
        }

        public MoeModuleManager()
        {
            list = new List<TModule>();
        }
    }


    //Linked
    [Serializable]
    public abstract class MoeLinkedModule<TLink> : MoeModule
    {
        public TLink Link { get; protected set; }
        public virtual void SetLink(TLink link)
        {
            this.Link = link;
        }
    }

    [Serializable]
    public abstract class MoeLinkedModuleManager<TModule, TLink> : MoeModuleManager<TModule>
        where TModule : MoeLinkedModule<TLink>
    {
        public TLink Link { get; protected set; }

        public virtual void SetLinks(TLink link)
        {
            this.Link = link;

            ForAll(SetLink);
        }
        protected virtual void SetLink(MoeLinkedModule<TLink> module)
        {
            module.SetLink(Link);
        }
    }
}