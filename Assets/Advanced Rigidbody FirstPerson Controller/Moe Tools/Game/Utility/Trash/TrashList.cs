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
    public class TrashList
    {
        List<TrashItem> list;
        public List<TrashItem> List { get { return list; } }

        public TrashItem Add(GameObject gameObject, float delay)
        {
            var item = new TrashItem(gameObject, delay);

            list.Add(item);

            return item;
        }

        public void Update()
        {
            for (int i = list.Count; i-- > 0;)
            {
                list[i].Time -= Time.deltaTime;

                if (list[i].Time <= 0f || list[i].GameObject == null)
                    Remove(i);
            }
        }

        public void Remove(TrashItem item)
        {
            if (item == null)
                return;

            item.Destroy();

            list.Remove(item);
        }
        public void Remove(int index)
        {
            list[index].Destroy();

            list.RemoveAt(index);
        }

        public void Clear()
        {
            for (int i = list.Count; i-- > 0;)
                Remove(i);
        }

        public TrashList()
        {
            list = new List<TrashItem>();
        }
    }
}