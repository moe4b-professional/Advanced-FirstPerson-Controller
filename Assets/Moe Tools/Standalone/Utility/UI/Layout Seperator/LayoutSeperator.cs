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
    public class LayoutSeperator : MonoBehaviour
    {
        [SerializeField]
        GameObject template;

        [SerializeField]
        Color color = Color.black;

        [SerializeField]
        float size = 2f;

        public LayoutType Layout { get; protected set; }
        public enum LayoutType
        {
            Vertical, Horizontal
        }
        public virtual bool VerticalLayout { get { return Layout == LayoutType.Vertical; } }
        public virtual bool HorizontalLayout { get { return Layout == LayoutType.Horizontal; } }

        public List<Transform> Childern { get; protected set; }
        public List<GameObject> Seperators { get; protected set; }

        [SerializeField]
        bool updateOnChange = true;

        void Start()
        {
            if (GetComponent<VerticalLayoutGroup>())
                Layout = LayoutType.Vertical;
            else if (GetComponent<HorizontalLayoutGroup>())
                Layout = LayoutType.Horizontal;
            else
            {
                Debug.LogWarning("No Layout Group Found On (" + name + "), Seperator Cannot Work");
                enabled = false;
                return;
            }

            Childern = new List<Transform>();
            Seperators = new List<GameObject>();

            Process();
        }

        void Update()
        {
            if (updateOnChange)
                CheckAndProcessChange();
        }

        protected virtual void CheckAndProcessChange()
        {
            int count = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeInHierarchy && !transform.GetChild(i).gameObject.GetComponent<LayoutSeperatorElement>())
                    count++;
            }

            if (count != Childern.Count)
                Process();
        }

        protected virtual void Process()
        {
            UpdateChildern();

            UpdateSeperators();
        }

        protected virtual void UpdateChildern()
        {
            Childern.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeInHierarchy && !transform.GetChild(i).gameObject.GetComponent<LayoutSeperatorElement>())
                    Childern.Add(transform.GetChild(i));
            }
        }
        protected virtual void UpdateSeperators()
        {
            Seperators.ForEach((GameObject seperator) => Destroy(seperator));
            Seperators.Clear();

            for (int i = 0; i < Childern.Count - 1; i++)
                Seperators.Add(CreateSeperator(Childern[i].GetSiblingIndex() + 1));
        }

        protected virtual GameObject CreateSeperator(int childIndex)
        {
            GameObject instance = Instantiate(template, transform, false);
            instance.transform.SetSiblingIndex(childIndex);

            Image image = instance.GetComponentInChildren<Image>();
            image.color = color;

            RectTransform rect = image.transform.parent.GetComponent<RectTransform>();

            var layout = instance.GetComponent<LayoutElement>();
            if (VerticalLayout)
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, size);

                layout.minHeight = size;
            }
            if (HorizontalLayout)
            {
                rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);

                layout.minWidth = size;
            }

            return instance;
        }
    }
}