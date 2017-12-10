#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

using UnityEditor;

namespace Moe.Tools
{
    public partial class OptionsBox
    {
        [CustomEditor(typeof(OptionsBox))]
        public class Inspector : InspectorBaseCustomDrawer<OptionsBox>
        {
            ListPopup<string> value;
            InspectorList options;
            PropertyDisplayEnumDrawer<Transition> transition;
            SerializedProperty colors;
            SerializedProperty animations;
            SerializedProperty sprites;

            protected override void OnEnable()
            {
                base.OnEnable();

                options = new InspectorList(serializedObject.FindProperty("options"));

                InitValue();

                transition = new PropertyDisplayEnumDrawer<Transition>(serializedObject.FindProperty("m_Transition"));
                colors = serializedObject.FindProperty("m_Colors");
                animations = serializedObject.FindProperty("m_AnimationTriggers");
                sprites = serializedObject.FindProperty("m_SpriteState");
                transition.Assign(Transition.ColorTint, colors);
                transition.Assign(Transition.Animation, animations);
                transition.Assign(Transition.SpriteSwap, sprites);

                gui.Ignores.Add(colors.name);
                gui.Ignores.Add(animations.name);
                gui.Ignores.Add(sprites.name);

                gui.Overrides.Add(transition.property.name, DrawTransition);

                gui.Overrides.Add(options.serializedProperty.name, DrawOptions);
                gui.Overrides.Add(value.Property.name, DrawValue);
            }

            public override void OnInspectorGUI()
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                {
                    gui.Draw();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    target.UpdateText();
                }
            }

            protected virtual void InitValue()
            {
                value = new ListPopup<string>(serializedObject.FindProperty("value"), target.options);
            }

            protected virtual void DrawValue()
            {
                EditorGUILayout.Space();

                value.Draw();
            }

            protected virtual void DrawTransition()
            {
                transition.Draw();
            }

            void DrawOptions()
            {
                EditorGUI.BeginChangeCheck();
                {
                    options.Draw();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    InitValue();
                }
            }
        }
    }
}
#endif