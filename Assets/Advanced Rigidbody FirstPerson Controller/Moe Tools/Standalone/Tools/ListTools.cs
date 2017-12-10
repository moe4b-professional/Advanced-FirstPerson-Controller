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
using URandom = UnityEngine.Random;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class List
        {
            public static void ForEach<T>(IList<T> list, Action<T> command)
            {
                for (int i = 0; i < list.Count; i++)
                    command(list[i]);
            }
            public static void ForEach<T>(IList<T> list, Action<T, int> command)
            {
                for (int i = 0; i < list.Count; i++)
                    command(list[i], i);
            }
            public static void Untill<T>(IList<T> list, Func<T, bool> checkCondition)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (checkCondition(list[i]))
                        return;
                }
            }

            public static T FindMember<T>(IList<T> list, Func<T, bool> checkCondition)
                where T : class
            {
                T value;

                FindMember(list, checkCondition, out value);

                return value;
            }
            public static bool FindMember<T>(IList<T> list, Func<T, bool> checkCondition, out T resault)
            {
                int index;

                if (FindMemberIndex(list, checkCondition, out index))
                {
                    resault = list[index];
                    return true;
                }
                else
                {
                    resault = default(T);
                    return false;
                }
            }
            public static bool FindMemberIndex<T>(IList<T> list, Func<T, bool> checkCondition, out int index)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (checkCondition(list[i]))
                    {
                        index = i;
                        return true;
                    }
                }

                index = 0;
                return false;
            }

            public static bool IsInRange<T>(IList<T> list, int index)
            {
                return index >= 0 && index < list.Count;
            }

            public static bool IsNullOrEmpty<T>(IList<T> list)
            {
                if (list == null)
                    return false;

                return list.Count == 0;
            }

            public static T GetRandom<T>(IList<T> list)
            {
                return list[GetRandomIndex(list)];
            }
            public static int GetRandomIndex<T>(IList<T> list)
            {
                if (list == null || list.Count == 0f)
                    throw new ArgumentNullException();

                if (list.Count == 1)
                    return 0;

                return URandom.Range(0, list.Count);
            }
            public static int[] GetRandomIndexArray<T>(IList<T> list)
            {
                List<int> resault = new List<int>();

                int index;
                for (int i = 0; i < list.Count; i++)
                {
                    do
                    {
                        index = GetRandomIndex(list);
                    } while (resault.Contains(index));

                    resault.Add(index);
                }

                return resault.ToArray();
            }

            public static TResault[] GetArrayOf<TSource, TResault>(IList<TSource> list, Func<TSource, TResault> getFunction)
            {
                TResault[] array = new TResault[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    array[i] = getFunction(list[i]);
                }

                return array;
            }

            public static bool ContainsNulls(IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == null)
                        return true;
                }

                return false;
            }
            public static void RemoveNulls(IList list)
            {
                for (int i = list.Count; i-- > 0;)
                {
                    if (list[i] == null)
                        list.RemoveAt(i);
                }
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static void ForEach<T>(this IList<T> list, Action<T> command)
        {
            MoeTools.List.ForEach(list, command);
        }
        public static void ForEach<T>(this IList<T> list, Action<T, int> command)
        {
            MoeTools.List.ForEach(list, command);
        }
        public static void Untill<T>(this IList<T> list, Func<T, bool> checkCondition)
        {
            MoeTools.List.Untill(list, checkCondition);
        }

        public static T FindMember<T>(this IList<T> list, Func<T, bool> checkCondition)
                where T : class
        {
            return MoeTools.List.FindMember(list, checkCondition);
        }
        public static bool FindMember<T>(this IList<T> list, Func<T, bool> checkCondition, out T resault)
        {
            return FindMember(list, checkCondition, out resault);
        }
        public static bool FindMemberIndex<T>(this IList<T> list, Func<T, bool> checkCondition, out int index)
        {
            return FindMemberIndex(list, checkCondition, out index);
        }

        public static bool IsInRange<T>(this IList<T> list, int index)
        {
            return MoeTools.List.IsInRange(list, index);
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return MoeTools.List.IsNullOrEmpty(list);
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            return MoeTools.List.GetRandom(list);
        }
        public static int GetRandomIndex<T>(this IList<T> list)
        {
            return MoeTools.List.GetRandomIndex(list);
        }

        public static TResault[] GetArrayOf<TSource, TResault>(this IList<TSource> list, Func<TSource, TResault> getFunction)
        {
            return MoeTools.List.GetArrayOf(list, getFunction);
        }

        public static bool ContainsNulls(this IList list)
        {
            return MoeTools.List.ContainsNulls(list);
        }
        public static void RemoveNulls(this IList list)
        {
            MoeTools.List.RemoveNulls(list);
        }

        public static bool IsSubsetOf<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return !a.Except(b).Any();
        }
    }
}