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
    [RequireComponent(typeof(Image))]
    public class OptionsBoxArrow : UIBehaviour, IPointerDownHandler
    {
        public Color NormalColor { get; protected set; }
        [SerializeField]
        Color disabledColor = Color.grey;
        public Color DisabledColor { get { return disabledColor; } set { disabledColor = value; } }

        Image image;
        public Image Image
        {
            get
            {
                if (image == null)
                {
                    image = GetComponent<Image>();

                    NormalColor = image.color;
                }

                return image;
            }
        }

        public bool Interactable
        {
            set
            {
                Image.color = value ? NormalColor : disabledColor;
            }
        }

        public void Init(Action clickAction)
        {
            this.clickAction = clickAction;
        }

        Action clickAction;
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (clickAction != null)
                clickAction();
        }
    }
}