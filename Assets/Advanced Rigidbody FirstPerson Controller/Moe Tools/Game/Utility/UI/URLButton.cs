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
    [RequireComponent(typeof(Button))]
    public class URLButton : MonoBehaviour
    {
        [SerializeField]
        string URL;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OpenURL);
        }

        public void OpenURL()
        {
            Application.OpenURL(URL);
        }
    }
}