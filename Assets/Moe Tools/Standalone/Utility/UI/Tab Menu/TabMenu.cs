using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Moe.Tools
{
    public partial class TabMenu : MonoBehaviour
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
            newIndex = MoeTools.Math.ClampRewind(newIndex, 0, tabs.Count - 1);

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
    }
}