using System;
using System.IO;
using System.Linq;
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
	public class TagCheckZone : CheckZone
	{
        [SerializeField]
        protected string tagToCheck = "Player";
        public string TagToCheck { get { return tagToCheck; } }

        public override bool CheckCollider(Collider collider)
        {
            if (tagToCheck == "")
                return true;

            return collider.CompareTag(tagToCheck);
        }
    }
}