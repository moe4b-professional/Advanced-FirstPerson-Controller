using System;
using System.IO;
using System.Linq;
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
    [Serializable]
    public partial class KeyCodeList
    {
        [SerializeField]
        KeyCode[] list;
        public KeyCode[] List { get { return list; } }

        public virtual bool GetInput()
        {
            return MoeTools.Input.GetInput(list);
        }
        public virtual bool GetInputUp()
        {
            return MoeTools.Input.GetInputUp(list);
        }
        public virtual bool GetInputDown()
        {
            return MoeTools.Input.GetInputDown(list);
        }

        public KeyCodeList() : this(new KeyCode[] { })
        {

        }
        public KeyCodeList(KeyCode value1) : this(new KeyCode[] { value1 })
        {

        }
        public KeyCodeList(KeyCode value1, KeyCode value2) : this(new KeyCode[] { value1, value2 })
        {

        }
        public KeyCodeList(KeyCode value1, KeyCode value2, KeyCode value3) : this(new KeyCode[] { value1, value2, value3})
        {

        }
        public KeyCodeList(KeyCode[] list)
        {
            if (list == null)
                list = new KeyCode[] { };
            else
                this.list = list;
        }
    }
}