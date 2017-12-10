using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Moe.Tools;

namespace Moe.Tools
{
    public interface IInputModulator
    {

    }
    public abstract partial class InputModulator : ScriptableObject
    {
        public abstract Type ModuleType
        {
            get;
        }

        [SerializeField]
        protected DeployablePlatform[] platforms;
        public DeployablePlatform[] Platforms { get { return platforms; } }

        [Serializable]
        public class DeployablePlatform
        {
            [SerializeField]
            string name;
            public string Name { get { return name; } }

            [SerializeField]
            InputModule module;
            public InputModule Module { get { return module; } }

            [SerializeField]
            RuntimePlatform[] supportedPlatforms;

            public bool IsCurrentPlatform
            {
                get
                {
                    return supportedPlatforms.Contains(Application.platform);
                }
            }

            public DeployablePlatform(string name, RuntimePlatform[] supportedPlatforms)
            {
                this.name = name;
                this.supportedPlatforms = supportedPlatforms;
            }
        }

        public InputModulator()
        {
            platforms = new DeployablePlatform[]
            {
            new DeployablePlatform("PC", new RuntimePlatform[] { RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer, RuntimePlatform.OSXEditor, RuntimePlatform.OSXPlayer, RuntimePlatform.LinuxEditor, RuntimePlatform.LinuxPlayer }),
            new DeployablePlatform("Mobile", new RuntimePlatform[] { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer, RuntimePlatform.TizenPlayer }),
            new DeployablePlatform("XBOX", new RuntimePlatform[] { RuntimePlatform.XboxOne }),
            new DeployablePlatform("PS4", new RuntimePlatform[] { RuntimePlatform.PS4 })
            };
        }
    }

    public interface IInputModulator<T>
        where T : InputModule
    {

    }
    public class InputModulator<T> : InputModulator where T : InputModule
    {
        public override Type ModuleType
        {
            get
            {
                return typeof(T);
            }
        }

        public virtual T GetCurrentModule()
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i].IsCurrentPlatform)
                {
                    platforms[i].Module.Init();

                    return (T)platforms[i].Module;
                }
            }

            return null;
        }
    }
}