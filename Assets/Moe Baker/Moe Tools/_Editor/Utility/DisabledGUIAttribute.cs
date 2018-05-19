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

namespace ARFC
{
	public class DisabledGUIAttribute : PropertyAttribute
	{
        [CustomPropertyDrawer(typeof(DisabledGUIAttribute))]
        public class Drawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUI.enabled = false;

                base.OnGUI(position, property, label);

                GUI.enabled = true;
            }
        }
	}
}
#endif