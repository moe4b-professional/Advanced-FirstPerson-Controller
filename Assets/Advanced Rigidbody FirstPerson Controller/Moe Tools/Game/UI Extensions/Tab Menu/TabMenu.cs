using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Moe.GameFramework;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Moe.Tools;

namespace Moe.Tools
{
    public class TabMenu : MonoBehaviour
    {
        [SerializeField]
        int index;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                NavigateTo(value);
            }
        }

        [SerializeField]
        List<Tab> tabs;
        public List<Tab> Tabs { get { return tabs; } }
        public string[] TabsNames
        {
            get
            {
                if (tabs == null || tabs.Count == 0)
                    return new string[] { };

                return tabs.GetArrayOf(delegate (Tab tab) { return tab.Name; });
            }
        }

        public virtual void InitButton(Button button, int index)
        {
            tabs[index].Init(button);

            button.onClick.AddListener(() => NavigateTo(index));
        }

        public int Add(string name, GameObject menu)
        {
            if (tabs.Any(delegate (Tab tab) { return tab.Name == name; }))
                throw new Exception("Tab With Name " + name + " Already Added");

            tabs.Add(new Tab(name, menu));

            return tabs.Count - 1;
        }
        public bool Remove(string name, bool destroyMenu = true)
        {
            if (!tabs.Any(delegate (Tab tab) { return tab.Name == name; }))
                throw new Exception("No Tab With Name " + name + " Was Found");

            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].Name == name)
                {
                    if (destroyMenu)
                        Destroy(tabs[i].Menu);

                    tabs.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        public virtual void NavigateTo(int newIndex)
        {
            newIndex = GameTools.Math.ClampRewind(newIndex, 0, tabs.Count - 1);

            if (tabs[newIndex].Button)
                EventSystem.current.SetSelectedGameObject(tabs[newIndex].Button.gameObject);

            for (int i = 0; i < tabs.Count; i++)
                tabs[i].SetVisibility(i == newIndex);

            index = newIndex;
        }

        [Serializable]
        public class Tab
        {
            [SerializeField]
            string name;
            public string Name { get { return name; } }

            [SerializeField]
            GameObject menu;
            public GameObject Menu { get { return menu; } }

            public Button Button { get; private set; }

            public void Init(Button button)
            {
                this.Button = button;
            }

            public void SetVisibility(bool value)
            {
                menu.SetActive(value);
            }

            public Tab(string name, GameObject menu)
            {
                this.name = name;
                this.menu = menu;
                this.Button = null;
            }
        }

        public TabMenu()
        {
            index = 0;
            tabs = new List<Tab>();
        }

#if UNITY_EDITOR
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

                tabs.elementHeight = 40;
                tabs.drawElementCallback = DrawTabsElement;
            }

            void DrawTabsElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                SerializedProperty name = tabs.GetPropertyOfIndex(index, "name");
                SerializedProperty menu = tabs.GetPropertyOfIndex(index, "menu");

                name.stringValue = EditorGUI.TextField(GUIArea.ProgressLine(ref rect), "Name", name.stringValue);
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), menu);
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
#endif
    }
}