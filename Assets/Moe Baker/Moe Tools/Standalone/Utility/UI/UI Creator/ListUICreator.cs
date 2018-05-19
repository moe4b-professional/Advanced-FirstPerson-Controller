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
	public class ListUICreator<TData, TTemplate> : UICreator<TData, TTemplate>
        where TTemplate : UITemplate<TData>
	{
        List<TTemplate> instances = new List<TTemplate>();
        public List<TTemplate> Instances { get { return instances; } }

        public virtual void Create(TData[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Instances.Add(Create(data[i]));

                EditInstance(i, Instances[i], data[i]);
            }
        }

        protected virtual void EditInstance(int index, TTemplate template, TData data)
        {

        }

        public virtual void DestroyInstances()
        {
            for (int i = 0; i < Instances.Count; i++)
                if (Instances[i] != null)
                    Object.Destroy(Instances[i]);

            Instances.Clear();
        }
	}
}