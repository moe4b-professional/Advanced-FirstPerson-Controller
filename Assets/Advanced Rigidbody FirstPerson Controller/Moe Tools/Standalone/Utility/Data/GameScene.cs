using System;
using System.Linq;
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
    public partial class GameScene
    {
        [SerializeField]
        string name;
        public string Name { get { return GetName(); } }
        protected virtual string GetName()
        {
            if(Application.isEditor)
            {
                if (asset && asset.name != name)
                {
                    Debug.LogWarning("Asset " + MoeTools.String.Enclose(asset.ToString()) + " Doesn't Match Saved Name " + MoeTools.String.Enclose(name) + ", Updating Name");

                    name = asset.name;
                }
            }

            return name;
        }

        [SerializeField]
        Object asset;
#if UNITY_EDITOR
        public Object Asset { get { return asset; } }
#endif
    }
}