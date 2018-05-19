using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Moe.Tools
{
    [DisallowMultipleComponent]
    public partial class MultiTag : MonoBehaviour
    {
        [SerializeField]
        List<TagElement> elements = new List<TagElement>();
        public List<TagElement> Elements { get { return elements; } }

        public int Length { get { return elements.Count; } }

        public virtual TagElement FindElement(string value)
        {
            for (int i = 0; i < elements.Count; i++)
                if (elements[i].Value == value) return elements[i];

            return null;
        }
        public virtual TagElement GetElement(int index)
        {
            if (!elements.IsInRange(index))
                throw new ArgumentOutOfRangeException("index", "Index " + index + " Out Of Tag Elements Range");

            return elements[index];
        }

        public bool Contains(string value)
        {
            for (int i = 0; i < elements.Count; i++)
                if (elements[i].Value == value) return true;

            return false;
        }
        public virtual bool Contains(TagElement element)
        {
            return elements.Contains(element);
        }

        public virtual bool Contains(IList<string> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (Contains(values[i]))
                    continue;
                else
                    return false;
            }

            return true;
        }
        public virtual bool Contains(IList<TagElement> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (Contains(elements[i]))
                    continue;
                else
                    return false;
            }

            return true;
        }

        public virtual void AddTag(TagElement element)
        {
            if (Contains(element))
            {
                Debug.LogError("Trying to add a duplicate Tag Element of " + element.name);

                return;
            }
            else
            {
                elements.Add(element);
            }
        }

        public void RemoveTag(string value)
        {
            var element = FindElement(value);

            if (elements == null)
                throw new ArgumentException("No Tag Element Of Value " + value + " Was Found In MultiTag " + name);

            RemoveTag(element);
        }
        public virtual void RemoveTag(TagElement element)
        {
            if(!elements.Contains(element))
            {
                Debug.LogWarning("Trying To Remove Non-Added Tag " + element.name);
                return;
            }

            elements.Remove(element);
        }

        protected virtual void OnEnable()
        {
            RegisterThis();
        }
        protected virtual void RegisterThis()
        {
            MultiTagTools.Register(this);
        }

        protected virtual void OnDisable()
        {
            UnRegisterThis();
        }
        protected virtual void UnRegisterThis()
        {
            MultiTagTools.UnRegister(this);
        }
    }

    public static partial class MultiTagTools
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnGameLoad()
        {
            All = new List<MultiTag>();
        }

        public static List<MultiTag> All { get; private set; }
        public static void Register(MultiTag tag)
        {
            if (All.Contains(tag))
            {
                Debug.LogError("Trying To Register Pre-Registerd MultiTag " + tag.name);
                return;
            }

            All.Add(tag);
        }
        public static void UnRegister(MultiTag tag)
        {
            if (!All.Contains(tag))
            {
                Debug.LogError("Trying To UnRegister Non-Registered MultiTag " + tag.name);
                return;
            }

            All.Remove(tag);
        }

        #region Get MultiTag
        public static MultiTag GetMultiTag(this Component target)
        {
            return GetMultiTag(target.gameObject);
        }
        public static MultiTag GetMultiTag(this Transform target)
        {
            return GetMultiTag(target.gameObject);
        }
        public static MultiTag GetMultiTag(this GameObject target)
        {
            return target.GetComponent<MultiTag>();
        }
        #endregion

        #region Get MultiTags

        #region Value
        public static string[] GetMultiTagsValues(this Component target)
        {
            return GetMultiTagsValues(target.gameObject);
        }
        public static string[] GetMultiTagsValues(this Transform target)
        {
            return GetMultiTagsValues(target.gameObject);
        }
        public static string[] GetMultiTagsValues(this GameObject target)
        {
            var elements = GetMultiTags(target);

            if (elements == null)
                return null;

            return MoeTools.List.GetArrayOf(elements, GetTagValue);
        }
        public static string GetTagValue(TagElement element)
        {
            if (element == null)
                return "NULL";

            return element.Value;
        }
        #endregion

        #region Element
        public static List<TagElement> GetMultiTags(this Component target)
        {
            return GetMultiTags(target.gameObject);
        }
        public static List<TagElement> GetMultiTags(this Transform target)
        {
            return GetMultiTags(target.gameObject);
        }
        public static List<TagElement> GetMultiTags(this GameObject target)
        {
            var multiTag = GetMultiTag(target);

            if (multiTag == null)
                return null;

            return multiTag.Elements;
        }
        #endregion

        #endregion

        #region Compare

        #region Single

        #region Value
        public static bool CompareMultiTag(this Component target, string value)
        {
            return CompareMultiTag(target.gameObject, value);
        }
        public static bool CompareMultiTag(this Transform target, string value)
        {
            return CompareMultiTag(target.gameObject, value);
        }
        public static bool CompareMultiTag(this GameObject target, string value)
        {
            var multiTag = GetMultiTag(target);

            if (multiTag == null)
                return false;

            return multiTag.Contains(value);
        }
        #endregion

        #region Element
        public static bool CompareMultiTag(this Component target, TagElement element)
        {
            return CompareMultiTag(target.gameObject, element);
        }
        public static bool CompareMultiTag(this Transform target, TagElement element)
        {
            return CompareMultiTag(target.gameObject, element);
        }
        public static bool CompareMultiTag(this GameObject target, TagElement element)
        {
            var multiTag = GetMultiTag(target);

            if (multiTag == null)
                return false;

            return multiTag.Contains(element);
        }
        #endregion

        #endregion

        #region list

        #region Value
        public static bool CompareMultiTag(this Component target, IList<string> values)
        {
            return CompareMultiTag(target.gameObject, values);
        }
        public static bool CompareMultiTag(this Transform target, IList<string> values)
        {
            return CompareMultiTag(target.gameObject, values);
        }
        public static bool CompareMultiTag(this GameObject target, IList<string> values)
        {
            var multiTag = GetMultiTag(target);

            if (multiTag == null)
                return false;

            return multiTag.Contains(values);
        }
        #endregion

        #region Element
        public static bool CompareMultiTag(this Component target, IList<TagElement> elements)
        {
            return CompareMultiTag(target.gameObject, elements);
        }
        public static bool CompareMultiTag(this Transform target, IList<TagElement> elements)
        {
            return CompareMultiTag(target.gameObject, elements);
        }
        public static bool CompareMultiTag(this GameObject target, IList<TagElement> elements)
        {
            var multiTag = GetMultiTag(target);

            if (multiTag == null)
                return false;

            return multiTag.Contains(elements);
        }
        #endregion

        #endregion

        #endregion

        #region Find

        #region Object
        //Single
        public static GameObject FindObject(string value)
        {
            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(value))
                    return All[i].gameObject;

            return null;
        }
        public static GameObject FindObject(TagElement element)
        {
            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(element))
                    return All[i].gameObject;

            return null;
        }

        //List
        public static GameObject FindObject(IList<string> values)
        {
            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(values))
                    return All[i].gameObject;

            return null;
        }
        public static GameObject FindObject(IList<TagElement> elements)
        {
            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(elements))
                    return All[i].gameObject;

            return null;
        }
        #endregion

        #region Objects
        //Single
        public static GameObject[] FindObjects(string value)
        {
            var list = new List<GameObject>();

            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(value))
                    list.Add(All[i].gameObject);

            return list.ToArray();
        }
        public static GameObject[] FindObjects(TagElement element)
        {
            var list = new List<GameObject>();

            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(element))
                    list.Add(All[i].gameObject);

            return list.ToArray();
        }

        //List
        public static GameObject[] FindObjects(IList<string> values)
        {
            var list = new List<GameObject>();

            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(values))
                    list.Add(All[i].gameObject);

            return list.ToArray();
        }
        public static GameObject[] FindObjects(IList<TagElement> elements)
        {
            var list = new List<GameObject>();

            for (int i = 0; i < All.Count; i++)
                if (All[i].Contains(elements))
                    list.Add(All[i].gameObject);

            return list.ToArray();
        }
        #endregion

        #endregion
    }
}