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

public class FPController : MonoBehaviour
{
    [SerializeField]
    bool initOnStart = true;

    [SerializeField]
    ControllerConstraints constraints;
    public ControllerConstraints Constraints { get { return constraints; } }

    [Header("Movement")]
    [SerializeField]
    ControllerStates states;
    public ControllerStates States { get { return states; } }

    public virtual ControllerState CurrentState
    {
        get
        {
            return states.Traverser.State;
        }
    }
    public virtual ControllerStateData CurrentStateData
    {
        get
        {
            return states.Traverser.Current;
        }
    }

    [SerializeField]
    MaxSmoothValue speed = new MaxSmoothValue(0f, 5f);

    [SerializeField]
    ControllerMoveInput moveInput;
    public ControllerMoveInput MoveInput { get { return moveInput; } }

    [SerializeField]
    ControllelrCastData castData;
    public ControllelrCastData CastData { get { return castData; } }
    public bool OnGround
    {
        get
        {
            return castData.Ground.grounded;
        }
        protected set
        {
            castData.Ground.grounded = value;
        }
    }

    [SerializeField]
    ControllerJumpData jumpData;
    public ControllerJumpData JumpData { get { return jumpData; } }

    [SerializeField]
    ControllerLandingData landingData;
    public ControllerLandingData LandingData { get { return landingData; } }

    [SerializeField]
    ControllerInAirData inAirData;
    public ControllerInAirData InAirData { get { return inAirData; } }

    [SerializeField]
    ControllerSoundData soundData;
    public ControllerSoundData SoundData { get { return soundData; } }

    [Header("Look")]
    [SerializeField]
    ControllerLookModifiers lookModifiers;
    public ControllerLookModifiers Modifiers { get { return lookModifiers; } }

    [SerializeField]
    LookRange lookRange = new LookRange(80, 80f);
    public LookRange LookRange { get { return lookRange; } }

    [SerializeField]
    CameraRig cameraRig;
    public CameraRig CameraRig { get { return cameraRig; } }

    LookCoord lookCoord;
    [Serializable]
    public struct LookCoord
    {
        public Vector3 pivotPosition;
        public Vector3 cameraPosition;

        public Quaternion cameraRotation;

        public Quaternion characterRotation;
    }

    [SerializeField]
    ControllerLeanData leanData;
    public ControllerLeanData LeanData { get { return leanData; } }

    [SerializeField]
    ControllerHeadBobData headBobData;
    public ControllerHeadBobData HeadBobData { get { return headBobData; } }

    public Rigidbody Rigidbody { get; protected set; }
    public CapsuleCollider CapsuleCollider { get; protected set; }

    FPControllerInput input;
    public FPControllerInput Input { get { return input; } }

    #region Events
    public event Action OnLeftGround;
    public event Action<ControllerLandingData> OnLanded;

    public event Action OnJumpStart;
    public event Action OnJumpEnd;

    public event Action<ControllerState> OnStateChangeStart;
    public event Action<ControllerState> OnStateChangeEnd;
    #endregion

    protected virtual void Start()
    {
        if (initOnStart)
            Init();
        else
            enabled = false;
    }

    public virtual void Init()
    {
        enabled = true;

        Rigidbody = GetComponent<Rigidbody>();
        CapsuleCollider = GetComponent<CapsuleCollider>();

        input = GetComponent<FPControllerInput>();

        states.Init(this);

        castData.Ground.grounded = GroundCastCheck();

        headBobData.UpdateStates(states.Traverser.Previous.State, states.Traverser.Target.State);
        soundData.Init(this);

        cameraRotationTarget = cameraRig.CameraTransform.localRotation;
        characterRotationTarget = transform.localRotation;
    }

    protected virtual void Update()
    {
        CheckOnGround();

        ProccessInput();

        UpdateSpeed();

        CheckState();
        UpdateState();

        Sounds();

        Look();

        HeadBob();

        ApplyLookCoord();
    }

    protected virtual void ProccessInput()
    {
        input.UpdateInput();

        moveInput.Update(input.Walk, input.Strafe);

        if ((OnGround && input.Move.magnitude == 0f))
            CapsuleCollider.material.dynamicFriction = 1f;
        else
            CapsuleCollider.material.dynamicFriction = 0f;
    }

    protected virtual void UpdateSpeed()
    {
        speed.SetMax(CurrentStateData.Speed);

        if (input.Move.magnitude > 0f)
            speed.MoveTowardsMax();
        else
            speed.MoveTowardsMin();
    }

