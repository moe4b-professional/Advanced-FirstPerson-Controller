using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

using Moe.Tools;

namespace ARFC
{
    public class BaseFPController : TypesData.TFPController
    {
        public const string MenuPath = MoeTools.Constants.Paths.Menu + "ARFC/";

        public FPController This { get { return this as FPController; } }

        /// <summary>
        /// define when the controller will be initilized
        /// </summary>
        [SerializeField]
        ControllerInitMode initMode = ControllerInitMode.Start;
        public ControllerInitMode InitMode { get { return initMode; } }
        /// <summary>
        /// has the controller been initlized ?
        /// </summary>
        protected bool initilized = false;

        public FPController.ModuleManager Modules { get; protected set; }
        [Serializable]
        public abstract class BaseModuleManager : MoeLinkedModuleManager<FPController.Module, FPController>
        {
            public FPController Controller { get { return Link; } }

            public virtual void Init(FPController controller)
            {
                this.Link = controller;

                ForAll(InitModule);
            }
            protected virtual void InitModule(FPController.Module module)
            {
                module.Init(Controller);
            }
        }

        /// <summary>
        /// controller constraints (move, jump, ...)
        /// </summary>
        [SerializeField]
        protected FPController.ConstraintsData constraints;
        public FPController.ConstraintsData Constraints { get { return constraints; } }
        /// <summary>
        /// the base constraints data, mostly has booleans that can be toggled to restrict and allow certian actions
        /// </summary>
        [Serializable]
        public class BaseConstraintsData
        {
            //the base condition, turning this of will stop all form of control from the controller
            [SerializeField]
            protected bool control = true;
            public bool Control { get { return control; } set { control = value; } }

            [Space]
            [SerializeField]
            protected bool move = true;
            public virtual bool Move { get { return Control && move; } set { move = value; } }

            [SerializeField]
            protected bool jump = true;
            public virtual bool Jump { get { return Control && jump; } protected set { jump = value; } }

            [SerializeField]
            protected bool jumpFromAir = false;
            public virtual bool JumpFromAir { get { return Control && jumpFromAir; } protected set { jumpFromAir = value; } }

            [SerializeField]
            protected bool sprint = true;
            public virtual bool Sprint { get { return Control && sprint; } protected set { sprint = value; } }

            [SerializeField]
            protected bool crouch = true;
            public virtual bool Crouch { get { return Control && crouch; } protected set { crouch = value; } }

            [SerializeField]
            protected bool prone = true;
            public virtual bool Prone { get { return Control && prone; } protected set { prone = value; } }

            [SerializeField]
            protected bool slide = true;
            public virtual bool Slide { get { return Control && slide; } protected set { slide = value; } }

            [Space]
            [SerializeField]
            protected bool look = true;
            public virtual bool Look { get { return Control && look; } set { look = value; } }

            [SerializeField]
            protected bool lean = true;
            public virtual bool Lean { get { return Control && lean; } protected set { lean = value; } }

            [SerializeField]
            protected bool headBob = true;
            public virtual bool HeadBob { get { return Control && headBob; } protected set { headBob = value; } }
        }

        /// <summary>
        /// movement data, will calculate and apply velocity based on current state and speed
        /// </summary>
        [Space()]
        [SerializeField]
        protected FPController.MovementModule movement;
        public FPController.MovementModule Movement { get { return movement; } }
        /// <summary>
        /// base movement module, processes the speed, gravity, in air movement, jump force, ...
        /// </summary>
        [Serializable]
        public class BaseMovementModule : FPController.Module
        {
            [SerializeField]
            protected FPController.MovementModule.SpeedData speed;
            public FPController.MovementModule.SpeedData Speed { get { return speed; } }
            [Serializable]
            public class BaseSpeedModule : FPController.Module
            {
                [SerializeField]
                protected Vector2 vector;
                public Vector2 Vector { get { return vector; } }

                [SerializeField]
                protected MaxFloatValue value;
                public MaxFloatValue Value { get { return value; } }

                public float Magnitude { get; protected set; }

                [SerializeField]
                protected FPController.MovementModule.SpeedData.SpeedAxisSmoothValue walk = new FPController.MovementModule.SpeedData.SpeedAxisSmoothValue(15f);
                public FPController.MovementModule.SpeedData.SpeedAxisSmoothValue Walk { get { return walk; } }

                [SerializeField]
                protected FPController.MovementModule.SpeedData.SpeedAxisSmoothValue strafe = new FPController.MovementModule.SpeedData.SpeedAxisSmoothValue(15f);
                public FPController.MovementModule.SpeedData.SpeedAxisSmoothValue Strafe { get { return strafe; } }

                public override void Init(FPController controller)
                {
                    base.Init(controller);

                    walk.SetController(Controller);
                    strafe.SetController(Controller);

                    Update(Vector2.zero);
                }

                public virtual void SetDelta(float newDelta)
                {
                    walk.Delta = Strafe.Delta = newDelta;
                }

                public virtual void Update(Vector2 input)
                {
                    UpdateValue();

                    UpdateVector(input);
                }
                public virtual void UpdateValue()
                {
                    value.Max = States.Traverser.TargetData.Speed;
                    value.Value = States.Traverser.CurrentData.Speed;
                }
                public virtual void UpdateVector(Vector2 input)
                {
                    walk.Update(input.y);
                    Strafe.Update(input.x);

                    vector.y = walk.Value;
                    vector.x = strafe.Value;

                    if (vector.magnitude > value.Max)
                        vector = vector.normalized * value.Max;

                    Magnitude = vector.magnitude / value.Max;
                }

                [Serializable]
                public abstract class BaseSpeedAxisSmoothValue : BaseAxisSmoothValue
                {
                    public FPController Controller { get; protected set; }
                    public virtual void SetController(BaseFPController controller)
                    {
                        this.Controller = controller as FPController;
                    }

                    public float MaxSpeed { get { return Controller.Movement.speed.value.Max; } }

                    public override float Min { get { return -MaxSpeed; } }
                    public override float Max { get { return MaxSpeed; } }

                    public BaseSpeedAxisSmoothValue(float delta) : base(delta)
                    {

                    }
                }
            }

            /// <summary>
            /// gravity multiplier that is to be multiplied by the Physics.gravity.y
            /// </summary>
            [SerializeField]
            protected float gravityMultiplier = 1;
            public float GravityMultiplier { get { return gravityMultiplier; } }

            /// <summary>
            /// defines in air movement data
            /// </summary>
            [SerializeField]
            protected InAirData inAir;
            public InAirData InAir { get { return inAir; } }
            [Serializable]
            public class InAirData
            {
                [SerializeField]
                [Range(0f, 1f)]
                float control = 0.15f;
                public float Control { get { return control; } }

                [SerializeField]
                float deAcceleration = 3f;
                public float DeAcceleration { get { return deAcceleration; } }

                [SerializeField]
                protected bool updateDirection = false;
                public bool UpdateDirection { get { return updateDirection; } }
            }

            [SerializeField]
            protected Vector3 velocity;
            public Vector3 Velocity { get { return velocity; } }

            public Vector3 Forward { get; protected set; }
            public Vector3 Right { get; protected set; }
            protected virtual void UpdateDirections()
            {
                if (CurrentState == ControllerState.Sliding && !Slide.UpdateDirection)
                    return;

                Forward = Transform.forward;
                Right = Transform.right;
            }

            public override void Init(FPController controller)
            {
                base.Init(controller);

                speed.Init(controller);
            }

            public virtual void Process()
            {
                if (OnGround)
                    Ground();
                else
                    Air();
            }

            protected virtual void Ground()
            {
                UpdateDirections();

                if (Constraints.Move)
                {
                    if (CurrentState == ControllerState.Sliding)
                        speed.Update(Slide.VectorInput);
                    else
                        speed.Update(InputModule.Movement);

                    velocity = Forward * Speed.Vector.y + Right * Speed.Vector.x;
                }
                else
                    velocity = Vector3.zero;

                velocity = Vector3.ProjectOnPlane(velocity, GroundCast.Normal);

                #region Draw Direction
#if UNITY_EDITOR
                if (velocity == Vector3.zero)
                    Debug.DrawRay(Transform.position, Vector3.ProjectOnPlane(Transform.forward, GroundCast.Normal), Color.yellow);
                else
                    Debug.DrawRay(Transform.position, velocity.normalized, Color.yellow);
#endif
                #endregion

                if ((GroundCast.Rigidbody && !GroundCast.Rigidbody.isKinematic) || Rigidbody.velocity.y < 0f)
                {
                    velocity.y = Rigidbody.velocity.y;
                }

                ApplyForce();
            }

