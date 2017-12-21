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
        public class Inspector : InspectorBase<TabMenu>
        {
            ListPopup<Tab> index;
            InspectorList tabs;

            protected override void OnEnable()
            {
                base.OnEnable();

                index = new ListPopup<Tab>(serializedObject.FindProperty("index"), target.TabsNames);

                tabs = new InspectorList(serializedObject.FindProperty("tabs"));
            }

            public override void OnInspectorGUI()
            {
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                index.Draw();
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    target.NavigateTo(index.Index);
                }

                tabs.Draw();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif