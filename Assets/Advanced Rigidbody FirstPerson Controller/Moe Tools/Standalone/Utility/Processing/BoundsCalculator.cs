using System;
using System.IO;
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
    public partial class BoundsCalculator : MonoBehaviour
    {
        [SerializeField]
        Bounds value = new Bounds(Vector3.zero, Vector3.one);
        public Bounds Value { get { return value; } }

        Bounds worldValue;
        public Bounds WorldValue
        {
            get
            {
                worldValue = value;
                worldValue.center += transform.position;

                return worldValue;
            }
        }

        void Reset()
        {
            Calculate();
        }

        protected virtual void Calculate()
        {
            value = MoeTools.GameObject.GetLocalBounds(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            Gizmos.DrawWireCube(transform.position + value.center, value.size);
        }
    }
}