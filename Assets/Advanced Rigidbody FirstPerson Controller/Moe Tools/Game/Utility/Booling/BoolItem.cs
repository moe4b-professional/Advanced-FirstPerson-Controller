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
    public abstract class BoolItem : MonoBehaviour, IBoolItem
    {
        [SerializeField]
        BoolValueEvent avalibility;
        public BoolValueEvent Avalability { get { return avalibility; } }

        void IBoolItem.Instantiated()
        {
            Instantiated();
        }
        protected virtual void Instantiated()
        {

        }

        void IBoolItem.Enable()
        {
            Enable();
        }
        protected virtual void Enable()
        {
            gameObject.SetActive(true);

            avalibility.Value = false;
        }

        void IBoolItem.Disable()
        {
            Disable();
        }
        protected virtual void Disable()
        {
            gameObject.SetActive(false);

            avalibility.Value = true;
        }
    }

    public abstract class BoolItem<TItem> : BoolItem, IBoolItem<TItem>
        where TItem : BoolItem<TItem>
    {
        public TItem Item { get { return this as TItem; } }
    }
}