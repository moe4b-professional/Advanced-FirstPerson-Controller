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
    public class KeyCodeList
    {
        [SerializeField]
        KeyCode[] list;
        public KeyCode[] List { get { return list; } }

        public virtual bool GetInput()
        {
            return GameTools.Input.GetInput(list);
        }
        public virtual bool GetInputUp()
        {
            return GameTools.Input.GetInputUp(list);
        }
        public virtual bool GetInputDown()
        {
            return GameTools.Input.GetInputDown(list);
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

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(KeyCodeList))]
        public class Drawer : PropertyDrawer
        {
            public SerializedProperty GetList(SerializedProperty property)
            {
                return property.FindPropertyRelative("list");
            }
            InspectorList list;

            protected virtual void InitList(SerializedProperty property, GUIContent label)
            {
                if(list == null)
                {
                    list = new InspectorList(GetList(property));
                    list.label = label.text;
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                InitList(property, label);

                return EditorGUIUtility.singleLineHeight + (list.serializedProperty.isExpanded ? EditorGUI.GetPropertyHeight(list.serializedProperty) + EditorGUIUtility.singleLineHeight : 0);
            }
            
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                InitList(property, label);

                list.Draw(position);
            }
        }
#endif
    }
}