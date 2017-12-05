using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace Moe.Tools
{
    public class CollisionIgnoreController : MonoBehaviour
    {
        [SerializeField]
        Set[] sets;

        void Awake()
        {
            sets.ForEach((Set set) => set.Apply());
        }

        [Serializable]
        public class Set
        {
            [SerializeField]
            GameObject obj1;
            public GameObject Obj1 { get { return obj1; } }

            [SerializeField]
            GameObject obj2;
            public GameObject Obj2 { get { return obj2; } }

            [SerializeField]
            bool enabled = true;
            public bool Enabled { get { return enabled; } }

            public void Apply()
            {
                GameTools.GameObject.SetCollision(obj1, obj2, enabled);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CollisionIgnoreController))]
        public class Inspector : InspectorBase<CollisionIgnoreController>
        {
            InspectorList sets;

            protected override void OnEnable()
            {
                base.OnEnable();

                sets = new InspectorList(serializedObject.FindProperty("sets"));
                sets.drawElementCallback = DrawSetsElement;
                sets.elementHeight = 60f;
            }

            protected virtual void DrawSetsElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                SerializedProperty obj1 = sets.GetPropertyOfIndex(index, "obj1");
                SerializedProperty obj2 = sets.GetPropertyOfIndex(index, "obj2");
                SerializedProperty enabled = sets.GetPropertyOfIndex(index, "enabled");

                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), obj1);
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), obj2);
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), enabled);
            }

            public override void OnInspectorGUI()
            {
                EditorGUILayout.Space();

                sets.Draw();

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}