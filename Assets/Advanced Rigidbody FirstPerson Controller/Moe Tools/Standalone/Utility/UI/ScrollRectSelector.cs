using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectSelector : MonoBehaviour
    {
        public ScrollRect Scroll { get; protected set; }
        public RectTransform ScrollRectTrasnform { get; protected set; }

        protected virtual void Awake()
        {
            Scroll = GetComponent<ScrollRect>();
            ScrollRectTrasnform = Scroll.GetComponent<RectTransform>();
        }

        protected virtual void Update()
        {
            if(EventSystem.current && EventSystem.current.currentSelectedGameObject)
            {
                RectTransform current = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

                if(current && current.transform.IsChildOf(Scroll.content))
                {
                    CenterToItem(current);
                }
            }
        }

        public void CenterToItem(RectTransform target)
        {
            
        }
    }
}