            protected virtual void Air()
            {
                velocity = Rigidbody.velocity;
                var y = velocity.y;

                velocity.y = 0f;

                velocity = Vector3.MoveTowards(velocity, Vector3.zero, inAir.DeAcceleration * Time.deltaTime);

                if (Constraints.Move)
                {
                    speed.UpdateVector(InputModule.Movement * inAir.Control);

                    if (inAir.UpdateDirection)
                        UpdateDirections();

                    velocity += (Transform.forward * Speed.Vector.y + Transform.right * Speed.Vector.x) * inAir.Control;

                    if (velocity.magnitude > Speed.Value.Max)
                        velocity = velocity.normalized * Speed.Value.Max;
                }

                velocity.y = y;

                if (Jump.Power.Value == 0f)
                    AirFall();
                else
                    AirJump();

                ApplyForce();
            }
            protected virtual void AirFall()
            {
                Rigidbody.useGravity = true;

                velocity.y = Rigidbody.velocity.y * gravityMultiplier;
            }
            protected virtual void AirJump()
            {
                Rigidbody.useGravity = false;

                velocity.y = Jump.Power.Value + Jump.StartVelocity;
            }

            protected virtual void ApplyForce()
            {
                Rigidbody.velocity = velocity;
            }
        }

        /// <summary>
        /// controller's state data (crouching, sprinting, ...) will controller and apply state transitions
        /// </summary>
        [SerializeField]
        protected FPController.StatesModule states;
        public FPController.StatesModule States { get { return states; } }
        /// <summary>
        /// the base states module, will define all of the controller's sates (crouch, sprinting, ...) and process input to transition between those states using its nested Traverser module
        /// </summary>
        [Serializable]
        public class BaseStatesModule : FPController.Module
        {
            //the state that the controller will start as
            [SerializeField]
            ControllerState startingState = ControllerState.Walking;
            public ControllerState StartingState { get { return startingState; } set { startingState = value; } }
            public ControllerStateData StartingStateData { get { return GetData(startingState); } }

            [SerializeField]
            ControllerStateData walk = new ControllerStateData(1.8f, 0.35f, 3.5f, 0.5f, ControllerState.Walking);
            public ControllerStateData Walk { get { return walk; } }

            [SerializeField]
            protected SprintData sprint;
            public SprintData Sprint { get { return sprint; } }
            [Serializable]
            public class SprintData
            {
                [SerializeField]
                protected float speed = 7;
                public float Speed { get { return speed; } }

                [SerializeField]
                protected float stepTime = 0.3f;
                public float StepTime { get { return stepTime; } }

                [SerializeField]
                protected SoundSet sounds;
                public SoundSet Sounds { get { return sounds; } }

                [SerializeField]
                protected HeadBobData headbob;
                public HeadBobData Headbob { get { return headbob; } }

                public ControllerStateData State { get; protected set; }
                public void SetState(ControllerStateData walkState)
                {
                    State = new ControllerStateData(walkState.Height, walkState.Radius, speed, stepTime, sounds, headbob, ControllerState.Sprinting);
                }

                //defines the kind of input to handle sprinting
                [SerializeField]
                protected ButtonInputMode input = ButtonInputMode.Hold;
                public ButtonInputMode Input { get { return input; } set { input = value; } }
            }

            [SerializeField]
            ControllerStateData crouch = new ControllerStateData(1f, 0.35f, 1f, 0.8f, ControllerState.Crouching);
            public ControllerStateData Crouch { get { return crouch; } }

            [SerializeField]
            ControllerStateData prone = new ControllerStateData(0.4f, 0.2f, 0.5f, 1f, ControllerState.Proning);
            public ControllerStateData Prone { get { return prone; } }

            [SerializeField]
            FPController.StatesModule.TraverserModule traverser;
            public FPController.StatesModule.TraverserModule Traverser { get { return traverser; } }
            /// <summary>
            /// the states traverser, handles transitioning from one state to the other
            /// </summary>
            [Serializable]
            public class BaseTraverserModule : FPController.Module
            {
                /// <summary>
                /// used to lerp the previous state to the target state
                /// </summary>
                [SerializeField]
                ScaleSmoothValue lerp = new ScaleSmoothValue(3.5f);
                public ScaleSmoothValue Lerp { get { return lerp; } }
                public float LerpValue
                {
                    get
                    {
                        return lerp.Value;
                    }
                    protected set
                    {
                        lerp.Value = value;
                    }
                }
                public float DefaultLerpDelta { get; protected set; }
                public float? OverrideLerpDelta
                {
                    set
                    {
                        if (value == null)
                            lerp.Delta = DefaultLerpDelta;
                        else
                            lerp.Delta = value.Value;
                    }
                }

                /// <summary>
                /// ration of the pivot to the camera, will define where the pivot is positioned in relation to the current state's height
                /// </summary>
                [SerializeField]
                [Range(0f, 1f)]
                float pivotScale = 0.5f;
                public float PivotScale { get { return pivotScale; } }

                /// <summary>
                /// will lower the camera & pivot by the current value, so the camera wont by right at the top of the collider
                /// </summary>
                [SerializeField]
                float cameraOffset = 0.05f;
                public float CameraOffset { get { return cameraOffset; } }

                /// <summary>
                /// the previous state's data
                /// </summary>
                protected ControllerStateData previousData;
                public ControllerStateData PreviousData { get { return previousData; } }

                /// <summary>
                /// the current state's data, a lerp resault from the previous and target using the lerpScale value
                /// </summary>
                protected ControllerStateData currentData;
                public ControllerStateData CurrentData { get { return currentData; } }

                /// <summary>
                /// the target state's data
                /// </summary>
                protected ControllerStateData targetData;
                public ControllerStateData TargetData { get { return targetData; } }
                public virtual void UpdateTarget()
                {
                    targetData = States.GetData(Current);

                    UpdateCurrent();
                }

                /// <summary>
                /// Controller State Of The Previous Data
                /// </summary>
                public ControllerState Previous { get { return previousData.State; } }
                /// <summary>
                /// Controller state of the target state, which is considered the current state
                /// </summary>
                public ControllerState Current { get { return targetData.State; } }

                public override void Init(FPController controller)
                {
                    base.Init(controller);

                    DefaultLerpDelta = lerp.Delta;

                    previousData = currentData = targetData = States.StartingStateData;
                }

                public virtual void Process()
                {
                    if (LerpValue != 1f)
                    {
                        if (targetData.Height > previousData.Height && RoofCast.Process())
                            GoToSafeState();

                        if (LerpValue != 1f)
                        {
                            Lerp.MoveTowardsMax();

                            UpdateCurrent();

                            if (LerpValue == 1f)
                            {
                                Controller.InvokeOnStateChangeEnd(Current);
                            }
                        }

                        Apply();
                    }

                    ApplyCameraRig();
                }

                protected virtual void UpdateCurrent()
                {
                    currentData = ControllerStateData.Lerp(previousData, targetData, lerp.Value);
                }

                /// <summary>
                /// go to state defined by a controller state
                /// </summary>
                /// <param name="newState"></param>
                public virtual void GoTo(ControllerState newState)
                {
                    GoTo(States.GetData(newState));
                }
                /// <summary>
                /// go to state using its data
                /// </summary>
                /// <param name="newState"></param>
                public virtual void GoTo(ControllerStateData newState)
                {
                    GoTo(newState, ControllerStateData.InverseLerpHeight(targetData, newState, currentData));
                }
                /// <summary>
                /// internal method to transition to state based on a certian lerpScale, will be automatically calculated when using the above methods, the lerp scale will have any (mid transition) transition smooth
                /// </summary>
                /// <param name="newState"></param>
                /// <param name="lerpScale"></param>
                protected virtual void GoTo(ControllerStateData newState, float lerpScale)
                {
                    lerpScale = Mathf.Clamp01(lerpScale);

                    previousData = targetData;
                    targetData = newState;

                    lerp.Value = lerpScale;

                    UpdateCurrent();

                    Controller.InvokeOnStateChangeStart(previousData.State, targetData.State);
                }

                /// <summary>
                /// go to a safe state based on the roof cast
                /// </summary>
                protected virtual void GoToSafeState()
                {
                    if (States.Crouch.Height < CurrentData.Height)
                        GoTo(ControllerState.Crouching);
                    else if (States.Prone.Height < CurrentData.Height)
                        GoTo(ControllerState.Proning);
                }

