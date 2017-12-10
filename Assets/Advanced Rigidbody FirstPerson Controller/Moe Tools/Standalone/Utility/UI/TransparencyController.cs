using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;

namespace Moe.Tools
{
    [ExecuteInEditMode()]
    public class TransparencyController : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 1f)]
        float transparency = 1f;
        public float Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                value = Mathf.Clamp01(value);

                SetTransparency(value);
                transparency = value;
            }
        }

        [SerializeField]
        List<Graphic> graphics;

        void Reset()
        {
            graphics = GetComponentsInChildren<Graphic>().ToList();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (UnityEditor.EditorApplication.isPlaying)
                return;

            SetTransparency(transparency);
        }
#endif

        void SetTransparency(float value)
        {
            Color color;
            for (int i = 0; i < graphics.Count; i++)
            {
                color = graphics[i].color;
                color.a = value;

                graphics[i].color = color;
            }
        }

        public void AddGraphic(Graphic graphic)
        {
            graphics.Add(graphic);

            SetTransparency(transparency);
        }

        public void RemoveGraphic(Graphic graphic)
        {
            graphics.Remove(graphic);
        }
    }
}