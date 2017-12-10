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
    public class DestroyZone : MonoBehaviour
    {
        void OnTriggerEnter(Collider collider)
        {
            if (collider.attachedRigidbody)
                Destroy(collider.attachedRigidbody.gameObject);
            else
                Destroy(collider.gameObject);
        }
    }
}