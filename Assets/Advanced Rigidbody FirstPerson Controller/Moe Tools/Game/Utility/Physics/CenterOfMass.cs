using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    [RequireComponent(typeof(Rigidbody))]
    public class CenterOfMass : MonoBehaviour
    {
        [SerializeField]
        Vector3 point;

        void Awake()
        {
            GetComponent<Rigidbody>().centerOfMass = point;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireSphere(point, 0.05f);
            Gizmos.DrawLine(point + (Vector3.down * 2), point + (Vector3.up * 2));
        }
    }
}