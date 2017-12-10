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
    [Serializable]
    public class BaseTemplateListUICreator<TInstance, TData> : BaseTemplateUICreator<TInstance, TData>
    where TInstance : UITemplate<TData>
    {
        [SerializeField]
        protected List<TInstance> instances;
        public List<TInstance> Instances { get { return instances; } }

        protected virtual void Create(IList<TData> list)
        {
            instances = new List<TInstance>();

            for (int i = 0; i < list.Count; i++)
                CreateSingle(list[i]);
        }

        protected virtual TInstance CreateSingle(TData data)
        {
            instances.Add(base.Create(data));

            return instances.Last();
        }

        public virtual void Clear()
        {
            for (int i = 0; i < instances.Count; i++)
                GameObject.Destroy(instances[i].gameObject);

            instances = new List<TInstance>();
        }
    }

    [Serializable]
    public class TemplateListUICreator<TInstance, TData> : BaseTemplateListUICreator<TInstance, TData>
        where TInstance : UITemplate<TData>
    {
        new public virtual void Create(IList<TData> list)
        {
            base.Create(list);
        }
    }
}