    #region Ground Check
    protected virtual void CheckOnGround()
    {
        bool newOnGround = GroundCastCheck();

        if (newOnGround && jumpData.Power.Value > 0f)
            newOnGround = false;

        if (newOnGround && !OnGround)
            Landed();
        if (!newOnGround && OnGround)
            LeftGround();
    }
    Vector3 groundCheckStart;
    Vector3 groundCheckEnd;
    protected virtual bool GroundCastCheck()
    {
        groundCheckStart = transform.position + Vector3.up * castData.Ground.Offfset;

        groundCheckEnd = groundCheckStart + (Vector3.down * (castData.Ground.CheckRange + castData.Ground.Offfset));

        Debug.DrawLine(groundCheckStart, groundCheckEnd, Color.green);

        if (Physics.SphereCast(transform.position + Vector3.up * (CapsuleCollider.radius * castData.Ground.RadiusScale + castData.Ground.Offfset),
                CapsuleCollider.radius * castData.Ground.RadiusScale,
                Vector3.down,
                out castData.Ground.hit,
                Mathf.Infinity,
                castData.Ground.Mask,
                castData.Ground.TriggerInteraction
                ))
        {
            if (castData.Ground.hit.distance <= castData.Ground.CheckRange + castData.Ground.Offfset)
            {
                castData.Ground.normal = castData.Ground.hit.normal;
                castData.Ground.slope = Vector3.Angle(castData.Ground.normal, Vector3.up);

                castData.Ground.rigidbody = castData.Ground.hit.collider.attachedRigidbody;

                SoundSurface soundSurface = castData.Ground.hit.collider.GetComponent<SoundSurface>();
                if (soundSurface == null && castData.Ground.rigidbody)
                    soundSurface = castData.Ground.rigidbody.GetComponent<SoundSurface>();

                if (soundSurface)
                    soundData.OverrideStates = soundSurface.SoundData;
                else
                {
                    TerrainSoundSurface terrainSoundSurface = castData.Ground.hit.collider.GetComponent<TerrainSoundSurface>();

                    if (terrainSoundSurface)
                        soundData.OverrideStates = terrainSoundSurface.GetSoundsSet(transform.position.x, transform.position.z);
                    else
                        soundData.OverrideStates = null;
                }

                return true;
            }
        }

        castData.Ground.normal = Vector3.up;
        castData.Ground.slope = 0f;
        castData.Ground.rigidbody = null;

        soundData.OverrideStates = null;

        return false;
    }
    #endregion

    protected virtual void LeftGround()
    {
        OnGround = false;

        landingData.position.leave = transform.position;
        if (jumpData.Power.Value == 0f)
        {
            landingData.fromJump = false;
            landingData.position.fall = transform.position;
        }

        inAirData.forward = transform.forward;
        inAirData.right = transform.right;

        inAirData.speed = CurrentStateData.Speed;

        if (OnLeftGround != null)
            OnLeftGround();
    }

    protected virtual void Landed()
    {
        OnGround = true;

        jumpData.count = 0;

        CalculateLandingData();

        soundData.PlayLandingSound();

        if (OnLanded != null)
            OnLanded(landingData);
    }
    protected virtual void CalculateLandingData()
    {
        landingData.position.current = transform.position;

        landingData.distance.travel = Vector3.Distance(landingData.position.fall, landingData.position.current);
        landingData.distance.fall = landingData.position.fall.y - landingData.position.current.y;
    }

    #region Roof Check
    Vector3 roofCheckStart;
    Vector3 roofCheckEnd;
    protected virtual bool RoofCastCheck()
    {
        roofCheckStart = transform.position + Vector3.down * castData.Roof.Offfset + Vector3.up * CapsuleCollider.height;

        roofCheckEnd = roofCheckStart + (Vector3.up * (castData.Roof.CheckRange + castData.Roof.Offfset));

        Debug.DrawLine(roofCheckStart, roofCheckEnd, Color.green);

        if (Physics.SphereCast(transform.position + Vector3.up * CapsuleCollider.height + Vector3.down * (CapsuleCollider.radius * castData.Roof.RadiusScale + castData.Roof.Offfset),
                CapsuleCollider.radius * castData.Roof.RadiusScale,
                Vector3.up,
                out castData.Roof.hit,
                Mathf.Infinity,
                castData.Roof.Mask,
                castData.Roof.TriggerInteraction
                ))
        {
            if (castData.Roof.hit.distance <= castData.Roof.CheckRange + castData.Roof.Offfset)
            {
                castData.Roof.normal = castData.Roof.hit.normal;
                castData.Roof.slope = Vector3.Angle(castData.Roof.normal, Vector3.up);

                castData.Roof.rigidbody = castData.Roof.hit.collider.attachedRigidbody;

                return true;
            }
        }

        castData.Roof.normal = Vector3.up;
        castData.Roof.slope = 0f;
        castData.Roof.rigidbody = null;
        return false;
    }
    #endregion

