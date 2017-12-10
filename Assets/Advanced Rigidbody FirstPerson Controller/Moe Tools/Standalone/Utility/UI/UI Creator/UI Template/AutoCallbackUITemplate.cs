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
    public interface IAutoCallbackUITemplate<TData, TAccessor>
    {

    }

    public abstract class AutoCallbackUITemplate<TData, TAccessor> : UITemplate<TData>, IAutoCallbackUITemplate<TData, TAccessor>
    {
        public abstract void SetCallbacks(TAccessor accessor);
    }
}