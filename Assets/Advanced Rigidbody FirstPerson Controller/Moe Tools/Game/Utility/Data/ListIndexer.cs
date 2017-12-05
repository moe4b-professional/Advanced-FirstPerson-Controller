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
    public class ListIndexer<TAccessor, TData>
    {
        public Dictionary<TAccessor, int> Dictionary { get; protected set; }

        public int this[TAccessor accessor]
        {
            get
            {
                if (!Contains(accessor))
                    throw new ArgumentException(accessor + " Not Definded In ListIndexer");

                return Dictionary[accessor];
            }
        }

        public bool Contains(TAccessor name)
        {
            return Dictionary.ContainsKey(name);
        }

        public void Add(TAccessor accessor, int value)
        {
            Dictionary.Add(accessor, value);
        }
        public void Remove(TAccessor accessor)
        {
            Dictionary.Remove(accessor);
        }

        public void Clear()
        {
            Dictionary.Clear();
        }

        public void Setup<T>(IList<T> list, Func<T, TAccessor> AccessorProvider)
        {
            for (int i = 0; i < list.Count; i++)
                Dictionary.Add(AccessorProvider(list[i]), i);
        }

        public ListIndexer()
        {
            Dictionary = new Dictionary<TAccessor, int>();
        }
        public ListIndexer(IList<TData> list, Func<TData, TAccessor> AccessorProvider) : this()
        {
            Setup(list, AccessorProvider);
        }
    }
}