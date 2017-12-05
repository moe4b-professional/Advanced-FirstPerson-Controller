#if UNITY_EDITOR
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
    public class MultiPrefab : MonoBehaviour
    {
        [SerializeField]
        Data[] objects;
        public Data[] Objects { get { return objects; } }
        [Serializable]
        public class Data
        {
            [SerializeField]
            GameObject prefab;
            public GameObject Prefab { get { return prefab; } }

            [SerializeField]
            GameObject instance;
            public GameObject Instance { get { return instance; } }

            [SerializeField]
            Vector3 position;
            public Vector3 Position { get { return position; } }

            [SerializeField]
            Vector3 rotation;
            public Vector3 Rotation { get { return rotation; } }

            [SerializeField]
            Vector3 scale;
            public Vector3 Scale { get { return scale; } }

            public void Instantiate(Transform parent)
            {
                if (instance)
                    DestroyImmediate(instance);

                if (prefab == null)
                    return;

                instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                UpdateInstance(parent);
            }

            public void UpdateInstance(Transform parent)
            {
                if (!instance)
                    return;

                instance.transform.SetParent(parent, true);

                UpdateInstanceCoords();
            }
            public void UpdateInstanceCoords()
            {
                if (!instance)
                    return;

                instance.transform.localPosition = position;
                instance.transform.localEulerAngles = rotation;
                instance.transform.localScale = scale;
            }
        }

        public virtual void Instantiate()
        {
            objects.ForEach((Data obj) => obj.Instantiate(transform));
        }

        public virtual void Clear()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].Instance)
                    DestroyImmediate(objects[i].Instance);
            }
        }

#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(MultiPrefab))]
        public class Inspector : InspectorBase<MultiPrefab>
        {
            InspectorList objects;

            protected override void OnEnable()
            {
                base.OnEnable();

                objects = new InspectorList(serializedObject.FindProperty("objects"));
                objects.drawElementCallback = DrawObjectsElement;
                objects.elementHeight = 80f;

                objects.onAddCallback = delegate
                {
                    ReorderableList.defaultBehaviours.DoAddButton(objects);

                    if (objects.count == 1)
                    {
                        SerializedProperty scale = objects.GetArrayElement(0).FindPropertyRelative("scale");

                        scale.FindPropertyRelative("x").floatValue = 1f;
                        scale.FindPropertyRelative("y").floatValue = 1f;
                        scale.FindPropertyRelative("z").floatValue = 1f;
                    }
                };
            }

            protected virtual void DrawObjectsElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                EditorGUIUtility.fieldWidth = 50f;

                SerializedProperty prefab = objects.GetPropertyOfIndex(index, "prefab");
                SerializedProperty position = objects.GetPropertyOfIndex(index, "position");
                SerializedProperty rotation = objects.GetPropertyOfIndex(index, "rotation");
                SerializedProperty scale = objects.GetPropertyOfIndex(index, "scale");

                EditorGUI.BeginChangeCheck();
                prefab.objectReferenceValue = EditorGUI.ObjectField(GUIArea.ProgressLine(ref rect), "Prefab", prefab.objectReferenceValue, typeof(GameObject), false);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();

                    ForAllTargets((MultiPrefab target) => target.objects[index].Instantiate(target.transform));
                }

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), position);
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), rotation);
                EditorGUI.PropertyField(GUIArea.ProgressLine(ref rect), scale);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();

                    ForAllTargets((MultiPrefab target) => target.objects[index].UpdateInstanceCoords());
                }
            }

            public override void OnInspectorGUI()
            {
                if (targets.Length == 1)
                {
                    EditorGUILayout.Space();
                    objects.Draw();
                }

                serializedObject.ApplyModifiedProperties();

                if (GUILayout.Button("Instantiate"))
                {
                    ForAllTargets((MultiPrefab target) => target.Instantiate());
                }
                if (GUILayout.Button("Clear"))
                {
                    ForAllTargets((MultiPrefab target) => target.Clear());
                }
            }
        }
#endif
    }
}
#endif