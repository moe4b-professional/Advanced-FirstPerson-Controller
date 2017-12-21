using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField]
        GameScene scene;
        public GameScene Scene { get { return scene; } }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(LoadScene);
        }

        public void LoadScene()
        {
            SceneManager.LoadScene(scene.Name);
        }
    }
}