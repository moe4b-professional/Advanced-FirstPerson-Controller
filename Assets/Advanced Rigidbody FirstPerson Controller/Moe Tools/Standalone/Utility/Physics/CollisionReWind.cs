using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moe.Tools
{
    public class CollisionReWind : MonoBehaviour
    {
        internal event Action<Collision> collisionEnter;

        void OnCollisionEnter(Collision collision)
        {
            if (collisionEnter != null)
                collisionEnter(collision);
        }
    }
}