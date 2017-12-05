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
    public class BaseTemplateUICreator<TInstance, TData> : BaseUICreator<TInstance>
    where TInstance : IUITemplate<TData>
    {
        protected virtual TInstance Create(TData data)
        {
            TInstance instance = base.Create(GetInstance);

            instance.SetData(data);

            return instance;
        }

        protected virtual TInstance GetInstance(GameObject gameObject)
        {
            return gameObject.GetComponent<TInstance>();
        }
    }

    public class TemplateUICreator<TInstance, TData> : BaseTemplateUICreator<TInstance, TData>
        where TInstance : UITemplate<TData>
    {
        new public virtual TInstance Create(TData data)
        {
            return base.Create(data);
        }
    }
}