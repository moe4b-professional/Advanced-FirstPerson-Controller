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
    public interface IPoolItem
    {
        BoolValueEvent Avalability { get; }

        void Instantiated();

        void Enable();

        void ReEnable();

        void Disable();
    }
    public interface IPoolItem<TItem> : IPoolItem
        where TItem : MonoBehaviour
    {
        TItem Item { get; }
    }

    public abstract class PoolItem : MonoBehaviour, IPoolItem
    {
        [SerializeField]
        BoolValueEvent avalibility;
        public BoolValueEvent Avalability { get { return avalibility; } }

        void IPoolItem.Instantiated()
        {
            Instantiated();
        }
        protected virtual void Instantiated()
        {

        }

        void IPoolItem.Enable()
        {
            Enable();
        }
        protected virtual void Enable()
        {
            gameObject.SetActive(true);

            avalibility.Value = false;
        }

        void IPoolItem.ReEnable()
        {
            ReEnable();
        }
        protected virtual void ReEnable()
        {

        }

        void IPoolItem.Disable()
        {
            Disable();
        }
        public virtual void Disable()
        {
            gameObject.SetActive(false);

            avalibility.Value = true;
        }
    }
    public abstract class PoolItem<TItem> : PoolItem, IPoolItem<TItem>
        where TItem : PoolItem<TItem>
    {
        public TItem Item { get { return this as TItem; } }
    }
}