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
using UGameObject = UnityEngine.GameObject;

namespace Moe.Tools
{
	public static partial class MoeTools
	{
        public static class GameObject
        {
            public static void SetLayer(UGameObject gameobject, string layerName)
            {
                SetLayer(gameobject, LayerMask.NameToLayer(layerName));
            }
            public static void SetLayer(UGameObject gameobject, int layerIndex)
            {
                gameobject.layer = layerIndex;

                for (int i = 0; i < gameobject.transform.childCount; i++)
                    SetLayer(gameobject.transform.GetChild(i).gameObject, layerIndex);
            }

            public static Bounds GetWorldBounds(UGameObject gameObject)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);

                if (renderer)
                    bounds = renderer.bounds;

                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    bounds.Encapsulate(GetWorldBounds(gameObject.transform.GetChild(i).gameObject));
                }

                return bounds;
            }
            public static Bounds GetLocalBounds(UGameObject gameObject)
            {
                Bounds bounds = GetWorldBounds(gameObject);

                bounds.center = gameObject.transform.InverseTransformPoint(bounds.center);

                return bounds;
            }

            public static T GetLocalComponent<T>(UGameObject gameObject) where T : Component
            {
                T component = gameObject.GetComponent<T>();

                if (component)
                    return component;

                return gameObject.GetComponentInChildren<T>();
            }

            public static T GetOrAddComponent<T>(UGameObject gameObject) where T : Component
            {
                if (!gameObject.GetComponent<T>())
                    return gameObject.AddComponent<T>();

                return gameObject.GetComponent<T>();
            }

            public static void SetCollision(UGameObject obj1, UGameObject obj2, bool enabled)
            {
                Collider[] col1 = obj1.GetComponentsInChildren<Collider>();
                Collider[] col2 = obj2.GetComponentsInChildren<Collider>();

                for (int x = 0; x < col1.Length; x++)
                {
                    for (int y = 0; y < col2.Length; y++)
                    {
                        Physics.IgnoreCollision(col1[x], col2[y], !enabled);
                    }
                }
            }

            public static List<T> GetNestedComponents<T>(UGameObject gameObject)
            {
                return Transform.GetNestedComponents<T>(gameObject.transform);
            }

            public static List<T> GetAllComponents<T>(bool allowAtEditTime = false)
            {
                if(!Application.isPlaying && !allowAtEditTime)
                    throw new ArgumentException("Allow At EditTime arguments is false, please note that changes to components made at runtime will most likely be non-revertable, set the argument as true if you are sure of what you are doing and make a backup of you scene first !");

                List<T> list = new List<T>();

                for (int x = 0; x < SceneManager.sceneCount; x++)
                {
                    var scene = SceneManager.GetSceneAt(x);

                    list.AddRange(MoeTools.Scene.GetAllComponents<T>(scene, allowAtEditTime));
                }

                return list;
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static void SetLayer(this UGameObject gameobject, string layerName)
        {
            MoeTools.GameObject.SetLayer(gameobject, layerName);
        }
        public static void SetLayer(this UGameObject gameobject, int layerIndex)
        {
            MoeTools.GameObject.SetLayer(gameobject, layerIndex);
        }

        public static Bounds GetWorldBounds(this UGameObject gameObject)
        {
            return MoeTools.GameObject.GetWorldBounds(gameObject);
        }
        public static Bounds GetLocalBounds(this UGameObject gameObject)
        {
            return MoeTools.GameObject.GetLocalBounds(gameObject);
        }

        public static T GetLocalComponent<T>(this UGameObject gameObject) where T : Component
        {
            return MoeTools.GameObject.GetLocalComponent<T>(gameObject);
        }
        public static T GetLocalComponent<T>(this Component component)
            where T : Component
        {
            return MoeTools.GameObject.GetLocalComponent<T>(component.gameObject);
        }

        public static T GetOrAddComponent<T>(this UGameObject gameObject) where T : Component
        {
            return MoeTools.GameObject.GetOrAddComponent<T>(gameObject);
        }
        public static T GetOrAddComponent<T>(this Component component)
            where T : Component
        {
            return MoeTools.GameObject.GetOrAddComponent<T>(component.gameObject);
        }

        public static List<T> GetNestedComponents<T>(this UGameObject gameObject)
        {
            return MoeTools.Transform.GetNestedComponents<T>(gameObject.transform);
        }
        public static List<T> GetNestedComponents<T>(this Component component)
        {
            return MoeTools.Transform.GetNestedComponents<T>(component.transform);
        }

        public static void SetCollision(this UGameObject obj1, UGameObject obj2, bool enabled)
        {
            MoeTools.GameObject.SetCollision(obj1, obj2, enabled);
        }
    }
}