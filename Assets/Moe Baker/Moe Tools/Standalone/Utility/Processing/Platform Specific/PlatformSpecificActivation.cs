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
	public class PlatformSpecificActivation : PlatformSpecificDataModifier<bool>
	{
        protected override void InitPlatform(GameTargetPlatform platform, bool data)
        {
            base.InitPlatform(platform, data);

            gameObject.SetActive(data);
        }
    }
}