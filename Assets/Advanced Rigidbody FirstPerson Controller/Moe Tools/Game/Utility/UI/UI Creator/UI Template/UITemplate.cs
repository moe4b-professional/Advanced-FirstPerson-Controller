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
    public interface IUITemplate
    {

    }

    public interface IUITemplate<TData>
    {
        void SetData(TData data);
    }

    public abstract class UITemplate : MonoBehaviour, IUITemplate
    {

    }

    public abstract class UITemplate<TData> : UITemplate, IUITemplate<TData>
    {
        public abstract void SetData(TData data);
    }
}