    #region Controller State
    protected virtual void CheckState()
    {
        if (constraints.Control)
        {
            if(input.Jump)
            {
                if(CurrentState == ControllerState.Crouching || CurrentState == ControllerState.Proning)
                {
                    GoToState(ControllerState.Walking);
                }
                else if (Input.Jump && constraints.Control && constraints.Jump)
                {
                    if (OnGround || (jumpData.count < jumpData.MaxCount && (constraints.JumpFromAir || jumpData.count > 0)))
                        Jump();
                }
            }

            if (input.Sprint && constraints.Sprint && CurrentState != ControllerState.Sprinting)
            {
                GoToState(ControllerState.Sprinting);
            }
            if (!input.Sprint && CurrentState == ControllerState.Sprinting)
            {
                GoToState(ControllerState.Walking);
            }

            if(input.Crouch)
            {
                if(CurrentState == ControllerState.Crouching)
                {
                    GoToState(ControllerState.Walking);
                }
                else if(constraints.Crouch)
                {
                    GoToState(ControllerState.Crouching);
                }
            }

            if (input.Prone)
            {
                if (CurrentState == ControllerState.Proning)
                {
                    GoToState(ControllerState.Walking);
                }
                else if (constraints.Prone)
                {
                    GoToState(ControllerState.Proning);
                }
            }
        }
    }

    protected virtual void UpdateState()
    {
        if (states.Traverser.Lerp.Value != 1f)
        {
            if (states.Traverser.Target.Height > states.Traverser.Previous.Height && RoofCastCheck())
                GoToSafeState();

            if (states.Traverser.Lerp.Value != 1f)
            {
                states.Traverser.Lerp.MoveTowardsMax();

                states.Traverser.UpdateCurrentState();

                if (states.Traverser.Lerp.Value == 1f)
                {
                    if (OnStateChangeEnd != null)
                        OnStateChangeEnd(CurrentState);
                }
            }

            AssignStateData();
        }

        UpdateCameraPivotScale();
    }
    protected virtual void AssignStateData()
    {
        CapsuleCollider.height = CurrentStateData.Height;
        CapsuleCollider.radius = CurrentStateData.Radius;
        CapsuleCollider.center = new Vector3(0, CapsuleCollider.height / 2f, 0);
    }

    protected virtual void UpdateCameraPivotScale()
    {
        GetScalerValues(CapsuleCollider.height, states.PivotScale, out lookCoord.pivotPosition.y, out lookCoord.cameraPosition.y);

        lookCoord.pivotPosition.y -= States.CameraOffset / 2f;
        lookCoord.cameraPosition.y -= States.CameraOffset / 2f;
    }

    protected virtual void GoToSafeState()
    {
        if (states.Traverser.Target.Height > states.Traverser.Previous.Height && RoofCastCheck())
        {
            if (states.Crouch.Height < CurrentStateData.Height)
                GoToState(ControllerState.Crouching);
            else if (states.Prone.Height < CurrentStateData.Height)
                GoToState(ControllerState.Proning);
        }
    }
    #endregion

    #region Jump
    protected virtual void Jump()
    {
        jumpData.count++;
        jumpData.Power.SetValueToMax();

        if (OnGround && castData.Ground.rigidbody && !castData.Ground.rigidbody.isKinematic)
        {
            castData.Ground.rigidbody.AddForceAtPosition(Vector3.down * jumpData.Power.Max * jumpData.PushDownMultiplier, castData.Ground.hit.point, ForceMode.VelocityChange);

            jumpData.startVelocity = castData.Ground.rigidbody.velocity.y;

            if (jumpData.startVelocity < 0f)
                jumpData.startVelocity = 0f;
        }
        else
        {
            jumpData.startVelocity = 0f;
        }

        Rigidbody.AddForce(transform.TransformDirection(jumpData.Axis), ForceMode.VelocityChange);

        soundData.PlayJumpSound();

        if(OnGround)
            LeftGround();

        if (OnJumpStart != null)
            OnJumpStart();
    }

    protected virtual void JumpEnd()
    {
        landingData.fromJump = true;
        landingData.position.fall = transform.position;

        if (OnJumpEnd != null)
            OnJumpEnd();
    }
    #endregion

