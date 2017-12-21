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
	public class Coordinates
	{
        [SerializeField]
        public Vector3 position;

        [SerializeField]
        public Quaternion rotation;
        public virtual Vector3 EulerRotation
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = Quaternion.Euler(value);
            }
        }

        public static readonly Coordinates Zero = new Coordinates(Vector3.zero, Quaternion.identity);

        public Coordinates(Transform transform) : this(transform.position, transform.rotation)
        {

        }
        public Coordinates(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    public static class CoordinatesTools
    {
        public static void ApplyCoords(this Transform transform, Coordinates coords)
        {
            ApplyCoords(transform, coords, Space.World);
        }
        public static void ApplyCoords(this Transform transform, Coordinates coords, Space space)
        {
            if(space == Space.Self)
            {
                transform.localPosition = coords.position;
                transform.localRotation = coords.rotation;
            }
            else if(space == Space.World)
            {
                transform.position = coords.position;
                transform.rotation = coords.rotation;
            }
        }

        public static Coordinates GetCoords(this Transform transform)
        {
            return new Coordinates(transform);
        }
    }
}