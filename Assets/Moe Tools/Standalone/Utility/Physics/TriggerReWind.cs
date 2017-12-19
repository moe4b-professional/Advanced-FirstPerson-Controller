using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moe.Tools
{
    public class TriggerReWind : MonoBehaviour
    {
        internal event Action<Collider> TriggerEnter;
        void OnTriggerEnter(Collider collider)
        {
            if (TriggerEnter != null)
                TriggerEnter(collider);
        }

        internal event Action<Collider> TriggerExit;
        void OnTriggerExit(Collider collider)
        {
            if (TriggerExit != null)
                TriggerExit(collider);
        }

        internal event Action<Collider> TriggerStay;
        void OnTriggerStay(Collider collider)
        {
            if (TriggerStay != null)
                TriggerStay(collider);
        }
    }
}