    protected virtual void Sounds()
    {
        soundData.Movement.Update(moveInput.Vector.magnitude * (OnGround ? 1f : 0f));
    }

    #region Look

    Quaternion cameraRotationTarget = Quaternion.identity;
    Quaternion characterRotationTarget = Quaternion.identity;
    protected virtual void Look()
    {
        if (constraints.Control && constraints.Look)
        {
            characterRotationTarget *= Quaternion.Euler(0f, input.Look.x * lookModifiers.XScale * lookModifiers.Sensitivity, 0f);
            cameraRotationTarget *= Quaternion.Euler(-input.Look.y * lookModifiers.YScale * lookModifiers.Sensitivity, 0f, 0f);
        }

        lookCoord.characterRotation = Quaternion.Slerp(transform.rotation, characterRotationTarget,
            lookModifiers.SmoothScale * Time.deltaTime);

        lookCoord.cameraRotation = Quaternion.Slerp(cameraRig.CameraTransform.localRotation, cameraRotationTarget,
            lookModifiers.SmoothScale * Time.deltaTime);

        Lean();
    }

    void Lean()
    {
        if(LeanCheck(leanData.Offset * Mathf.Sign(input.Lean)))
        {
            leanData.Update(0f);
        }
        else if(constraints.Control && constraints.Lean)
        {
            leanData.Update(input.Lean);
        }

        LeanData.pivotRotation.z = leanData.Angle.Value;
        cameraRig.Pivot.transform.localEulerAngles = leanData.pivotRotation;

        leanData.cameraRotation = lookCoord.cameraRotation.eulerAngles;
        leanData.cameraRotation.z = -leanData.Angle.Value * leanData.AlignScale;
        lookCoord.cameraRotation.eulerAngles = leanData.cameraRotation;
    }

    bool LeanCheck(float offset)
    {
        Debug.DrawLine(cameraRig.Pivot.position, cameraRig.CameraTransform.position);

        if(Physics.Linecast(cameraRig.Pivot.position, cameraRig.CameraTransform.position + transform.right * offset, leanData.Mask, leanData.TriggerInteraction))
        {
            return true;
        }

        return false;
    }

    protected virtual void ApplyLookCoord()
    {
        ApplyLookRange();

        cameraRig.Pivot.localPosition = lookCoord.pivotPosition;

        cameraRig.CameraTransform.localPosition = lookCoord.cameraPosition;
        cameraRig.CameraTransform.localRotation = lookCoord.cameraRotation;

        transform.localRotation = lookCoord.characterRotation;
    }

    Vector3 cameraEuelerRotation;
    protected virtual void ApplyLookRange()
    {
        cameraEuelerRotation = lookCoord.cameraRotation.eulerAngles;

        if (cameraEuelerRotation.x > 180f)
            cameraEuelerRotation.x -= 360f;

        if (cameraEuelerRotation.x > lookRange.Down)
        {
            cameraEuelerRotation.x = Mathf.Clamp(cameraEuelerRotation.x, 0, lookRange.Down);

            cameraRotationTarget = Quaternion.Euler(cameraEuelerRotation);
        }
        else if(cameraEuelerRotation.x < -lookRange.Up)
        {
            cameraEuelerRotation.x = Mathf.Clamp(cameraEuelerRotation.x, -lookRange.Up, 0f);

            cameraRotationTarget = Quaternion.Euler(cameraEuelerRotation);
        }

        lookCoord.cameraRotation.eulerAngles = cameraEuelerRotation;
    }
    #endregion

    protected virtual void HeadBob()
    {
        lookCoord.cameraPosition.x = 0f;
        lookCoord.cameraPosition.z = 0f;

        if(OnGround)
            headBobData.Update(moveInput.Vector.magnitude * (constraints.HeadBob ? 1f : 0f), states.Traverser.Lerp.Value);

        if(headBobData.ToCamera)
            lookCoord.cameraPosition += headBobData.Offset;
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }
    
    #region Move
    Vector3 velocity;
    protected virtual void Move()
    {
        if (OnGround && castData.Ground.slope <= castData.Ground.MaxSlope)
            GroundMovement();
        else
            AirMovement();

        ApplyForce();
    }

