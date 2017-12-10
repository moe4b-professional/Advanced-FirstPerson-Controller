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
    [Serializable]
    public class ListEnumerator<T>
    {
        public IList<T> List { get; protected set; }

        public int Index { get; protected set; }
        public T Current
        {
            get
            {
                if (List.IsInRange(Index))
                    return List[Index];

                throw new ArgumentOutOfRangeException("Index", "Enumeration Complete Please Reset");
            }
        }

        public virtual void Reset()
        {
            Index = 0;
        }

        public virtual bool Previous()
        {
            if (!List.IsInRange(Index - 1))
                return false;

            Index--;
            return true;
        }
        public virtual bool Next()
        {
            if (!List.IsInRange(Index + 1))
                return false;

            Index++;
            return true;
        }

        public virtual void Random()
        {
            int randomIndex = List.GetRandomIndex();

            if (randomIndex == Index)
            {
                Index = randomIndex;

                if (!Next())
                    Previous();
            }
            else
            {
                Index = randomIndex;
            }
        }

        public ListEnumerator(IList<T> list) : this(list, 0)
        {

        }
        public ListEnumerator(IList<T> list, int index)
        {
            this.List = list;

            index = MoeTools.Math.ClampRewind(index, 0, list.Count - 1);
        }
    }
}