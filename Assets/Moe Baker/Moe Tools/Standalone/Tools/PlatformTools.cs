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
            static GameTargetPlatform current = Init();
            public static GameTargetPlatform Current { get { return current; } }

            public static GameTargetPlatform Init()
            {
                pc = new PCData();
                mobile = new MobileData();
                console = new ConsoleData();
                web = new WebData();

                return GetCurrent();
            }
            static GameTargetPlatform GetCurrent()
            {
                if (pc.IsCurrent)
                    return GameTargetPlatform.PC;
                else if (mobile.IsCurrent)
                    return GameTargetPlatform.Mobile;
                else if (console.IsCurrent)
                    return GameTargetPlatform.Console;
                else if (web.IsCurrent)
                    return GameTargetPlatform.Web;
                else
                    return GameTargetPlatform.Unknown;
            }

            static PCData pc;
            public static PCData PC { get { return pc; } }
            public class PCData : Data
            {
                public PCData()
                {
                    RuntimePlatforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.WindowsEditor,
                        RuntimePlatform.WindowsPlayer,

                        RuntimePlatform.OSXEditor,
                        RuntimePlatform.OSXPlayer,

                        RuntimePlatform.LinuxEditor,
                        RuntimePlatform.LinuxPlayer,
                    };
                }
            }

            static MobileData mobile;
            public static MobileData Mobile { get { return mobile; } }
            public class MobileData : Data
            {
                public MobileData()
                {
                    RuntimePlatforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.Android,
                        RuntimePlatform.IPhonePlayer,

                        RuntimePlatform.TizenPlayer,
                    };
                }
            }

            static ConsoleData console;
            public static ConsoleData Console { get { return console; } }
            public class ConsoleData : Data
            {
                public ConsoleData()
                {
                    RuntimePlatforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.PS4,
                        RuntimePlatform.XboxOne,
                    };
                }
            }

            static WebData web;
            public static WebData Web { get { return web; } }
            public class WebData : Data
            {
                public WebData()
                {
                    RuntimePlatforms = new RuntimePlatform[]
                    {
                        RuntimePlatform.WebGLPlayer,
                    };
                }
            }

            public static Data GetData(GameTargetPlatform targetPlatform)
            {
                switch (targetPlatform)
                {
                    case GameTargetPlatform.PC:
                        return pc;

                    case GameTargetPlatform.Mobile:
                        return mobile;

                    case GameTargetPlatform.Console:
                        return console;
                    case GameTargetPlatform.Web:
                        return web;

                    case GameTargetPlatform.Unknown:
                        throw new ArgumentException("No Platform Data Specified For The Unknow Platform");
                }

                throw new ArgumentOutOfRangeException("No Platform Data Is Defined For " + targetPlatform.ToString());
            }
            public static RuntimePlatform[] GetRuntimePlatforms(GameTargetPlatform targetPlatform)
            {
                return GetData(targetPlatform).RuntimePlatforms;
            }

            public abstract class Data
            {
                public bool IsCurrent { get { return IsRuntimePlatform(Application.platform); } }

                public virtual bool IsRuntimePlatform(RuntimePlatform runtimePlatform)
                {
                    return RuntimePlatforms.Contains(runtimePlatform);
                }

                public RuntimePlatform[] RuntimePlatforms { get; protected set; }
            }
        }
    }

    public static partial class MoeToolsExtensionMethods
    {
        public static MoeTools.Platform.Data GetData(this GameTargetPlatform targetPlatform)
        {
            return MoeTools.Platform.GetData(targetPlatform);
        }
        public static RuntimePlatform[] GetRuntimePlatforms(this GameTargetPlatform targetPlatform)
        {
            return MoeTools.Platform.GetRuntimePlatforms(targetPlatform);
        }
    }

    public enum GameTargetPlatform
    {
        PC, Mobile, Console, Web, Unknown
    }

    public enum MobileTargetPlatform
    {
        Android, IPhone, Tizen
    }

    public enum ConsoleTargetPlatform
    {
        XBOX, PlayStation
    }

    public enum PCTargetPlatform
    {
        Windows, Linux, OSX
    }
}