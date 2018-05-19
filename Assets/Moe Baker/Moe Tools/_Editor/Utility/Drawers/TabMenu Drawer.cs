#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEditor;

namespace Moe.Tools
{
    public partial class TabMenu
    {
        [CustomEditor(typeof(TabMenu))]
        public class Inspector : MoeInspector<TabMenu>
        {
            ListPopup<Tab> index;
            InspectorList tabs;

            protected override void OnEnable()
            {
                base.OnEnable();

                index = new ListPopup<Tab>(serializedObject.FindProperty("index"), target.TabsNames);
                CustomGUI.Overrides.Add(index.Property, DrawIndex);

                tabs = new InspectorList(serializedObject.FindProperty("tabs"));
                CustomGUI.Overrides.Add(tabs.serializedProperty, DrawTabs);
            }

            protected virtual void DrawIndex()
            {
                EditorGUI.BeginChangeCheck();
                index.Draw();
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    target.NavigateTo(index.Index);
                }
            }
            protected virtual void DrawTabs()
            {
                tabs.Draw();
            }
        }
    }
}
#endif