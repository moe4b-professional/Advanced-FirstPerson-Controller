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
        List<string> tags = new List<string>();
        public List<string> Tags { get { return tags; } }

        public int Length { get { return tags.Count; } }

        public string this[int index]
        {
            get
            {
                return tags[index];
            }
        }

        public bool Contains(string tag)
        {
            return tags.Contains(tag);
        }

        public void AddTag(string tag)
        {
            if (!tags.Contains(tag))
                tags.Add(tag);
            else
                Debug.LogError(tag + " Already Exists Within " + gameObject.name);
        }

        public void RemoveTag(string tag)
        {
            if (tags.Contains(tag))
                tags.Remove(tag);
            else
                Debug.LogError(gameObject.name + " Doesn't Define Tag " + tag);
        }
        public void RemoveTagAtIndex(int index)
        {
            tags.RemoveAt(index);
        }

        void OnEnable()
        {
            if (!AllTags.Contains(this))
                AllTags.Add(this);
        }

        void OnDisable()
        {
            AllTags.Remove(this);
        }

        public static List<MultiTag> AllTags { get; protected set; }

        public static GameObject FindObjectWithTag(string text)
        {
            MultiTag tag = AllTags.FindMember(delegate (MultiTag mTag) { return mTag.Contains(text); });

            if (tag)
                return tag.gameObject;

            return null;
        }
        public static GameObject[] FindObjectsWithTag(string tag)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            for (int i = 0; i < AllTags.Count; i++)
            {
                if (AllTags[i].Contains(tag))
                    gameObjects.Add(AllTags[i].gameObject);
            }

            return gameObjects.ToArray();
        }

        public static GameObject FindObjectWithTags(string[] tags)
        {
            for (int i = 0; i < AllTags.Count; i++)
            {
                if (tags.IsSubsetOf(AllTags[i].tags))
                    return AllTags[i].gameObject;
            }

            return null;
        }
        public static GameObject[] FindObjectsWithTags(string[] tags)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            for (int i = 0; i < AllTags.Count; i++)
            {
                if (tags.IsSubsetOf(AllTags[i].tags))
                    gameObjects.Add(AllTags[i].gameObject);
            }

            return gameObjects.ToArray();
        }
    }

    public static class MultiTagTools
    {
        public static MultiTag GetMultiTag(this GameObject gameObject)
        {
            return gameObject.GetComponent<MultiTag>();
        }
        public static MultiTag GetMultiTag(this Transform transform)
        {
            return transform.gameObject.GetMultiTag();
        }
        public static MultiTag GetMultiTag(this Component component)
        {
            return component.gameObject.GetMultiTag();
        }

        public static string[] GetTags(this GameObject gameObject)
        {
            var multiTag = gameObject.GetMultiTag();

            string[] tags = new string[multiTag == null ? 1 : multiTag.Tags.Count + 1];

            tags[0] = gameObject.tag;

            for (int i = 1; i < tags.Length; i++)
                tags[i] = multiTag[i - 1];

            return tags;
        }
        public static string[] GetTags(this Transform transform)
        {
            return transform.gameObject.GetTags();
        }
        public static string[] GetTags(this Component component)
        {
            return component.gameObject.GetTags();
        }

        public static bool CompareTags(this GameObject gameObject, string tag)
        {
            return gameObject.GetTags().Contains(tag);
        }
        public static bool CompareTags(this Transform transform, string tag)
        {
            return transform.gameObject.CompareTags(tag);
        }
        public static bool CompareTags(this Component component, string tag)
        {
            return component.gameObject.CompareTags(tag);
        }
    }
}