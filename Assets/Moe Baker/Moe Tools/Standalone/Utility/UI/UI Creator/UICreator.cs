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
	public class UICreator<TData, TTemplate>
        where TTemplate : UITemplate<TData>
	{
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected RectTransform parent;
        public RectTransform Parent { get { return parent; } }

        public virtual TTemplate Create(TData data)
        {
            var instance = Object.Instantiate(prefab, parent, false).GetComponent<TTemplate>();

            instance.SetData(data);

            return instance;
        }
	}
}