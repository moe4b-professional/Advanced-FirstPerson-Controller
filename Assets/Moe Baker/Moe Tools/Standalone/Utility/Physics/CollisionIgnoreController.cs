using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace Moe.Tools
{
    public partial class CollisionIgnoreController : MonoBehaviour
    {
        [SerializeField]
        SetData[] sets;

        void Awake()
        {
            sets.ForEach(ApplySet);
        }

        protected virtual void ApplySet(SetData set)
        {
            if (set.Obj1 == null)
                throw new ArgumentNullException("Obj1 Must Have A Value");

            if (set.Obj2 == null)
                throw new ArgumentNullException("Obj2 Must Have A Value");

            set.Apply();
        }

        [Serializable]
        public class SetData
        {
            [SerializeField]
            GameObject obj1;
            public GameObject Obj1 { get { return obj1; } }

            [SerializeField]
            GameObject obj2;
            public GameObject Obj2 { get { return obj2; } }

            [SerializeField]
            bool enabled = true;
            public bool Enabled { get { return enabled; } }

            public void Apply()
            {
                MoeTools.GameObject.SetCollision(obj1, obj2, enabled);
            }
        }
    }
}