                /// <summary>
                /// appply the current state data
                /// </summary>
                protected virtual void Apply()
                {
                    ApplyCollider();

                    ApplyCameraRig();
                }
                /// <summary>
                /// apply current state's collider data (height, radius)
                /// </summary>
                protected virtual void ApplyCollider()
                {
                    Collider.height = currentData.Height;
                    Collider.radius = currentData.Radius;

                    Collider.center = new Vector3(0f, Collider.height / 2f, 0f);
                }
                /// <summary>
                /// apply camera rig data (coordinates)
                /// </summary>
                protected virtual void ApplyCameraRig()
                {
                    CameraRig.Coordinates.Pivot.position = GetPivotPosition(CurrentData);
                    CameraRig.Coordinates.Camera.position = GetCameraPosition(CurrentData);
                }
                protected virtual Vector3 GetPivotPosition(ControllerStateData stateData)
                {
                    return new Vector3(0, (stateData.Height * pivotScale) - cameraOffset, 0);
                }
                protected virtual Vector3 GetCameraPosition(ControllerStateData stateData)
                {
                    return new Vector3(0, stateData.Height - (stateData.Height * pivotScale) - cameraOffset, 0);
                }

                public virtual FPController.CameraRigModule.CoordinatesData GetCurrentCameraCoordinates()
                {
                    return GetStateCameraCoordinates(currentData);
                }
                public virtual FPController.CameraRigModule.CoordinatesData GetStateCameraCoordinates(ControllerStateData stateData)
                {
                    return new BaseCameraRigModule.CoordinatesData(GetPivotPosition(stateData), GetCameraPosition(stateData));
                }
            }

            public override void Init(FPController controller)
            {
                InitStates();

                base.Init(controller);

                traverser.Init(controller);
            }
            protected virtual void InitStates()
            {
                walk.State = ControllerState.Walking;
                sprint.SetState(walk);
                crouch.State = ControllerState.Crouching;
                prone.State = ControllerState.Proning;
            }

            public virtual void Process()
            {
                ProcessState();

                traverser.Process();
            }

            protected virtual void ProcessState()
            {
                if (ProcessSprint())
                {

                }
                else if (ProcessCrouch())
                {

                }
                else if (ProcessProne())
                {

                }
            }

            public bool SprintLock { get; protected set; }
            protected virtual bool ProcessSprint()
            {
                if (InputModule.Sprint)
                {
                    if (!SprintLock)
                    {
                        if (sprint.Input == ButtonInputMode.Hold)
                        {
                            if (Constraints.Sprint && traverser.Current != ControllerState.Sprinting && OnGround)
                            {
                                SprintStart();

                                return true;
                            }
                        }
                        else if (sprint.Input == ButtonInputMode.Toggle)
                        {
                            if (traverser.Current == ControllerState.Sprinting)
                                SprintEnd();
                            else
                                SprintStart();

                            SprintLock = true;

                            return true;
                        }
                    }
                }
                else
                {
                    if (sprint.Input == ButtonInputMode.Hold)
                    {
                        if (traverser.Current == ControllerState.Sprinting)
                        {
                            SprintEnd();

                            return true;
                        }
                    }

                    SprintLock = false;
                }

                if (!Constraints.Sprint && traverser.Current == ControllerState.Sprinting && sprint.Input == ButtonInputMode.Toggle)
                    traverser.GoTo(ControllerState.Walking);

                return false;
            }
            protected virtual void SprintStart()
            {
                if (Slide.Processing)
                    Slide.End(ControllerState.Sprinting);
                else
                    traverser.GoTo(ControllerState.Sprinting);
            }
            protected virtual void SprintEnd()
            {
                traverser.GoTo(ControllerState.Walking);
            }

            protected virtual bool ProcessCrouch()
            {
                if (InputModule.Crouch)
                {
                    if (CurrentState == ControllerState.Crouching)
                    {
                        traverser.GoTo(ControllerState.Walking);

                        return true;
                    }
                    else if (CurrentState == ControllerState.Sliding)
                    {
                        Slide.End(ControllerState.Sprinting);
                    }
                    else if (Constraints.Slide && traverser.Current == ControllerState.Sprinting && OnGround)
                    {
                        SprintLock = true;

                        Slide.Start();
                    }
                    else if (Constraints.Crouch)
                    {
                        traverser.GoTo(ControllerState.Crouching);

                        return true;
                    }
                }

                return false;
            }

            protected virtual bool ProcessProne()
            {
                if (InputModule.Prone)
                {
                    if (traverser.Current == ControllerState.Proning)
                    {
                        traverser.GoTo(ControllerState.Walking);

                        return true;
                    }
                    else if (Constraints.Prone)
                    {
                        traverser.GoTo(ControllerState.Proning);

                        return true;
                    }
                }

                return false;
            }

            public ControllerStateData GetData(ControllerState state)
            {
                switch (state)
                {
                    case ControllerState.Walking:
                        return walk;
                    case ControllerState.Sprinting:
                        return sprint.State;
                    case ControllerState.Crouching:
                        return crouch;
                    case ControllerState.Proning:
                        return prone;
                    case ControllerState.Sliding:
                        return Slide.StateData;
                }

                throw new ArgumentException("Controller State " + state.ToString() + " Not Defined");
            }
        }

        public ControllerState CurrentState { get { return states.Traverser.Current; } }
        public ControllerStateData CurrentStateData { get { return states.Traverser.CurrentData; } }
        /// <summary>
        /// defines a state's data
        /// </summary>
        [Serializable]
        public struct ControllerStateData
        {
            public ControllerState State { get; set; }

            [SerializeField]
            float height;
            public float Height { get { return height; } }

            [SerializeField]
            float radius;
            public float Radius { get { return radius; } }

            [SerializeField]
            float speed;
            public float Speed { get { return speed; } }

            [SerializeField]
            float stepTime;
            public float StepTime { get { return stepTime; } }

            [SerializeField]
            SoundSet sounds;
            public SoundSet Sounds { get { return sounds; } }

            [SerializeField]
            HeadBobData headbob;
            public HeadBobData HeadBob { get { return headbob; } }

            [Serializable]
            public struct DependenciesData
            {
                [SerializeField]
                float stepTime;
                public float StepTime { get { return stepTime; } }

                [SerializeField]
                SoundSet sounds;
                public SoundSet Sounds { get { return sounds; } }

                [SerializeField]
                HeadBobData headbob;
                public HeadBobData Headbob { get { return headbob; } }

                public DependenciesData(float stepTime)
                {
                    this.stepTime = stepTime;

                    sounds = null;
                    headbob = null;
                }
            }

            static ControllerStateData tempState = new ControllerStateData();
            public static ControllerStateData Lerp(ControllerStateData a, ControllerStateData b, float t)
            {
                Lerp(ref tempState, a, b, t);

                return tempState;
            }
            public static void Lerp(ref ControllerStateData target, ControllerStateData a, ControllerStateData b, float t)
            {
                target.height = Mathf.Lerp(a.height, b.height, t);
                target.radius = Mathf.Lerp(a.radius, b.radius, t);
                target.speed = Mathf.Lerp(a.speed, b.speed, t);
            }

            public static float InverseLerp(ControllerStateData a, ControllerStateData b, ControllerStateData value)
            {
                float resault = 0f;

                resault += InverseLerpHeight(a, b, value);
                resault += InverseLerpRadius(a, b, value);
                resault += InverseLerpSpeed(a, b, value);

                return resault / 3;
            }
            public static float InverseLerpHeight(ControllerStateData a, ControllerStateData b, ControllerStateData value)
            {
                return Mathf.InverseLerp(a.height, b.height, value.height);
            }
            public static float InverseLerpRadius(ControllerStateData a, ControllerStateData b, ControllerStateData value)
            {
                return Mathf.InverseLerp(a.radius, b.radius, value.radius);
            }
            public static float InverseLerpSpeed(ControllerStateData a, ControllerStateData b, ControllerStateData value)
            {
                return Mathf.InverseLerp(a.speed, b.speed, value.speed);
            }

            public ControllerStateData(float height, float radius, float speed, float stepTime, ControllerState state) :
                this(height, radius, speed, stepTime, null, null, state)
            {

            }
            public ControllerStateData(float height, float radius, float speed, float stepTime, SoundSet sounds, HeadBobData headbob, ControllerState state)
            {
                this.height = height;
                this.radius = radius;
                this.speed = speed;

                this.stepTime = stepTime;
                this.sounds = sounds;

                this.headbob = headbob;

                this.State = state;
            }

            public override bool Equals(object obj)
            {
                if (obj is ControllerStateData)
                {
                    var objStateData = (ControllerStateData)obj;

                    return objStateData.height == height &&
                        objStateData.radius == radius &&
                        objStateData.speed == speed;
                }

                return false;
            }

