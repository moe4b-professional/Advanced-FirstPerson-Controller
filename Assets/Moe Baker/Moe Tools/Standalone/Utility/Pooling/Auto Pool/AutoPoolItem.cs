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
    public class AutoPoolItem : MonoBehaviour, IPoolItem<AutoPoolItem>
    {
        [SerializeField]
        BoolValueEvent avalibility;
        public BoolValueEvent Avalability { get { return avalibility; } }

        public AutoPoolItem Item { get { return this; } }

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
        protected virtual void Disable()
        {
            avalibility.Value = true;
        }

        protected virtual void OnDisable()
        {
            Disable();
        }
    }
}