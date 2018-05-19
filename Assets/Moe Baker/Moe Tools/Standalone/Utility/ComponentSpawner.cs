using System;
using System.IO;
using System.Linq;
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
	public class ComponentSpawner<TComponent>
        where TComponent : Component
	{
		[SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        public TComponent Instance { get; protected set; }

        public virtual bool UseLocalComponent { get { return true; } }

        public virtual TComponent Spawn()
        {
            if (prefab == null)
                throw new NullReferenceException(GetType().Name + "'s Prefab Is Null");

            bool instantiated = false;

            if(UseLocalComponent)
                Instance = Object.FindObjectOfType<TComponent>();

            if(Instance == null || !UseLocalComponent)
            {
                instantiated = true;

                Instance = GameObject.Instantiate(prefab).GetComponent<TComponent>();

                if (Instance == null)
                    throw new Exception("Instantiated Prefab " + Instance.gameObject.name + " Doesn't Have A " + typeof(TComponent).Name + " Attached");
            }

            EditInstance(Instance, instantiated);

            return Instance;
        }

        protected virtual void EditInstance(TComponent instance, bool instantiated)
        {
            if (instantiated)
                Instance.gameObject.name = prefab.name;
        }
    }
}