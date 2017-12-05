#if UNITY_EDITOR
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
    public abstract class BaseLayoutPropertyDrawer
    {
        public abstract SerializedProperty Property { get; set; }

        public GUIContent label;
        public abstract bool DrawFoldout { get; set; }

        public virtual void Draw()
        {
            if (DrawFoldout)
            {
                Property.isExpanded = EditorGUILayout.Foldout(Property.isExpanded, label, true);

                if (Property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawElements();
                    EditorGUI.indentLevel--;
                }
            }
            else
                DrawElements();
        }
        public virtual void DrawElements()
        {

        }

        protected BaseLayoutPropertyDrawer(SerializedProperty property)
        {
            this.Property = property;

            label = new GUIContent(property.displayName);
        }
    }

    public class LayoutPropertyDrawer : BaseLayoutPropertyDrawer
    {
        public SerializedProperty property;
        public override SerializedProperty Property
        {
            get
            {
                return property;
            }
            set
            {
                property = value;
            }
        }

        public bool drawFoldout = true;
        public override bool DrawFoldout
        {
            get
            {
                return drawFoldout;
            }
            set
            {
                drawFoldout = value;
            }
        }

        public LayoutPropertyDrawer(SerializedProperty property) : base(property)
        {

        }
    }
}
#endif