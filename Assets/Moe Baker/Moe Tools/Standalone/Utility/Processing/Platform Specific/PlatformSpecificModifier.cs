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
	public abstract class PlatformSpecificModifier : MonoBehaviour
	{
        [SerializeField]
        protected PlatformOverrideValue platformOverride;
        public PlatformOverrideValue PlatformOverride { get { return platformOverride; } }

        public GameTargetPlatform GetCurrentPlatform()
        {
#if UNITY_EDITOR
            if (platformOverride.Enabled)
                return platformOverride.Value;
#endif

            return MoeTools.Platform.Current;
        }

        protected virtual void Start()
        {
            Init(GetCurrentPlatform());
        }

        protected virtual void Init(GameTargetPlatform platform)
        {
            InitPlatform(platform);

            switch (platform)
            {
                case GameTargetPlatform.PC:
                    InitPC();
                    return;

                case GameTargetPlatform.Mobile:
                    InitMobile();
                    return;

                case GameTargetPlatform.Console:
                    InitConsole();
                    return;
            }

            throw GetUndefinedPlatformException(platform);
        }

        protected virtual void InitPlatform(GameTargetPlatform platform) { }
        protected virtual void InitPC() { }
        protected virtual void InitMobile() { }
        protected virtual void InitConsole() { }

        public static ArgumentOutOfRangeException GetUndefinedPlatformException(GameTargetPlatform platform)
        {
            return new ArgumentOutOfRangeException("platform" + platform.ToString() + " Not Defined");
        }
    }

    public abstract class PlatformSpecificDataModifier<TData, TPCData, TMobileData, TConsoleData> : PlatformSpecificModifier
        where TPCData : TData
        where TMobileData : TData
        where TConsoleData : TData
    {
        [SerializeField]
        protected TPCData _PC;
        public TPCData PC { get { return _PC; } }

        [SerializeField]
        protected TMobileData mobile;
        public TMobileData Mobile { get { return mobile; } }

        [SerializeField]
        protected TConsoleData console;
        public TConsoleData Console { get { return console; } }

        protected override void InitPlatform(GameTargetPlatform platform)
        {
            base.InitPlatform(platform);

            InitPlatform(platform, GetPlatformData(platform));
        }
        protected virtual void InitPlatform(GameTargetPlatform platform, TData data) { }

        protected override void InitPC()
        {
            base.InitPC();

            InitPC(_PC);
        }
        protected virtual void InitPC(TPCData data) { }

        protected override void InitMobile()
        {
            base.InitMobile();

            InitMobile(mobile);
        }
        protected virtual void InitMobile(TMobileData data) { }

        protected override void InitConsole()
        {
            base.InitConsole();

            InitConsole(console);
        }
        protected virtual void InitConsole(TConsoleData data) { }

        public virtual TData GetPlatformData(GameTargetPlatform platform)
        {
            switch (platform)
            {
                case GameTargetPlatform.PC:
                    return _PC;

                case GameTargetPlatform.Mobile:
                    return mobile;

                case GameTargetPlatform.Console:
                    return console;
            }

            throw GetUndefinedPlatformException(platform);
        }
    }
    public abstract class PlatformSpecificDataModifier<TData> : PlatformSpecificDataModifier<TData, TData, TData, TData> { }
}