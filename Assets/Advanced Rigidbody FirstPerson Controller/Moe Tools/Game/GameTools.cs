using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using URandom = UnityEngine.Random;
using UTransform = UnityEngine.Transform;
using UGameObject = UnityEngine.GameObject;
using UInput = UnityEngine.Input;

using SEnum = System.Enum;

namespace Moe.Tools
{
    public static partial class GameTools
    {
        public const string MenuPath = Constants.CreateAssetMenuPath + "Tools/";
    }

    public static partial class GameToolsExtensionMethods
    {
        
    }
}