            public static bool operator ==(ControllerStateData obj1, ControllerStateData obj2)
            {
                return obj1.Equals(obj2);
            }
            public static bool operator !=(ControllerStateData obj1, ControllerStateData obj2)
            {
                return !obj1.Equals(obj2);
            }

            public override int GetHashCode()
            {
                return height.GetHashCode() ^ radius.GetHashCode() ^ speed.GetHashCode();
            }
        }

        [SerializeField]
        protected FPController.SlideModule slide;
        public FPController.SlideModule Slide { get { return slide; } }
        [Serializable]
        public abstract class BaseSlideModule : FPController.Module
        {
            [SerializeField]
            protected float stepTime = 0.4f;
            public float StepTime { get { return stepTime; } }

            [SerializeField]
            protected SoundSet sounds;
            public SoundSet Sounds { get { return sounds; } }

            [SerializeField]
            protected HeadBobData headbob;
            public HeadBobData Headbob { get { return headbob; } }

            [SerializeField]
            protected MaxSmoothValue power = new MaxSmoothValue(1.2f, 0.85f);
            public MaxSmoothValue Power { get { return power; } }

            [SerializeField]
            [Tooltip("Defines The Scale To Multiply To The Value That Defines How Fast The Transition From One State To Another Will Be, Make Traversion Faster Or Slower")]
            protected float stateTraverseMultiplier = 2f;
            public float StateTraverseMultiplier { get { return stateTraverseMultiplier; } }

            [SerializeField]
            protected bool updateDirection = false;
            public bool UpdateDirection { get { return updateDirection; } }

            public ControllerStateData StateData { get; protected set; }
            public float Speed { get; protected set; }

            public Vector2 VectorInput { get; protected set; }

            public virtual bool Processing { get { return power.Value > 0f; } }

            public virtual void Start()
            {
                VectorInput = InputModule.Movement;
                Speed = Movement.Speed.Value.Value;
                power.SetValueToMax();

                UpdateStateData();
                States.Traverser.OverrideLerpDelta = States.Traverser.DefaultLerpDelta * stateTraverseMultiplier;
                States.Traverser.GoTo(ControllerState.Sliding);
            }

            public virtual void UpdateStateData()
            {
                StateData = new ControllerStateData(States.Crouch.Height, States.Crouch.Radius, Speed * power.Value, stepTime, sounds, headbob, ControllerState.Sliding);
            }

            public virtual void Process()
            {
                if (power.Value > 0f)
                {
                    if (OnGround)
                    {
                        power.MoveTowardsMin();

                        UpdateStateData();
                        States.Traverser.UpdateTarget();
                    }

                    if (power.Value <= 0f || !OnGround || CheckBlockingCollision())
                        EndAction();
                }
            }

            protected bool CheckBlockingCollision()
            {
                return Rigidbody.velocity.magnitude < 0.4f;
            }

            public virtual void End(ControllerState stateToTransitionTo)
            {
                power.Value = 0f;
                States.Traverser.OverrideLerpDelta = null;
                States.Traverser.GoTo(stateToTransitionTo);

                Sound.Source.Stop();
            }
            protected virtual void EndAction()
            {
                End(ControllerState.Crouching);
            }
        }

        /// <summary>
        /// controls the controllers jump functionality and handles the jump input
        /// </summary>
        [SerializeField]
        protected FPController.JumpModule jump;
        public FPController.JumpModule Jump { get { return jump; } }
        /// <summary>
        /// the base jump module, handles the jump input and functionality
        /// </summary>
        public class BaseJumpModule : FPController.Module
        {
            /// <summary>
            /// variable defining the jump's max power and delta value
            /// </summary>
            [SerializeField]
            protected MaxSmoothValue power = new MaxSmoothValue(7f, 12f);
            public MaxSmoothValue Power { get { return power; } }

            /// <summary>
            /// maximum number of times the controller is allowed to jump
            /// </summary>
            [SerializeField]
            protected int maxCount = 1;
            public int MaxCount
            {
                get
                {
                    return maxCount;
                }
                set
                {
                    if (value < 1)
                        value = 1;

                    maxCount = value;
                }
            }
            public bool MaxedCount { get { return count >= maxCount; } }
            /// <summary>
            /// current number of times a character has jumped
            /// </summary>
            protected int count;
            public int Count { get { return count; } }

            /// <summary>
            /// multiplier to the force (jump power's value) which will be applied to the platform that the controller is standing on
            /// </summary>
            [SerializeField]
            protected float pushDownMultiplier = 2.5f;
            public float PushDownMultiplier
            {
                get
                {
                    return pushDownMultiplier;
                }
                set
                {
                    pushDownMultiplier = value;
                }
            }

            /// <summary>
            /// the start velocity of the jump (the platform's the player is standing on's velocity)
            /// </summary>
            protected float startVelocity;
            public float StartVelocity { get { return startVelocity; } }

            public virtual void Process()
            {
                if (OnGround)
                {
                    count = 0;
                }
                else if (power.Value != 0f)
                {
                    if (RoofCast.Process() && (!RoofCast.Rigidbody || RoofCast.Rigidbody.isKinematic))
                        power.SetValueToMin();
                    else
                        power.MoveTowardsMin();

                    if (power.Value == 0f)
                        End();
                }

                ProcessInput();
            }

            protected virtual void ProcessInput()
            {
                if (InputModule.Jump)
                {
                    if (CurrentState == ControllerState.Crouching || CurrentState == ControllerState.Proning)
                    {
                        States.Traverser.GoTo(ControllerState.Walking);
                    }
                    else if (CurrentState == ControllerState.Sliding)
                    {
                        Slide.End(ControllerState.Sprinting);
                    }
                    else if (CanDo)
                        Do();
                }
            }

            /// <summary>
            /// can jum ?
            /// </summary>
            public bool CanDo
            {
                get
                {
                    if (Constraints.Jump)
                    {
                        if (!MaxedCount)
                        {
                            if (OnGround)
                                return true;
                            else if (Constraints.JumpFromAir || count != 0)
                                return true;
                        }
                    }

                    return false;
                }
            }
            /// <summary>
            /// jump
            /// </summary>
            public virtual void Do()
            {
                count++;
                power.SetValueToMax();

                if (Controller.OnGround && GroundCast.Rigidbody && !GroundCast.Rigidbody.isKinematic)
                {
                    startVelocity = GroundCast.Rigidbody.velocity.y;

                    if (startVelocity < 0f)
                        startVelocity = 0f;

                    GroundCast.Rigidbody.AddForceAtPosition(Vector3.down * Power.Max * PushDownMultiplier, GroundCast.Hit.point, ForceMode.VelocityChange);
                }
                else
                {
                    startVelocity = 0f;
                }

                Sound.PlayJumpSound();

                Controller.InvokeOnJumpStart();
            }
            /// <summary>
            /// end of jump
            /// </summary>
            protected virtual void End()
            {
                Controller.InvokeOnJumpEnd();
            }
        }

        /// <summary>
        /// checks if the controller is on ground and the specifics of the ground (rigidbody, slope angle, ...)
        /// </summary>
        [SerializeField]
        protected FPController.GroundCastModule groundCast;
        public FPController.GroundCastModule GroundCast { get { return groundCast; } }
        /// <summary>
        /// the base ground cast module, will process ground data and determine if the controller is grounded
        /// </summary>
        [Serializable]
        public class BaseGroundCastModule : FPController.CastModule
        {
            public override Vector3 BaseStart { get { return Transform.position; } }
            public override Vector3 Direction { get { return Vector3.down; } }

            /// <summary>
            /// the angle of the slope the player is standing on, based on the up vector, also defines the maximum slope that can be walked on
            /// </summary>
            [SerializeField]
            protected SlopeValue slope = new SlopeValue(50f);
            public SlopeValue Slope { get { return slope; } }
            [Serializable]
            public class SlopeValue : MaxFloatValue
            {
                public virtual bool TooBig { get { return value > max; } }

                public override float Value
                {
                    get
                    {
                        return base.Value;
                    }
                    set
                    {
                        this.value = value;
                    }
                }

                public SlopeValue(float max) : base(0, max)
                {

                }
            }

            /// <summary>
            /// the angle of the slope the player is standing based on the movement direction of the player, 0 if not moving
            /// </summary>
            [SerializeField]
            protected float directionalSlope;
            public float DirectionalSlope { get { return directionalSlope; } }

            /// <summary>
            /// variable describing if the controller is grounded
            /// </summary>
            [SerializeField]
            protected bool grounded;
            public bool Grounded { get { return grounded; } }

