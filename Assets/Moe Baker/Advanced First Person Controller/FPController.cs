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

using Moe.Tools;

namespace AFPC
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public abstract partial class FPControllerBase : MonoBehaviour
    {
        public const string MenuPath = "UFPC/";

        FPController This { get { return this as FPController; } }


        [SerializeField]
        protected bool _control = true;
        public virtual bool Control
        {
            get
            {
                return _control;
            }
            set
            {
                _control = value;
            }
        }


        [SerializeField]
        protected InitMode initMode = InitMode.Start;
        public InitMode InitMode
        {
            get
            {
                return initMode;
            }
            set
            {
                initMode = value;
            }
        }


        new public Rigidbody rigidbody { get; protected set; }
        protected virtual void InitRigidBody()
        {
            rigidbody = GetComponent<Rigidbody>();

            rigidbody.freezeRotation = true;
        }

        new public CapsuleCollider collider { get; protected set; }
        protected virtual void InitCollider()
        {
            collider = GetComponent<CapsuleCollider>();
        }


        public FPController.ModulesManager Modules { get; protected set; }
        [Serializable]
        public abstract partial class ModulesManagerBase : MoeModulesManager<FPController.Module, FPController>
        {

        }
        protected virtual void InitModules()
        {
            Modules = new FPController.ModulesManager();

            AddModules();

            Modules.Init(This);
        }
        protected virtual void AddModules()
        {
            Modules.AddAll(gameObject);

            InitMovement();

            InitLook();
        }

        public abstract partial class ModuleBase : MoeModule<FPController>
        {
            FPController.Module This { get { return this as FPController.Module; } }

            public FPController Controller { get { return Link; } }

            new public Rigidbody rigidbody { get { return Controller.rigidbody; } }
            new public CapsuleCollider collider { get { return Controller.collider; } }

            public ControllerInputModule InputModule { get { return Controller.InputModule; } }
        }

        public ControllerMovement Movement { get; protected set; }
        protected virtual void InitMovement()
        {
            Movement = Modules.Find<ControllerMovement>();
        }

        public ControllerLook Look { get; protected set; }
        protected virtual void InitLook()
        {
            Look = Modules.Find<ControllerLook>();
        }
        public CameraRig CameraRig { get { return Look.CameraRig; } }


        [SerializeField]
        protected ControllerInputModulator inputModulator;
        public ControllerInputModulator InputModulator { get { return inputModulator; } }
        public ControllerInputModule InputModule { get; protected set; }
        protected virtual void InitInput()
        {
            if (inputModulator == null)
                throw new NullReferenceException("No Input Modulator defined for Controller " + name);

            InputModule = inputModulator.GetCurrentModule();

            if (InputModule == null)
                throw new ArgumentException("No Input Module was retrieved for Controller " + name + ", Check Input Modulator " + inputModulator.name + " And set an Input Module For the current platform");
        }


        protected virtual void Awake()
        {
            if (initMode == InitMode.Awake)
                Init();
            else if(initMode == InitMode.None)
                enabled = false;
        }


        protected virtual void Start()
        {
            if (initMode == InitMode.Start)
                Init();
        }


        protected virtual void Init()
        {
            if (!enabled)
                enabled = true;

            InitRigidBody();

            InitCollider();

            InitInput();

            InitModules();
        }


        protected virtual void Update()
        {
            Process();
        }

        public event Action OnProcess;
        protected virtual void Process()
        {
            ProcessInputModule();

            Movement.Process();

            Look.Process();

            if (OnProcess != null)
                OnProcess();
        }

        protected virtual void ProcessInputModule()
        {
            InputModule.Process();
        }


        protected virtual void FixedUpdate()
        {
            FixedProcess();
        }

        public event Action OnFixedProcess;
        protected virtual void FixedProcess()
        {
            Movement.FixedProcess();

            if (OnFixedProcess != null)
                OnFixedProcess();
        }
    }

	public partial class FPController : FPControllerBase
    {
        [Serializable]
        public partial class ModulesManager : ModulesManagerBase
        {

        }

        public abstract partial class Module : ModuleBase
        {

        }
	}

    public enum InitMode
    {
        None, Awake, Start
    }
}