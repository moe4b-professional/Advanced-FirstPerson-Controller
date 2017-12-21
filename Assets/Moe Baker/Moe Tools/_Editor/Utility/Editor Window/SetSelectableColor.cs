#if UNITY_EDITOR
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public class SetSelectableColor : EditorWindow
    {
        [MenuItem("Moe/Tools/Set Selectable Colors")]
        static void Init()
        {
            SetSelectableColor window = GetWindow<SetSelectableColor>();

            window.Show();

            window.SetDefaultColors();
        }
        
        [SerializeField]
        ColorProperty normalColor;
        [SerializeField]
        ColorProperty highlightedColor;
        [SerializeField]
        ColorProperty pressedColor;
        [SerializeField]
        ColorProperty disabledColor;
        void SetDefaultColors()
        {
            normalColor = new ColorProperty(true, ColorBlock.defaultColorBlock.normalColor);
            highlightedColor = new ColorProperty(true, ColorBlock.defaultColorBlock.highlightedColor);
            pressedColor = new ColorProperty(true, ColorBlock.defaultColorBlock.pressedColor);
            disabledColor = new ColorProperty(true, ColorBlock.defaultColorBlock.disabledColor);
        }

        void OnGUI()
        {
            if (GUILayout.Button("Set Defaults"))
                SetDefaultColors();

            normalColor.Draw("Normal Color");
            highlightedColor.Draw("Highlighted Color");
            pressedColor.Draw("Pressed Color");
            disabledColor.Draw("Disabled Color");

            if (Selection.activeGameObject)
            {
                if (GUILayout.Button("Apply"))
                {
                    MoeTools.GameObject.GetNestedComponents<Selectable>(Selection.activeGameObject).ForEach((Selectable selectable) =>
                    {
                        Debug.Log(selectable.name);

                        ColorBlock colors = selectable.colors;

                        if (normalColor.enabled)
                            colors.normalColor = normalColor.color;

                        if (highlightedColor.enabled)
                            colors.highlightedColor = highlightedColor.color;

                        if (pressedColor.enabled)
                            colors.pressedColor = pressedColor.color;

                        if (disabledColor.enabled)
                            colors.disabledColor = disabledColor.color;

                        selectable.colors = colors;
                    });
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please Select A GameObject", MessageType.Warning);
            }
        }

        [Serializable]
        public class ColorProperty
        {
            public bool enabled;
            public Color color;

            public void Draw(string label)
            {
                enabled = EditorGUILayout.Toggle(label, enabled);

                if (enabled)
                {
                    color = EditorGUILayout.ColorField("Value", color);

                    EditorGUILayout.Space();
                }
            }

            public ColorProperty(bool enabled, Color color)
            {
                this.enabled = enabled;
                this.color = color;
            }
        }
    }
}
#endif