            /// <summary>
            /// minimum distance that will trigger a fall event
            /// </summary>
            [SerializeField]
            protected float minFallDistance = 0.2f;
            public float MinFallDistance { get { return minFallDistance; } }

            /// <summary>
            /// data from the last landing, will be also sent from the on landed event of the controller
            /// holds useful data (travel distance, fall distance, fall position, ...)
            /// </summary>
            protected ControllerLandingData landing;
            public ControllerLandingData Landing { get { return landing; } }
            protected ControllerLandingData currentLanding;

            public override void Init(FPController controller)
            {
                base.Init(controller);

                Controller.OnJumpEnd += JumpEnd;
            }

            public override bool Process()
            {
                bool newOnGround = base.Process();

                if (newOnGround && (Jump.Power.Value > 0f || slope.TooBig))
                    newOnGround = false;

                if (newOnGround && !grounded)
                    Landed();
                if (!newOnGround && grounded)
                    Left();

                return grounded;
            }
            protected override bool ProcessHit()
            {
                if (base.ProcessHit())
                {
                    CalculateDirectionalSlope();

                    CalculateSlope();

                    return true;
                }

                return false;
            }
            protected virtual void CalculateSlope()
            {
                slope.Value = Vector3.Angle(normal, Vector3.up);
            }
            protected virtual void CalculateDirectionalSlope()
            {
                Vector3 direction = Vector3.forward * Movement.Speed.Walk.Value +
                        Vector3.right * Movement.Speed.Strafe.Value;

                direction.Normalize();

                direction = Transform.TransformDirection(direction);

                directionalSlope = Vector3.Angle(normal, direction) - 90;
            }

            protected override void ProcessMiss()
            {
                slope.Value = 0f;

                base.ProcessMiss();
            }

            /// <summary>
            /// left ground
            /// </summary>
            protected virtual void Left()
            {
                grounded = false;

                currentLanding = new ControllerLandingData(Transform.position);

                Controller.InvokeOnLeftGround();
            }
            /// <summary>
            /// landed on ground
            /// </summary>
            protected virtual void Landed()
            {
                grounded = true;

                Sound.PlayLandingSound();

                currentLanding.SetDistanceData(Transform.position);

                if (Mathf.Abs(currentLanding.Distances.Travel) > minFallDistance)
                {
                    landing = currentLanding;

                    Controller.InvokeOnLanded(currentLanding);
                }
            }
            /// <summary>
            /// subscriber to the OnJumpEnd event
            /// </summary>
            protected virtual void JumpEnd()
            {
                currentLanding.Jumped(Transform.position);
            }
        }
        /// <summary>
        /// controller landing data, holds landing related data, travel distance, fall position, ....
        /// </summary>
        [Serializable]
        public class ControllerLandingData
        {
            public int JumpCount { get; protected set; }

            [SerializeField]
            protected PositionsData positions;
            public PositionsData Positions { get { return positions; } }
            [Serializable]
            public class PositionsData
            {
                public Vector3 Leave { get; protected set; }

                public Vector3 Fall { get; set; }

                public PositionsData(Vector3 position)
                {
                    this.Leave = position;
                    this.Fall = position;
                }
            }

            [SerializeField]
            protected DistancesData distances;
            public DistancesData Distances { get { return distances; } }
            public virtual void SetDistanceData(Vector3 currentPosition)
            {
                distances = new DistancesData(positions, currentPosition);
            }
            [Serializable]
            public class DistancesData
            {
                public float Fall { get; protected set; }

                public float Travel { get; protected set; }

                public DistancesData(PositionsData position, Vector3 currentPosition)
                {
                    Fall = position.Fall.y - currentPosition.y;

                    Travel = Vector3.Distance(position.Leave, currentPosition);
                }
            }

            public virtual void Jumped(Vector3 currentPosition)
            {
                JumpCount++;

                positions.Fall = currentPosition;
            }

            public ControllerLandingData(Vector3 position)
            {
                JumpCount = 0;

                this.positions = new PositionsData(position);
            }
        }
        ///is the controller on the ground ?
        public virtual bool OnGround { get { return groundCast.Grounded; } }

        /// <summary>
        /// checks if the controller's head is hitting something
        /// </summary>
        [SerializeField]
        protected FPController.RoofCastModule roofCast;
        public FPController.RoofCastModule RoofCast { get { return roofCast; } }
        /// <summary>
        /// will check if the controller's head hits anything
        /// </summary>
        [Serializable]
        public class BaseRoofCastModule : FPController.CastModule
        {
            public override Vector3 BaseStart { get { return Transform.position + (Direction * Collider.height); } }
            public override Vector3 Direction { get { return Vector3.up; } }
        }

        /// <summary>
        /// controls the sound of the controller (moving, jumping, ....)
        /// </summary>
        [SerializeField]
        protected FPController.SoundModule sound;
        public FPController.SoundModule Sound { get { return sound; } }
        /// <summary>
        /// processes controller sound
        /// </summary>
        [Serializable]
        public class BaseSoundModule : FPController.Module
        {
            [SerializeField]
            AudioSource source;
            public AudioSource Source { get { return source; } }

            [SerializeField]
            FPController.SoundModule.MovementModule movement = new FPController.SoundModule.MovementModule();
            new public FPController.SoundModule.MovementModule Movement { get { return movement; } }
            /// <summary>
            /// processes the movement sounds (foot steps)
            /// </summary>
            [Serializable]
            public class BaseMovementModule : FPController.Module
            {
                public virtual float CurrentStepTime { get { return States.GetData(CurrentState).StepTime; } }
                public virtual SoundSet CurrentSoundSet { get { return Sound.CurrentStates.GetData(CurrentState); } }

                [SerializeField]
                protected float stepTime;
                public float StepTime { get { return stepTime; } }

                public override void Init(FPController controller)
                {
                    base.Init(controller);

                    Controller.OnStateChangeStart += OnStateChanged;
                }

                protected virtual void OnStateChanged(ControllerState oldState, ControllerState newState)
                {
                    if (newState == ControllerState.Sliding)
                        Controller.InvokeOnFootStep(PlayRandomSFX());
                }

                public virtual void Process()
                {
                    var stepTimeDelta = Movement.Speed.Magnitude * Time.deltaTime;

                    if (stepTimeDelta == 0f || !OnGround)
                        stepTime = 0f;
                    else
                        stepTime += stepTimeDelta;

                    if (stepTime >= CurrentStepTime)
                    {
                        stepTime = 0f;

                        Controller.InvokeOnFootStep(PlayRandomSFX());
                    }
                }

                public virtual AudioClip PlayRandomSFX()
                {
                    AudioClip clip = null;

                    if (CurrentSoundSet)
                        clip = CurrentSoundSet.RandomClip;

                    Sound.source.PlayOneShot(clip);

                    return clip;
                }
            }

            [SerializeField]
            protected DefaultSoundStates defaultStates;
            public DefaultSoundStates DefaultStates { get { return defaultStates; } }
            [Serializable]
            public class DefaultSoundStates : FPController.Module, IControllerSoundStates
            {
                public SoundSet Walk { get { return States.Walk.Sounds; } }

                public SoundSet Sprint { get { return States.Sprint.Sounds; } }

                public SoundSet Crouch { get { return States.Crouch.Sounds; } }

                public SoundSet Prone { get { return States.Walk.Sounds; } }

                new public SoundSet Slide { get { return base.Slide.Sounds; } }

                [SerializeField]
                SoundSet jump;
                new public SoundSet Jump { get { return jump; } }

                [SerializeField]
                SoundSet land;
                public SoundSet Land { get { return land; } }

                SoundSet IControllerStatesDataTemplate<SoundSet>.GetData(ControllerState state)
                {
                    return ControllerStatesDataTemplate<SoundSet>.GetData(this, state);
                }
            }

            /// <summary>
            /// override the default states
            /// </summary>
            public ControllerSoundStates OverrideStates { get; set; }

            public IControllerSoundStates CurrentStates
            {
                get
                {
                    if (OverrideStates)
                        return OverrideStates;

                    return defaultStates;
                }
            }

            public override void Init(FPController controller)
            {
                base.Init(controller);

                defaultStates.Init(controller);
                movement.Init(controller);
            }

            public virtual void Process()
            {
                this.movement.Process();
            }

            public virtual void PlayJumpSound()
            {
                if (CurrentStates.Jump)
                    source.PlayOneShot(CurrentStates.Jump.RandomClip);
            }
            public virtual void PlayLandingSound()
            {
                if (CurrentStates.Land)
                    source.PlayOneShot(CurrentStates.Land.RandomClip);
            }
        }

