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
    public abstract class BoolController
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected Transform parent;
        public Transform Parent { get { return parent; } }
    }

    [Serializable]
    public abstract class BoolController<TItem> : BoolController
        where TItem : MonoBehaviour, IBoolItem<TItem>
    {
        [SerializeField]
        protected List<TItem> list;
        public List<TItem> List { get { return list; } }

        public virtual TItem Get()
        {
            return Get(Vector3.zero, Quaternion.identity);
        }
        public virtual TItem Get(Vector3 position)
        {
            return Get(position, Quaternion.identity);
        }
        public virtual TItem Get(Vector3 position, Quaternion rotation)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Avalability.Value)
                {
                    list[i].transform.position = position;
                    list[i].transform.rotation = rotation;

                    List[i].Enable();

                    return List[i];
                }
            }

            var instance = Instantiate(position, rotation);

            List.Add(instance);

            return instance;
        }

        protected virtual TItem Instantiate(Vector3 position, Quaternion rotation)
        {
            TItem instance = GameObject.Instantiate(prefab, position, rotation).GetComponent<TItem>();

            instance.transform.SetParent(parent, true);

            EditInstance(ref instance);

            return instance;
        }
        protected virtual void EditInstance(ref TItem instance)
        {
            instance.Instantiated();
            instance.Enable();
        }
    }
}