    protected virtual void GroundMovement()
    {
        if (constraints.Control && constraints.Move)
        {
            velocity = Vector3.forward * moveInput.WalkValue + Vector3.right * moveInput.StrafeValue;

            if (velocity.magnitude > 1f)
                velocity.Normalize();
        }
        else
            velocity = Vector3.zero;

        velocity *= speed.Value;
        velocity = transform.TransformDirection(velocity);

        velocity = Vector3.ProjectOnPlane(velocity, castData.Ground.normal);

        if (!castData.Ground.rigidbody && Rigidbody.velocity.y < 0f)
        {
            velocity.y = Mathf.InverseLerp(-castData.Ground.MaxSlope, castData.Ground.MaxSlope, castData.Ground.slope) * (Physics.gravity.y * 0.4f);
        }

        #region Draw Direction
#if UNITY_EDITOR
        Vector3 direction = velocity.normalized;
        if (direction == Vector3.zero)
            direction = Vector3.ProjectOnPlane(transform.forward, castData.Ground.normal);

        Debug.DrawRay(transform.position, direction, Color.yellow);
#endif
        #endregion

        if(castData.Ground.rigidbody && !castData.Ground.rigidbody.isKinematic)
        {
            velocity.y = Rigidbody.velocity.y;
        }
    }

    protected virtual void AirMovement()
    {
        velocity = Rigidbody.velocity;

        velocity = transform.InverseTransformDirection(velocity);
        velocity.y = 0f;

        velocity = Vector3.MoveTowards(velocity, Vector3.zero, inAirData.DeAcceleration * Time.fixedDeltaTime);

        if (constraints.Control && constraints.Move)
        {
            inAirData.velocity = Vector3.forward * moveInput.WalkValue + Vector3.right * moveInput.StrafeValue;

            if (inAirData.velocity.magnitude > 1f)
                inAirData.velocity.Normalize();
        }
        else
            inAirData.velocity = Vector3.zero;

        velocity += inAirData.velocity * inAirData.Control * Mathf.InverseLerp(inAirData.speed, 0, velocity.magnitude);

        if (jumpData.Power.Value > 0f) //Jumping
        {
            if(RoofCastCheck() && (!castData.Roof.rigidbody || castData.Roof.rigidbody.isKinematic))
            {
                Rigidbody.useGravity = true;

                JumpData.Power.SetValueToMin();
            }
            else
            {
                Rigidbody.useGravity = false;

                velocity.y = jumpData.Power.Value + jumpData.startVelocity * jumpData.Axis.y;

                jumpData.Power.MoveTowardsMin();
            }

            if(jumpData.Power.Value == 0f)
                JumpEnd();
        }
        else //Falling
        {
            Rigidbody.useGravity = true;

            velocity.y = Rigidbody.velocity.y;
        }

        velocity = transform.TransformDirection(velocity);
    }

    protected virtual void ApplyForce()
    {
        Rigidbody.velocity = velocity;
    }
    #endregion

    public virtual void GoToState(ControllerState state)
    {
        GoToState(states.GetData(state));
    }
    public virtual void GoToState(ControllerStateData stateData)
    {
        states.Traverser.GoToState(stateData);

        headBobData.UpdateStates(states.Traverser.Previous.State, states.Traverser.Target.State);

        if (OnStateChangeStart != null)
            OnStateChangeStart(CurrentState);
    }

    public static void GetScalerValues(float maxValue, float scale, out float value1, out float value2)
    {
        scale = Mathf.Clamp01(scale);

        value1 = maxValue * scale;
        value2 = maxValue - value1;
    }
}

[Serializable]
public class ControllerSoundData
{
    [SerializeField]
    AudioSource source;
    public AudioSource Source { get { return source; } }

    MovementSoundData movement = new MovementSoundData();
    public MovementSoundData Movement { get { return movement; } }
    [Serializable]
    public class MovementSoundData
    {
        public ControllerSoundStates.SetData SetData { get { return soundData.CurrentSet.GetData(soundData.CurrentState); } }

        [SerializeField]
        float stepTime;
        public float StepTime { get { return stepTime; } }

        ControllerSoundData soundData;

        public void Init(ControllerSoundData soundData, ControllerState currentState)
        {
            this.soundData = soundData;
        }

        public void Update(float magnitude)
        {
            magnitude = Mathf.Clamp01(magnitude);

            if (magnitude == 0f)
                stepTime = 0f;
            else
                stepTime += Time.deltaTime * magnitude;

            if (stepTime >= SetData.StepInterval)
            {
                if (SetData.Set)
                    soundData.source.PlayOneShot(SetData.Set.RandomAudioClip);

                stepTime = 0f;
            }
        }
    }

    [SerializeField]
    ControllerSoundStates defaultStates;
    public ControllerSoundStates DefaultStates { get { return defaultStates; } }