        /// <summary>
        /// defines the camera rig (pivot & camera), and also a coordinates variable for both pivot & camera that can be modified by other modules and get's applied last
        /// </summary>
        [Space()]
        [SerializeField]
        protected FPController.CameraRigModule cameraRig;
        public FPController.CameraRigModule CameraRig { get { return cameraRig; } }
        /// <summary>
        /// the camera rig module, will handle camera related data & functionality
        /// </summary>
        [Serializable]
        public class BaseCameraRigModule : FPController.Module
        {
            [SerializeField]
            protected Transform pivot;
            public Transform Pivot { get { return pivot; } }

            [SerializeField]
            protected Camera camera;
            public Camera Camera { get { return camera; } }
            public Transform CameraTransform { get { return camera.transform; } }

            public override void Init(FPController controller)
            {
                base.Init(controller);

                coordinates = States.Traverser.GetCurrentCameraCoordinates();
                ApplyCoordinates();
            }

            public virtual void ApplyCoordinates()
            {
                pivot.ApplyCoords(coordinates.Pivot, Space.Self);
                CameraTransform.ApplyCoords(coordinates.Camera, Space.Self);
            }

            [SerializeField]
            protected CoordinatesData coordinates;
            public CoordinatesData Coordinates { get { return coordinates; } }
            [Serializable]
            public class CoordinatesData
            {
                [SerializeField]
                Coordinates pivot;
                public Coordinates Pivot { get { return pivot; } set { pivot = value; } }

                [SerializeField]
                Coordinates camera;
                public Coordinates Camera { get { return camera; } set { camera = value; } }

                public CoordinatesData(Vector3 pivot, Vector3 camera)
                {
                    this.pivot = new Coordinates(pivot, Quaternion.identity);
                    this.camera = new Coordinates(camera, Quaternion.identity);
                }
                public CoordinatesData(Coordinates pivot, Coordinates camera)
                {
                    this.pivot = pivot;
                    this.camera = camera;
                }
                public CoordinatesData(Transform pivot, Transform camera)
                {
                    this.pivot = pivot.GetCoords();
                    this.camera = camera.GetCoords();
                }
            }
        }

        /// <summary>
        /// defines the look data & functionality (sensitivity, smoothnes, invert axis, ...)
        /// </summary>
        [SerializeField]
        protected FPController.LookModule look;
        public FPController.LookModule Look { get { return look; } }
        /// <summary>
        /// handles the controller looking
        /// </summary>
        [Serializable]
        public class BaseControllerLookModule : FPController.Module
        {
            [SerializeField]
            protected FPController.LookModule.ModifiersData modifiers;
            public FPController.LookModule.ModifiersData Modifiers { get { return modifiers; } }
            /// <summary>
            /// looking modifiers (sensitivity, invert axis, smooothnes)
            /// </summary>
            [Serializable]
            public class BasseModifiersData
            {
                [SerializeField]
                protected float sensitivity = 5f;
                public float Sensitivity
                {
                    get
                    {
                        return sensitivity;
                    }
                    set
                    {
                        sensitivity = value;
                    }
                }

                [SerializeField]
                protected float smoothScale = 80f;
                public float SmoothScale { get { return smoothScale; } set { smoothScale = value; } }

                [SerializeField]
                protected bool invertX;
                public bool InverX { get { return invertX; } }
                public float XScale { get { return invertX ? -1f : 1f; } }

                [SerializeField]
                protected bool invertY;
                public bool InverY { get { return invertY; } }
                public float YScale { get { return invertY ? -1f : 1f; } }
            }

            /// <summary>
            /// looking range
            /// </summary>
            [SerializeField]
            protected RangeData range = new RangeData(80f, 80f);
            public RangeData Range { get { return range; } }
            [Serializable]
            public struct RangeData
            {
                [SerializeField]
                [Range(0f, 90f)]
                float up;
                public float Up
                {
                    get
                    {
                        return up;
                    }
                    set
                    {
                        up = Mathf.Clamp(value, 0f, 90f);
                    }
                }

                [SerializeField]
                [Range(0f, 90f)]
                float down;
                public float Down
                {
                    get
                    {
                        return down;
                    }
                    set
                    {
                        down = Mathf.Clamp(value, 0f, 90f);
                    }
                }

                public RangeData(float up, float down) : this()
                {
                    Up = up;
                    Down = down;
                }
            }

            public override void Init(FPController controller)
            {
                base.Init(controller);

                characterRotationTarget = Quaternion.Euler(0, Transform.eulerAngles.y, 0);
            }

            Quaternion cameraRotationTarget = Quaternion.identity;
            Quaternion characterRotationTarget = Quaternion.identity;
            public virtual void Process()
            {
                if (Constraints.Look)
                {
                    characterRotationTarget *= Quaternion.Euler(0f, InputModule.Look.x * modifiers.XScale * modifiers.Sensitivity, 0f);
                    cameraRotationTarget *= Quaternion.Euler(-InputModule.Look.y * modifiers.YScale * modifiers.Sensitivity, 0f, 0f);
                }

                Transform.rotation = Quaternion.Slerp(Transform.rotation, characterRotationTarget,
                    modifiers.SmoothScale * Time.deltaTime);

                CameraRig.Coordinates.Camera.rotation = Quaternion.Slerp(CameraRig.CameraTransform.localRotation, cameraRotationTarget,
                    modifiers.SmoothScale * Time.deltaTime);

                ApplyLookRange();
            }

            Vector3 cameraEuelerRotation;
            protected virtual void ApplyLookRange()
            {
                cameraEuelerRotation = CameraRig.Coordinates.Camera.rotation.eulerAngles;

                cameraEuelerRotation.y = 0f;
                cameraEuelerRotation.z = 0f;

                if (cameraEuelerRotation.x > 180f)
                    cameraEuelerRotation.x -= 360f;

                if (cameraEuelerRotation.x > range.Down)
                {
                    cameraEuelerRotation.x = Mathf.Clamp(cameraEuelerRotation.x, 0, range.Down);

                    cameraRotationTarget = Quaternion.Euler(cameraEuelerRotation);
                }
                else if (cameraEuelerRotation.x < -range.Up)
                {
                    cameraEuelerRotation.x = Mathf.Clamp(cameraEuelerRotation.x, -range.Up, 0f);

                    cameraRotationTarget = Quaternion.Euler(cameraEuelerRotation);
                }

                CameraRig.Coordinates.Camera.rotation = Quaternion.Euler(cameraEuelerRotation);
            }
        }

        /// <summary>
        /// controls the leaning data & functionality
        /// </summary>
        [SerializeField]
        protected FPController.LeanModule lean;
        public FPController.LeanModule Lean { get { return lean; } }
        /// <summary>
        /// processes the lean functionality
        /// </summary>
        [Serializable]
        public class BaseControllerLeanModule : FPController.Module
        {
            /// <summary>
            /// the the maximum lean angle
            /// </summary>
            [SerializeField]
            protected float maxAngle = 40f;
            public float MaxAngle { get { return maxAngle; } }

            /// <summary>
            /// axis value that will be updated with the lean input
            /// </summary>
            [SerializeField]
            protected AxisSmoothValue axis = new AxisSmoothValue(4f, 6f, false);
            public AxisSmoothValue Axis { get { return axis; } }
            /// <summary>
            /// angle value of the max angle and the axis value
            /// </summary>
            public float Angle { get { return axis.Value * maxAngle; } }

            /// <summary>
            /// defines the scale of alignment of the camera to the pivot's angle, 1 will give a fully horizontal aligned camera, 0 will give a leaned view
            /// </summary>
            [SerializeField]
            [Range(0f, 1f)]
            protected float alignScale = 0.8f;
            public float AlignScale { get { return alignScale; } }

            [SerializeField]
            protected LayerMask mask = Physics.AllLayers;
            public LayerMask Mask { get { return mask; } }

            [SerializeField]
            protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
            public QueryTriggerInteraction TriggerInteraction { get { return triggerInteraction; } }

            [SerializeField]
            protected float offset = 0.1f;
            public float Offset { get { return offset; } }

            Vector3 pivotRotation;
            Vector3 cameraRotation;
            public virtual void Process()
            {
                if (Constraints.Lean && InputModule.Lean != 0f)
                {
                    if (LeanCheck(offset))
                        axis.Update(0f);
                    else if (!LeanCheck(offset + 0.1f))
                        axis.Update(InputModule.Lean);
                }
                else
                    axis.Update(0f);

                pivotRotation = CameraRig.Coordinates.Pivot.EulerRotation;
                cameraRotation = CameraRig.Coordinates.Camera.EulerRotation;

                pivotRotation.z = -Angle;
                cameraRotation.z = Angle * alignScale;

                CameraRig.Coordinates.Pivot.EulerRotation = pivotRotation;
                CameraRig.Coordinates.Camera.EulerRotation = cameraRotation;
            }

