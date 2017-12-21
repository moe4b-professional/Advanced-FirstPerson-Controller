using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Moe.Tools
{
    [RequireComponent(typeof(Button))]
    public partial class TabButton : MonoBehaviour
    {
        [SerializeField]
        TabMenu menu;
        public TabMenu Menu { get { return menu; } set { menu = value; } }

        [SerializeField]
        int index;
        public int Index { get { return index; } set { index = value; } }

        void Awake()
        {
            menu.InitButton(GetComponent<Button>(), index);
        }
    }
}