    [SerializeField]
    ControllerSoundStates overrideStates;
    public ControllerSoundStates OverrideStates
    {
        get
        {
            return overrideStates;
        }
        set
        {
            overrideStates = value;
        }
    }

    public ControllerSoundStates CurrentSet
    {
        get
        {
            return overrideStates == null ? defaultStates : overrideStates;
        }
    }

    public ControllerState CurrentState { get { return FPController.CurrentState; } }

    public FPController FPController { get; protected set; }

    public void Init(FPController fpController)
    {
        this.FPController = fpController;

        movement.Init(this, FPController.States.StartingState);
    }

    public void PlayJumpSound()
    {
        if(CurrentSet.Jump)
            source.PlayOneShot(CurrentSet.Jump.RandomAudioClip);
    }
    public void PlayLandingSound()
    {
        if(CurrentSet.Landing)
            source.PlayOneShot(CurrentSet.Landing.RandomAudioClip);
    }
}

[Serializable]
public class ControllerHeadBobData
{
    [SerializeField]
    float scale = 1f;
    public float Scale { get { return scale; } }

    [SerializeField]
    ControllerHeadBobStates data;
    public ControllerHeadBobStates Data { get { return data; } }

    HeadBobData previous;
    public HeadBobData Previous { get { return previous; } }
    HeadBobData current;
    public HeadBobData Current { get { return current; } }

    [SerializeField]
    bool toCamera = true;
    public bool ToCamera { get { return toCamera; } set { toCamera = value; } }

    [SerializeField]
    Vector3 offset;
    public Vector3 Offset { get { return offset; } }

    [SerializeField]
    float time;

    public void UpdateStates(ControllerState previous, ControllerState current)
    {
        this.previous = GetData(previous);
        this.current = GetData(current);
    }

    public void Update(float moveInputMagnitude, float statesLerpScale)
    {
        moveInputMagnitude = Mathf.Clamp01(moveInputMagnitude);

        moveInputMagnitude *= Time.deltaTime;

        statesLerpScale = Mathf.Clamp01(statesLerpScale);

        if (moveInputMagnitude == 0f)
            time = Mathf.MoveTowards(time,
                Mathf.RoundToInt(time),
                Mathf.Lerp(previous.Gravity, current.Gravity, statesLerpScale) * Time.deltaTime);
        else
            time += moveInputMagnitude * 
                Mathf.Lerp(previous.Delta, current.Delta, statesLerpScale);

        offset.y = (previous.Evaluate(time) * (scale * (statesLerpScale * -1 + 1))) + (current.Evaluate(time) * scale * statesLerpScale);
    }

    public HeadBobData GetData(ControllerState state)
    {
        return data.GetData(state);
    }
}

[Serializable]
public struct ControllerLandingData
{
    public bool fromJump;

    public PositionsData position;
    [Serializable]
    public struct PositionsData
    {
        public Vector3 leave;

        public Vector3 fall;

        public Vector3 current;
    }

    public DistanceData distance;
    [Serializable]
    public struct DistanceData
    {
        public float fall;

        public float travel;
    }
}

[Serializable]
public class ControllerLeanData
{
    [SerializeField]
    RangedSmoothValue angle = new RangedSmoothValue(-30f, 30f, 50f);
    public RangedSmoothValue Angle { get { return angle; } }

    [SerializeField]
    [Range(0f, 1f)]
    float alignScale = 0.8f;
    public float AlignScale { get { return alignScale; } }

    [SerializeField]
    LayerMask mask = Physics.AllLayers;
    public LayerMask Mask { get { return mask; } }

    [SerializeField]
    QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
    public QueryTriggerInteraction TriggerInteraction { get { return triggerInteraction; } }

    [SerializeField]
    float offset = 0.1f;
    public float Offset { get { return offset; } }

    public Vector3 pivotRotation;
    public Vector3 cameraRotation;

    public void Update(float scale)
    {
        if (scale == 0f)
            angle.MoveTowards(0f);
        else
        {
            if (scale > 0f)
                angle.MoveTowardsMin();
            else
                angle.MoveTowardsMax();
        }
    }
}

[Serializable]
public class ControllerLookModifiers
{
    [SerializeField]
    float sensitivity = 3f;
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
    float smoothScale = 80f;
    public float SmoothScale { get { return smoothScale; } set { smoothScale = value; } }

    [SerializeField]
    bool invertX;
    public bool InverX { get { return invertX; } }
    public float XScale { get { return invertX ? -1f : 1f; } }

