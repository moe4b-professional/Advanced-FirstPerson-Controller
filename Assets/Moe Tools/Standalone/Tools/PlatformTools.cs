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
	public static partial class MoeTools
	{
        public static class Platform
        {
            public static GameTargetPlatform Current
            {
                get
                {
                    if (Windows)
                        return GameTargetPlatform.Windows;
                    else if (Linux)
                        return GameTargetPlatform.Linux;
                    else if (OSX)
                        return GameTargetPlatform.OSX;
                    else
                        return GameTargetPlatform.Unknown;
                }
            }

            public static bool Windows
            {
                get
                {
                    return Application.platform == RuntimePlatform.WindowsEditor ||
                        Application.platform == RuntimePlatform.WindowsPlayer;
                }
            }
            public static bool Linux
            {
                get
                {
                    return Application.platform == RuntimePlatform.LinuxEditor ||
                        Application.platform == RuntimePlatform.LinuxPlayer;
                }
            }
            public static bool OSX
            {
                get
                {
                    return Application.platform == RuntimePlatform.OSXEditor ||
                        Application.platform == RuntimePlatform.OSXPlayer;
                }
            }
        }
    }

    public enum GameTargetPlatform
    {
        Windows, Linux, OSX, Unknown
    }
}