            public virtual Vector3 Start
            {
                get
                {
                    return CameraRig.Pivot.position;
                }
            }
            public virtual Vector3 End
            {
                get
                {
                    return CameraRig.CameraTransform.position;
                }
            }
            public virtual Vector3 Direction
            {
                get
                {
                    if (InputModule.Lean > 0f)
                        return CameraRig.CameraTransform.right;
                    else if (InputModule.Lean < 0f)
                        return -CameraRig.CameraTransform.right;
                    else
                        return Vector3.zero;
                }
            }

            protected virtual bool LeanCheck(float offset)
            {
                Debug.DrawLine(Start, End + (Direction * offset));

                if (Physics.Linecast(Start, End + (Direction * offset), mask, triggerInteraction))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// controls the headbob data & functionality based on current state defined from a headbob states asset
        /// </summary>
        [SerializeField]
        protected FPController.HeadbobModule headbob;
        public FPController.HeadbobModule Headbob { get { return headbob; } }
        /// <summary>
        /// head bob data & functionality
        /// </summary>
        [Serializable]
        public class BaseControllerHeadbobModule : FPController.Module
        {
            /// <summary>
            /// the scale to multiply headbobing values with
            /// </summary>
            [SerializeField]
            protected float scale = 1f;
            public float Scale { get { return scale; } }

            public DefaultStatesData DefaultStates { get; protected set; }
            public class DefaultStatesData : FPController.Module, IControllerHeadBobStates
            {
                HeadBobData IControllerStatesDataTemplate<HeadBobData>.Walk { get { return States.Walk.HeadBob; } }

                HeadBobData IControllerStatesDataTemplate<HeadBobData>.Sprint { get { return States.Sprint.Headbob; } }

                HeadBobData IControllerStatesDataTemplate<HeadBobData>.Crouch { get { return States.Crouch.HeadBob; } }

                HeadBobData IControllerStatesDataTemplate<HeadBobData>.Prone { get { return States.Prone.HeadBob; } }

                HeadBobData IControllerStatesDataTemplate<HeadBobData>.Slide { get { return Slide.Headbob; } }

                HeadBobData IControllerStatesDataTemplate<HeadBobData>.GetData(ControllerState state)
                {
                    return ControllerStatesDataTemplate<HeadBobData>.GetData(this, state);
                }

                public DefaultStatesData(FPController controller)
                {
                    Init(controller);
                }
            }
            /// <summary>
            /// an asset that defines headbobing data per controller state
            /// </summary>
            [SerializeField]
            protected ControllerHeadBobStates overrideStates;
            public ControllerHeadBobStates OverrideStates { get { return overrideStates; } }

            public IControllerHeadBobStates CurrentStates
            {
                get
                {
                    if (overrideStates)
                        return overrideStates;

                    return DefaultStates;
                }
            }

            /// <summary>
            /// the previous headbob data
            /// </summary>
            public HeadBobData Previous { get { return CurrentStates.GetData(States.Traverser.Previous); } }
            /// <summary>
            /// the current headbob data
            /// </summary>
            public HeadBobData Current { get { return CurrentStates.GetData(States.Traverser.Current); } }

            /// <summary>
            /// apply headbob to camera ?
            /// </summary>

            /// <summary>
            /// headbob value as a vector3, the offset to be applied every frame to give a headbobing effect
            /// </summary>
            [SerializeField]
            protected Vector3 offset;
            public Vector3 Offset { get { return offset; } }

            protected float time;
            public float Time { get { return time; } }

            public override void Init(FPController controller)
            {
                base.Init(controller);

                DefaultStates = new DefaultStatesData(controller);
            }

            public virtual void Process()
            {
                if (Controller.OnGround)
                    UpdateOffset(Movement.Speed.Magnitude * (Constraints.HeadBob ? 1f : 0f), States.Traverser.Lerp.Value);

                Apply(offset);
            }

            protected virtual void UpdateOffset(float inputMagnitude, float lerpScale)
            {
                inputMagnitude = Mathf.Clamp01(inputMagnitude);

                inputMagnitude *= UnityEngine.Time.deltaTime;

                lerpScale = Mathf.Clamp01(lerpScale);

                if (inputMagnitude == 0f)
                    time = Mathf.MoveTowards(time,
                        Mathf.RoundToInt(time),
                        Mathf.Lerp(Previous.Gravity, Current.Gravity, lerpScale) * UnityEngine.Time.deltaTime);
                else
                    time += inputMagnitude *
                        Mathf.Lerp(Previous.Delta, Current.Delta, lerpScale);

                offset.y = (Previous.Evaluate(time) * (scale * (lerpScale * -1 + 1))) + (Current.Evaluate(time) * scale * lerpScale);
            }

            protected virtual void Apply(Vector3 offset)
            {
                CameraRig.Coordinates.Camera.position += offset;
            }

            public virtual HeadBobData GetData(ControllerState state)
            {
                return overrideStates.GetData(state);
            }
        }

        //the current input modulator
        [SerializeField]
        protected FPControllerInputModulator inputModulator;
        public FPControllerInputModulator InputModulator { get { return inputModulator; } }
        //Current input module
        public FPControllerInputModule InputModule { get; protected set; }
        //the rigidbody attached to the controller
        public Rigidbody Rigidbody { get; protected set; }
        //the capsule collider attached to the contoller
        public CapsuleCollider Collider { get; protected set; }

        #region Events
        /// <summary>
        /// multiple events that are pretty self explanitory, some are used internaly by modules to comunicate
        /// </summary>
        public event Action OnLeftGround;
        internal virtual void InvokeOnLeftGround()
        {
            if (OnLeftGround != null)
                OnLeftGround();
        }

        public event Action<ControllerLandingData> OnLanded;
        internal virtual void InvokeOnLanded(ControllerLandingData data)
        {
            if (OnLanded != null)
                OnLanded(data);
        }

        public event Action OnJumpStart;
        internal virtual void InvokeOnJumpStart()
        {
            InvokeOnLeftGround();

            if (OnJumpStart != null)
                OnJumpStart();
        }

        public event Action OnJumpEnd;
        internal virtual void InvokeOnJumpEnd()
        {
            if (OnJumpEnd != null)
                OnJumpEnd();
        }

        public delegate void StateChangeDelegate(ControllerState oldState, ControllerState newState);
        public event StateChangeDelegate OnStateChangeStart;
        internal virtual void InvokeOnStateChangeStart(ControllerState oldState, ControllerState newState)
        {
            if (OnStateChangeStart != null)
                OnStateChangeStart(oldState, newState);
        }

        public event Action<ControllerState> OnStateChangeEnd;
        internal virtual void InvokeOnStateChangeEnd(ControllerState state)
        {
            if (OnStateChangeEnd != null)
                OnStateChangeEnd(state);
        }

        public event Action<AudioClip> OnFootStep;
        internal virtual void InvokeOnFootStep(AudioClip clip)
        {
            if (OnFootStep != null)
                OnFootStep(clip);
        }
        #endregion

        protected virtual void Awake()
        {
            if (initMode == ControllerInitMode.Awake)
                Init();
        }
        protected virtual void Start()
        {
            if (initMode == ControllerInitMode.Start)
                Init();
        }

        /// <summary>
        /// the Init method Initilizes the controller's data (get current input module, get components like rigidbody & collider, ...)
        /// </summary>
        public virtual void Init()
        {
            initilized = true;

            GetComponents();
            GetInputModule();

            InitModules();
        }
        /// <summary>
        /// get the needed components (rigidbody, collider, ...)
        /// </summary>
        protected virtual void GetComponents()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<CapsuleCollider>();
        }
        /// <summary>
        /// get the current input module from the InputModulator
        /// </summary>
        protected virtual void GetInputModule()
        {
            InputModule = inputModulator.GetCurrentModule();
        }
        /// <summary>
        /// initilizes the modules (jump, movement, lean, ...)
        /// </summary>
        protected virtual void InitModules()
        {
            Modules = new FPController.ModuleManager();

            AddModule();

            Modules.Init(This);
        }
        protected virtual void AddModule()
        {
            Modules.Add(movement);
            Modules.Add(states);
            Modules.Add(slide);

            Modules.Add(jump);

            Modules.Add(groundCast);
            Modules.Add(roofCast);

            Modules.Add(Sound);

            Modules.Add(cameraRig);

            Modules.Add(look);
            Modules.Add(lean);
            Modules.Add(headbob);
        }

