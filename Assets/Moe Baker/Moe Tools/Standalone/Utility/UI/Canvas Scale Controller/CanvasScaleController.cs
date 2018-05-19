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
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScaleController : PlatformSpecificDataModifier<CanvasScaleProfile>
	{
        public virtual CanvasScaler GetScaler()
        {
            return GetComponent<CanvasScaler>();
        }

        protected override void InitPlatform(GameTargetPlatform platform, CanvasScaleProfile data)
        {
            base.InitPlatform(platform, data);

            data.Apply(GetScaler());
        }
    }
}