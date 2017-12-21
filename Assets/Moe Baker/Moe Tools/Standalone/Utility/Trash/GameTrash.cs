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
    public class GameTrash : MonoBehaviour
    {
        static GameTrash current;
        public static GameTrash Current
        {
            get
            {
                if (current == null)
                    CreateCurrent();

                return current;
            }
        }

        static void CreateCurrent()
        {
            GameObject gObject = new GameObject("Game Trash");

            current = gObject.AddComponent<GameTrash>();

            current.list = new List<TrashItem>();
        }

        [SerializeField]
        List<TrashItem> list;
        public List<TrashItem> List { get { return list; } }

        public static TrashItem Add(GameObject gameObject, float delay)
        {
            return Add(gameObject, delay, true);
        }
        public static TrashItem Add(GameObject gameObject, float delay, bool reParent)
        {
            if (reParent)
                gameObject.transform.SetParent(Current.transform, true);

            var item = new TrashItem(gameObject, delay);
            Current.list.Add(item);

            return item;
        }

        protected virtual void Update()
        {
            for (int i = list.Count; i-- > 0;)
            {
                list[i].Time -= Time.deltaTime;

                if (list[i].Time <= 0f || list[i].GameObject == null)
                    Remove(i);
            }
        }

        public static void Remove(TrashItem item)
        {
            if (item.GameObject)
                Destroy(item.GameObject);

            Current.list.Remove(item);
        }
        public static void Remove(int index)
        {
            if (Current.list[index].GameObject)
                Destroy(Current.list[index].GameObject);

            Current.list.RemoveAt(index);
        }
    }
}