    [SerializeField]
    bool invertY;
    public bool InverY { get { return invertY; } }
    public float YScale { get { return invertY ? -1f : 1f; } }
}

[Serializable]
public struct LookRange
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

    public LookRange(float up, float down) : this()
    {
        Up = up;
        Down = down;
    }
}

[Serializable]
public class ControllerInAirData
{
    [SerializeField]
    [Range(0f, 1f)]
    float control = 0.25f;
    public float Control { get { return control; } }

    [SerializeField]
    float deAcceleration;
    public float DeAcceleration { get { return deAcceleration; } }

    public Vector3 forward;
    public Vector3 right;
    public Vector3 velocity;

    public float speed;
}

[Serializable]
public class ControllerMoveInput
{
    [SerializeField]
    AxisSmoothValue walk = new AxisSmoothValue(5f);
    public float WalkValue { get { return walk.Value; } }

    [SerializeField]
    AxisSmoothValue strafe = new AxisSmoothValue(5f);
    public float StrafeValue { get { return strafe.Value; } }

    [SerializeField]
    Vector2 vector;
    public Vector2 Vector { get { return vector; } }

    public void Update(float walkScale, float strafeScale)
    {
        walk.Update(walkScale);
        strafe.Update(strafeScale);

        vector.y = walk.Value;
        vector.x = strafe.Value;
    }
}

[Serializable]
public class ControllelrCastData
{
    [SerializeField]
    GroundCastData ground;
    public GroundCastData Ground { get { return ground; } }

    [SerializeField]
    StateData roof;
    public StateData Roof { get { return roof; } }

    [Serializable]
    public class GroundCastData : StateData
    {
        [SerializeField]
        float maxSlope = 50f;
        public float MaxSlope { get { return maxSlope; } }

        public bool grounded;
    }

    [Serializable]
    public class StateData
    {
        [SerializeField]
        LayerMask mask = Physics.AllLayers;
        public LayerMask Mask { get { return mask; } }

        [SerializeField]
        QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        public QueryTriggerInteraction TriggerInteraction { get { return triggerInteraction; } }

        [SerializeField]
        float checkRange = 0.15f;
        public float CheckRange { get { return checkRange; } }

        [SerializeField]
        float offset = 0.1f;
        public float Offfset { get { return offset; } }

        [SerializeField]
        [Range(0f, 1f)]
        float radiusScale = 0.5f;
        public float RadiusScale { get { return radiusScale; } }

        public Ray ray;
        public RaycastHit hit;
        public Vector3 normal = Vector3.up;
        public Rigidbody rigidbody;

        public float slope;
    }
}

[Serializable]
public class ControllerJumpData
{
    [SerializeField]
    MaxSmoothValue power = new MaxSmoothValue(7f, 12f);
    public MaxSmoothValue Power { get { return power; } }

    [SerializeField]
    int maxCount = 1;
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
    [HideInInspector]
    public int count;

    [SerializeField]
    public Vector3 axis = Vector3.up;
    public Vector3 Axis { get { return axis; } set { axis = value; } }
    public Vector3 YAxis { get { return Vector3.Scale(axis, Vector3.up); } }
    public Vector3 XYAxis { get { return Vector3.Scale(axis, Vector3.forward + Vector3.right); } }

    [SerializeField]
    float pushDownMultiplier = 2.5f;
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

    [HideInInspector]
    public float startVelocity;
}

[Serializable]
public class ControllerStates
{
    [SerializeField]
    ControllerState startingState = ControllerState.Walking;
    public ControllerState StartingState { get { return startingState; } set { startingState = value; } }

    [SerializeField]
    ControllerStateData walking = new ControllerStateData(1.8f, 0.35f, 3.5f);
    public ControllerStateData Walking { get { return walking; } }

    [SerializeField]
    ControllerStateData sprint = new ControllerStateData(1.8f, 0.35f, 7);
    public ControllerStateData Sprint { get { return sprint; } }

    [SerializeField]
    ControllerStateData crouch = new ControllerStateData(1f, 0.35f, 1f);
    public ControllerStateData Crouch { get { return crouch; } }

    [SerializeField]
    ControllerStateData prone = new ControllerStateData(0.4f, 0.2f, 0.25f);
    public ControllerStateData Prone { get { return prone; } }

    [SerializeField]
    ControllerStateData custom;
    public ControllerStateData Custom { get { return custom; } set { custom = value; } }

    [Space()]
    [SerializeField]
    StatesTraverser traverser;
    public StatesTraverser Traverser { get { return traverser; } }
    [Serializable]
    public class StatesTraverser
    {
        [SerializeField]
        ControllerState state;
        public ControllerState State { get { return target.State; } }

