#if UNITY_EDITOR
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
    public class LayoutPropertyCustomDrawer : BaseLayoutPropertyDrawer
    {
        public PropertyCustomGUI gui;

        public override SerializedProperty Property
        {
            get
            {
                return gui.Property;
            }
            set
            {
                gui = new PropertyCustomGUI(value);
            }
        }
        public override bool DrawFoldout
        {
            get
            {
                return gui.DrawFoldout;
            }
            set
            {
                gui.DrawFoldout = value;
            }
        }

        public override void Draw()
        {
            gui.Draw();
        }

        public LayoutPropertyCustomDrawer(SerializedProperty property) : base(property)
        {

        }
    }
}
#endif