        protected virtual void Update()
        {
            if (initilized)
                UpdateInternal();
        }
        /// <summary>
        /// will be called from update if the controller has been initilized
        /// </summary>
        protected virtual void UpdateInternal()
        {
            //first update the input module
            UpdateInputModule();

            //then check the ground's state
            groundCast.Process();

            //then process the states module
            states.Process();
            slide.Process();

            //then the jump module
            jump.Process();

            //then process the movement
            movement.Process();

            //processing sound
            sound.Process();

            //process look
            look.Process();

            //process lean
            lean.Process();

            //process headbob
            headbob.Process();

            //and finaly apply the camera rig's coordinates which will get set from the above processed modules
            cameraRig.ApplyCoordinates();
        }
        /// <summary>
        /// method that will update the input module, can be overriden to defined when to update the input module example: dont update the input module when pausing
        /// </summary>
        protected virtual void UpdateInputModule()
        {
            InputModule.UpdateInput();
        }

        /// <summary>
        /// enum to define when to initilize the controller
        /// </summary>
        public enum ControllerInitMode
        {
            Awake, Start, Custom
        }

        /// <summary>
        /// the base module to any controller module, contains properties to all modules and values defined in the controller its self
        /// and should be initilized using the Init method
        /// </summary>
        [Serializable]
        public class BaseModule : MoeLinkedModule<FPController>
        {
            public FPController Controller { get { return Link; } }

            public Transform Transform { get { return Collider.transform; } }
            public FPControllerInputModule InputModule { get { return Controller.InputModule; } }
            public CapsuleCollider Collider { get { return Controller.Collider; } }
            public Rigidbody Rigidbody { get { return Controller.Rigidbody; } }

            public virtual bool OnGround { get { return Controller.OnGround; } }

            public ControllerState CurrentState { get { return Controller.CurrentState; } }
            public ControllerStateData CurrentStateData { get { return Controller.CurrentStateData; } }

            public FPController.ConstraintsData Constraints { get { return Controller.Constraints; } }
            public FPController.MovementModule Movement { get { return Controller.Movement; } }
            public FPController.StatesModule States { get { return Controller.States; } }
            public FPController.SlideModule Slide { get { return Controller.slide; } }
            public FPController.JumpModule Jump { get { return Controller.Jump; } }
            public FPController.GroundCastModule GroundCast { get { return Controller.GroundCast; } }
            public FPController.RoofCastModule RoofCast { get { return Controller.RoofCast; } }
            public FPController.SoundModule Sound { get { return Controller.Sound; } }

            public FPController.CameraRigModule CameraRig { get { return Controller.CameraRig; } }
            public FPController.LookModule Look { get { return Controller.Look; } }

            public virtual void Init(FPController controller)
            {
                SetLink(controller);
            }
        }

        /// <summary>
        /// the base cast module, will act as a base class that holds & process raycasting related data & functionality
        /// </summary>
        [Serializable]
        public abstract class BaseCastModule : FPController.Module
        {
            [SerializeField]
            protected LayerMask mask = Physics.AllLayers;
            public LayerMask Mask { get { return mask; } }

            [SerializeField]
            protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
            public QueryTriggerInteraction TriggerInteraction { get { return triggerInteraction; } }

            [SerializeField]
            protected float checkRange = 0.15f;
            public float CheckRange { get { return checkRange; } }

            [SerializeField]
            [Range(0f, 1f)]
            protected float radiusScale = 0.5f;
            public float RadiusScale { get { return radiusScale; } }
            public float RadiusValue { get { return Collider.radius * radiusScale; } }

            [SerializeField]
            protected float startOffset = 0.1f;
            public float StartOffset { get { return startOffset; } }
            public Vector3 StartOffsetVector { get { return -Direction * startOffset; } }

            [SerializeField]
            protected Vector3 normal = Vector3.zero;
            public Vector3 Normal { get { return normal; } }

            /// <summary>
            /// the rigidbody that was hit by the cast, null if none
            /// </summary>
            [SerializeField]
            protected Rigidbody rigidbody;
            new public Rigidbody Rigidbody { get { return rigidbody; } }

            public abstract Vector3 BaseStart { get; }
            public virtual Vector3 Start { get { return BaseStart + -Direction * (RadiusValue + startOffset); } }
            public abstract Vector3 Direction { get; }

            protected RaycastHit hit;
            public RaycastHit Hit { get { return hit; } }

            [SerializeField]
            protected DebugData debug = new DebugData(Color.green);
            [Serializable]
            public struct DebugData
            {
                [SerializeField]
                bool draw;
                public bool Draw { get { return draw; } }

                [SerializeField]
                Color color;
                public Color Color { get { return color; } }

                public DebugData(Color color)
                {
                    draw = true;
                    this.color = color;
                }
            }

            public virtual bool Process()
            {
                if (debug.Draw)
                    Debug.DrawLine(Start, Start + Direction * checkRange, debug.Color);

                if (Physics.SphereCast(Start, Collider.radius * radiusScale, Direction, out hit, Mathf.Infinity, mask, triggerInteraction))
                {
                    if (ProcessHit())
                        return true;
                }

                ProcessMiss();

                return false;
            }
            protected virtual bool ProcessHit()
            {
                if (hit.distance <= checkRange + startOffset)
                {
                    normal = hit.normal;

                    rigidbody = hit.collider.attachedRigidbody;

                    SoundSurface soundSurface = hit.collider.GetComponent<SoundSurface>();
                    if (soundSurface == null && rigidbody)
                        soundSurface = rigidbody.GetComponent<SoundSurface>();

                    if (soundSurface)
                        Sound.OverrideStates = soundSurface.SoundData;
                    else
                    {
                        TerrainSoundSurface terrainSoundSurface = hit.collider.GetComponent<TerrainSoundSurface>();

                        if (terrainSoundSurface)
                            Sound.OverrideStates = terrainSoundSurface.GetSoundsSet(Transform.position.x, Transform.position.z);
                        else
                            Sound.OverrideStates = null;
                    }

                    return true;
                }

                return false;
            }
            protected virtual void ProcessMiss()
            {
                normal = Vector3.up;
                rigidbody = null;

                Sound.OverrideStates = null;
            }
        }

        public enum ButtonInputMode
        {
            Hold, Toggle
        }
    }

    public partial class FPController : BaseFPController
    {
        [Serializable]
        public partial class ModuleManager : BaseModuleManager
        {

        }

        [Serializable]
        public partial class Module : BaseModule
        {

        }

        [Serializable]
        public partial class ConstraintsData : BaseConstraintsData
        {

        }

        [Serializable]
        public partial class MovementModule : BaseMovementModule
        {
            [Serializable]
            public partial class SpeedData : BaseSpeedModule
            {
                [Serializable]
                public partial class SpeedAxisSmoothValue : BaseSpeedAxisSmoothValue
                {
                    public SpeedAxisSmoothValue(float delta) : base(delta)
                    {

                    }
                }
            }
        }

        [Serializable]
        public partial class StatesModule : BaseStatesModule
        {
            [Serializable]
            public class TraverserModule : BaseTraverserModule
            {

            }
        }

        [Serializable]
        public partial class SlideModule : BaseSlideModule
        {

        }

        [Serializable]
        public partial class JumpModule : BaseJumpModule
        {

        }

        #region Cast Modules
        [Serializable]
        public abstract partial class CastModule : BaseCastModule
        {

        }
        [Serializable]
        public partial class GroundCastModule : BaseGroundCastModule
        {

        }
        [Serializable]
        public partial class RoofCastModule : BaseRoofCastModule
        {

        }
        #endregion

        [Serializable]
        public partial class SoundModule : BaseSoundModule
        {
            [Serializable]
            public partial class MovementModule : BaseMovementModule
            {

            }
        }


        [Serializable]
        public partial class CameraRigModule : BaseCameraRigModule
        {

        }

        [Serializable]
        public partial class LookModule : BaseControllerLookModule
        {
            [Serializable]
            public partial class ModifiersData : BasseModifiersData
            {

            }
        }

        [Serializable]
        public partial class LeanModule : BaseControllerLeanModule
        {

        }

        [Serializable]
        public partial class HeadbobModule : BaseControllerHeadbobModule
        {

        }
    }

    /// <summary>
    /// defines the states the controller can be in
    /// </summary>
    public enum ControllerState
    {
        Walking, Sprinting, Crouching, Proning, Sliding
    }
}