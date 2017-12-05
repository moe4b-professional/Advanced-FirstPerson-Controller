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
    public interface IBoolItem
    {
        BoolValueEvent Avalability { get; }

        void Instantiated();

        void Enable();

        void Disable();
    }

    public interface IBoolItem<TItem> : IBoolItem
        where TItem : MonoBehaviour
    {
        TItem Item { get; }
    }
}