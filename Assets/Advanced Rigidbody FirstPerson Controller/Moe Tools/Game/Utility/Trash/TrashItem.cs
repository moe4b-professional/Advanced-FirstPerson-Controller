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
    [Serializable]
    public class TrashItem
    {
        [SerializeField]
        GameObject gameObject;
        public GameObject GameObject { get { return gameObject; } }

        [SerializeField]
        float time;
        public float Time { get { return time; } set { time = value; } }

        public void Destroy()
        {
            if (gameObject)
                GameObject.Destroy(gameObject);
        }

        public TrashItem(GameObject item, float delay)
        {
            this.gameObject = item;
            this.time = delay;
        }
    }
}