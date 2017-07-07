using System;
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

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    Transform point;

	void OnTriggerEnter(Collider col)
    {
        if (col.attachedRigidbody)
            col.attachedRigidbody.velocity = Vector3.zero;

        col.transform.position = point.transform.position;
    }
}