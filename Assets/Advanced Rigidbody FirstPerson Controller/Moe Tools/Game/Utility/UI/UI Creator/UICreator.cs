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
    [Serializable]
    public abstract class BaseUICreator
    {

    }

    [Serializable]
    public abstract class BaseUICreator<TInstance> : BaseUICreator
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } set { prefab = value; } }

        [SerializeField]
        protected RectTransform menu;
        public RectTransform Menu { get { return menu; } set { menu = value; } }

        protected virtual TInstance Create(Func<GameObject, TInstance> instanceConverter)
        {
            TInstance instance = instanceConverter(Object.Instantiate(prefab, menu, false));

            EditInstance(ref instance);

            return instance;
        }

        protected virtual void EditInstance(ref TInstance instance)
        {

        }
    }

    [Serializable]
    public abstract class UICreator<TInstance> : BaseUICreator<TInstance>
    {
        new public virtual TInstance Create(Func<GameObject, TInstance> instanceConverter)
        {
            return base.Create(instanceConverter);
        }
    }
}