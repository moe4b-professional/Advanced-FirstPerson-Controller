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
    public class AutoPoolController : PoolController<AutoPoolItem>
    {
        protected override void EditInstance(ref AutoPoolItem instance)
        {
            ((IPoolItem<AutoPoolItem>)instance).Instantiated();
        }
    }
}