        [SerializeField]
        ScaleSmoothValue lerp = new ScaleSmoothValue(3.5f);
        public ScaleSmoothValue Lerp { get { return lerp; } }

        ControllerStateData current;
        public ControllerStateData Current { get { return current; } }

        ControllerStateData previous;
        public ControllerStateData Previous { get { return previous; } }

        ControllerStateData target;
        public ControllerStateData Target { get { return target; } }

        FPController fpController;

        public void Init(FPController fpController, ControllerStateData current)
        {
            this.fpController = fpController;

            this.previous = current;
            this.target = current;
            this.lerp.SetValueToMin();

            UpdateCurrentState();
        }

        public void UpdateCurrentState()
        {
            current = ControllerStateData.Lerp(previous, target, lerp.Value);
        }

        public void GoToState(ControllerStateData newState)
        {
            GoToState(newState, Mathf.InverseLerp(target.Height, newState.Height, Current.Height));
        }

        public void GoToState(ControllerStateData newState, float lerpScale)
        {
            lerpScale = Mathf.Clamp01(lerpScale);

            previous = target;
            target = newState;

            lerp.Value = lerpScale;

            UpdateCurrentState();
            current.State = target.State;

            state = target.State;
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    float pivotScale = 0.5f;
    public float PivotScale { get { return pivotScale; } }

    [SerializeField]
    float cameraOffset = 0.1f;
    public float CameraOffset { get { return cameraOffset; } }

    public ControllerStateData GetData(ControllerState state)
    {
        switch (state)
        {
            case ControllerState.Walking:
                return walking;
            case ControllerState.Sprinting:
                return sprint;
            case ControllerState.Crouching:
                return crouch;
            case ControllerState.Proning:
                return prone;
            case ControllerState.Custom:
                return custom;
        }

        throw new ArgumentException("Controller State " + state.ToString() + " Not Defined");
    }

    public FPController FPController { get; protected set; }

    public void Init(FPController fpController)
    {
        this.FPController = fpController;

        walking.State = ControllerState.Walking;
        sprint.State = ControllerState.Sprinting;

        crouch.State = ControllerState.Crouching;
        prone.State = ControllerState.Proning;

        custom.State = ControllerState.Custom;

        traverser.Init(FPController, GetData(startingState));
    }
}

public enum ControllerState
{
    Walking, Sprinting, Crouching, Proning, Custom
}
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

    public ControllerStateData(float height, float radius, float speed)
    {
        this.height = height;
        this.radius = radius;
        this.speed = speed;

        State = ControllerState.Custom;
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

[Serializable]
public class ControllerConstraints
{
    [SerializeField]
    public bool Control = true;

    [SerializeField]
    public bool Move = true;

    [SerializeField]
    public bool Look = true;

    [SerializeField]
    public bool Jump = true;

    [SerializeField]
    public bool JumpFromAir = false;

    [SerializeField]
    public bool Sprint = true;

    [SerializeField]
    public bool Crouch = true;

    [SerializeField]
    public bool Prone = true;

    [SerializeField]
    public bool Lean = true;

    [SerializeField]
    public bool HeadBob = true;
}

[Serializable]
public struct CameraRig
{
    [SerializeField]
    Transform pivot;
    public Transform Pivot { get { return pivot; } }

    [SerializeField]
    Camera camera;
    public Camera Camera { get { return camera; } }
    public Transform CameraTransform { get { return camera.transform; } }
}

public abstract class FPControllerInput : MonoBehaviour
{
    [SerializeField]
    protected Vector2 move;
    public Vector2 Move { get { return move; } }

    [SerializeField]
    public float Walk { get { return move.y; } protected set { move.y = value; } }

    [SerializeField]
    public float Strafe { get { return move.x; } protected set { move.x = value; } }

    [SerializeField]
    protected Vector2 look;
    public Vector2 Look { get { return look; } }

    [SerializeField]
    [Range(-1f, 1f)]
    protected float lean = 0f;
    public float Lean { get { return lean; } }

    [SerializeField]
    protected bool jump;
    public bool Jump { get { return jump; } }

    [SerializeField]
    protected bool sprint;
    public bool Sprint { get { return sprint; } }

    [SerializeField]
    protected bool crouch;
    public bool Crouch { get { return crouch; } }

    [SerializeField]
    protected bool prone;
    public bool Prone { get { return prone; } }

    public abstract void UpdateInput();
}