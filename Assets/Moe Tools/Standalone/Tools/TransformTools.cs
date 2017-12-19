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
using URandom = UnityEngine.Random;
using UTransform = UnityEngine.Transform;

namespace Moe.Tools
{
	public static partial class MoeTools
    {
        public static class Transform
        {
            public static void FaceTransform(UTransform transform, UTransform target)
            {
                transform.eulerAngles = GetFacingAngles(transform, target);
            }

            public static Vector3 GetFacingAngles(UTransform transform, UTransform target)
            {
                return GetFacingAngles(transform, target.position);
            }
            public static Vector3 GetFacingAngles(UTransform transform, Vector3 target)
            {
                var direction = target - transform.position;

                direction.y = 0f;

                return Quaternion.LookRotation(direction).eulerAngles;
            }

            public static void DestroyChildern(UTransform transform)
            {
                for (int i = 0; i < transform.childCount; i++)
                    Object.Destroy(transform.GetChild(i).gameObject);
            }

            public static List<T> GetNestedComponents<T>(UTransform transform)
            {
                List<T> list = new List<T>();

                list = transform.gameObject.GetComponents<T>().ToList();

                for (int i = 0; i < transform.childCount; i++)
                    list.AddRange(GetNestedComponents<T>(transform.GetChild(i)));

                return list;
            }

            static Vector3 Vector3;
            //Position
            public static void SetPositionX(UTransform transform, float value)
            {
                Vector3 = transform.position;
                Vector3.x = value;
                transform.position = Vector3;
            }
            public static void SetPositionY(UTransform transform, float value)
            {
                Vector3 = transform.position;
                Vector3.y = value;
                transform.position = Vector3;
            }
            public static void SetPositionZ(UTransform transform, float value)
            {
                Vector3 = transform.position;
                Vector3.z = value;
                transform.position = Vector3;
            }

            //Local Position
            public static void SetLocalPositionX(UTransform transform, float value)
            {
                Vector3 = transform.localPosition;
                Vector3.x = value;
                transform.localPosition = Vector3;
            }
            public static void SetLocalPositionY(UTransform transform, float value)
            {
                Vector3 = transform.localPosition;
                Vector3.y = value;
                transform.localPosition = Vector3;
            }
            public static void SetLocalPositionZ(UTransform transform, float value)
            {
                Vector3 = transform.localPosition;
                Vector3.z = value;
                transform.localPosition = Vector3;
            }

            //Euler Angles
            public static void SetEulerAnglesX(UTransform transform, float value)
            {
                Vector3 = transform.eulerAngles;
                Vector3.x = value;
                transform.eulerAngles = Vector3;
            }
            public static void SetEulerAnglesY(UTransform transform, float value)
            {
                Vector3 = transform.eulerAngles;
                Vector3.y = value;
                transform.eulerAngles = Vector3;
            }
            public static void SetEulerAnglesZ(UTransform transform, float value)
            {
                Vector3 = transform.eulerAngles;
                Vector3.z = value;
                transform.eulerAngles = Vector3;
            }

            //Local Euler Angles
            public static void SetLocalEulerAnglesX(UTransform transform, float value)
            {
                Vector3 = transform.localEulerAngles;
                Vector3.x = value;
                transform.localEulerAngles = Vector3;
            }
            public static void SetLocalEulerAnglesY(UTransform transform, float value)
            {
                Vector3 = transform.localEulerAngles;
                Vector3.y = value;
                transform.localEulerAngles = Vector3;
            }
            public static void SetLocalEulerAnglesZ(UTransform transform, float value)
            {
                Vector3 = transform.localEulerAngles;
                Vector3.z = value;
                transform.localEulerAngles = Vector3;
            }

            public static bool IsChild(UTransform parent, UTransform possibleChild)
            {
                UTransform currentTransform;

                do
                {
                    currentTransform = possibleChild.parent;

                    if (currentTransform == parent)
                        return true;
                } while (currentTransform != null);

                return false;
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static void SetPositionX(this UTransform transform, float value)
        {
            MoeTools.Transform.SetPositionX(transform, value);
        }
        public static void SetPositionY(this UTransform transform, float value)
        {
            MoeTools.Transform.SetPositionY(transform, value);
        }
        public static void SetPositionZ(this UTransform transform, float value)
        {
            MoeTools.Transform.SetPositionZ(transform, value);
        }

        //Local Position
        public static void SetLocalPositionX(this UTransform transform, float value)
        {
            MoeTools.Transform.SetLocalPositionX(transform, value);
        }
        public static void SetLocalPositionY(this UTransform transform, float value)
        {
            MoeTools.Transform.SetLocalPositionY(transform, value);
        }
        public static void SetLocalPositionZ(this UTransform transform, float value)
        {
            MoeTools.Transform.SetLocalPositionZ(transform, value);
        }

        //Euler Angles
        public static void SetEulerAnglesX(this UTransform transform, float value)
        {
            MoeTools.Transform.SetEulerAnglesX(transform, value);
        }
        public static void SetEulerAnglesY(this UTransform transform, float value)
        {
            MoeTools.Transform.SetEulerAnglesY(transform, value);
        }
        public static void SetEulerAnglesZ(this UTransform transform, float value)
        {
            MoeTools.Transform.SetEulerAnglesZ(transform, value);
        }

        //Local Euler Angles
        public static void SetLocalEulerAnglesX(this UTransform transform, float value)
        {
            MoeTools.Transform.SetLocalEulerAnglesX(transform, value);
        }
        public static void SetLocalEulerAnglesY(this UTransform transform, float value)
        {
            MoeTools.Transform.SetLocalEulerAnglesY(transform, value);
        }
        public static void SetLocalEulerAnglesZ(this UTransform transform, float value)
        {
            MoeTools.Transform.SetLocalEulerAnglesZ(transform, value);
        }
    }
}