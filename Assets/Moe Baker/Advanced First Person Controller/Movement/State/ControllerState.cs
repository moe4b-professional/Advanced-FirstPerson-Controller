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
	public abstract partial class ControllerStateBase : FPController.Module
	{
        public ControllerMovement Movement { get { return Controller.Movement; } }

        [SerializeField]
        protected bool _control = true;
        public virtual bool Control
        {
            get
            {
                return _control && Movement.Control.AbsoluteToggle;
            }
            set
            {
                _control = value;
            }
        }

        [SerializeField]
        protected ControllerState.Type startingState = ControllerState.Type.Walk;
        public ControllerState.Type StartingState { get { return startingState; } }
        public ControllerState.IData StartStateData { get { return GetData(StartingState); } }

        [SerializeField]
        protected ControllerState.Data walk = new ControllerState.Data(1.8f, 0.4f, 4f);
        public ControllerState.Data Walk { get { return walk; } }

        [SerializeField]
        protected ControllerState.Data crouch = new ControllerState.Data(1f, 0.4f, 1f);
        public ControllerState.Data Crouch { get { return crouch; } }

        [SerializeField]
        protected ControllerState.Data prone = new ControllerState.Data(0.4f, 0.2f, 0.35f);
        public ControllerState.Data Prone { get { return prone; } }

        [SerializeField]
        protected SprintStateData sprint;
        public SprintStateData Sprint { get { return sprint; } }
        [Serializable]
        public class SprintStateData : ControllerState.InstanceData
        {
            public override ControllerState.Type Source { get { return ControllerState.Type.Walk; } }

            public SprintStateData()
            {
                speed = new FloatOverrideValue(true, 7f);
            }
        }
        
        public virtual ControllerState.IData GetData(ControllerState.Type type)
        {
            switch (type)
            {
                case ControllerState.Type.Walk:
                    return walk;

                case ControllerState.Type.Crouch:
                    return crouch;

                case ControllerState.Type.Prone:
                    return prone;

                case ControllerState.Type.Sprint:
                    return sprint;
            }

            throw new ArgumentException();
        }

        public ControllerStateTransition Transition { get; protected set; }
        protected virtual void InitTransition()
        {
            Transition = Controller.Modules.Find<ControllerStateTransition>();
        }


        public ControllerStateTraverser Traverser { get; protected set; }
        protected virtual void InitTraverser()
        {
            Traverser = Controller.Modules.Find<ControllerStateTraverser>();

            if (Traverser == null)
                throw MoeTools.ExceptionTools.Templates.MissingDependacny<ControllerStateTraverser, ControllerState>(name);
        }


        public override void Init(FPController link)
        {
            base.Init(link);

            Sprint.Init(Controller);

            InitTransition();
            InitTraverser();
        }
    }

    public partial class ControllerState : ControllerStateBase
    {
        public interface IData
        {
            float Height { get; }

            float Radius { get; }

            float Speed { get; }
        }

        [Serializable]
        public class Data : IData
        {
            [SerializeField]
            float height;
            public float Height
            {
                get
                {
                    return height;
                }
                set
                {
                    if (value < 0f)
                        throw MoeTools.ExceptionTools.Templates.NegativeValue<Data>("height", value);

                    height = value;
                }
            }

            [SerializeField]
            float radius;
            public float Radius
            {
                get
                {
                    return radius;
                }
                set
                {
                    if (value < 0f)
                        throw MoeTools.ExceptionTools.Templates.NegativeValue<Data>("radius", value);

                    radius = value;
                }
            }

            [SerializeField]
            float speed;
            public float Speed
            {
                get
                {
                    return speed;
                }
                set
                {
                    if (value < 0f)
                        throw MoeTools.ExceptionTools.Templates.NegativeValue<Data>("speed", value);

                    speed = value;
                }
            }

            public Data() : this(0f, 0f, 0f)
            {

            }
            public Data(float height, float radius, float speed)
            {
                this.height = height;
                this.radius = radius;
                this.speed = speed;
            }
        }

        public enum Type
        {
            Walk, Crouch, Prone, Sprint
        }

        [Serializable]
        public abstract class InstanceData : IData
        {
            public abstract Type Source { get; }

            public virtual IData SourceData { get; protected set; }
            public virtual void Init(FPController controller)
            {
                SourceData = controller.Movement.State.GetData(Source);
            }

            [SerializeField]
            protected FloatOverrideValue height;
            public float Height
            {
                get
                {
                    if (height.Enabled) return height.Value;

                    return SourceData.Height;
                }
            }

            [SerializeField]
            protected FloatOverrideValue radius;
            public float Radius
            {
                get
                {
                    if (radius.Enabled) return radius.Value;

                    return SourceData.Radius;
                }
            }

            [SerializeField]
            protected FloatOverrideValue speed;
            public float Speed
            {
                get
                {
                    if (speed.Enabled) return speed.Value;

                    return SourceData.Speed;
                }
            }
        }

        public class DataSet<TData> : ScriptableObject
        {
            [SerializeField]
            protected TData walk;
            public TData Walk { get { return walk; } }

            [SerializeField]
            protected TData crouch;
            public TData Crouch { get { return crouch; } }

            [SerializeField]
            protected TData prone;
            public TData Prone { get { return prone; } }

            [SerializeField]
            protected TData sprint;
            public TData Sprint { get { return sprint; } }

            public virtual TData Get(Type type)
            {
                switch (type)
                {
                    case Type.Walk:
                        return walk;

                    case Type.Crouch:
                        return crouch;

                    case Type.Prone:
                        return prone;

                    case Type.Sprint:
                        return sprint;
                }

                throw new ArgumentException();
            }

            public virtual TData Get(FPController controller)
            {
                return Get(controller.Movement.State);
            }
            public virtual TData Get(FPController controller, IData target)
            {
                return Get(controller.Movement.State, target);
            }

            public virtual TData Get(ControllerState state)
            {
                return Get(state, state.Traverser.Target);
            }
            public virtual TData Get(ControllerState state, IData target)
            {
                if (target == state.walk) return walk;
                else if (target == state.crouch) return crouch;
                else if (target == state.prone) return prone;
                else if (target == state.sprint) return sprint;

                throw new ArgumentException("No Data corresponding to target data " + state.Traverser.Target);
            }
        }
    }
}