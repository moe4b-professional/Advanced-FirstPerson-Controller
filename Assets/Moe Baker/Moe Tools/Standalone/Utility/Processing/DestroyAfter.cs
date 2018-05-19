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
    public class DestroyAfter : MonoBehaviour
    {
        [SerializeField]
        float time;
        public float Time
        {
            get
            {
                return time;
            }
            set
            {
                if (value < 0f)
                    value = 0f;

                time = value;
            }
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(time);

            Destroy(gameObject);
        }
    }
}