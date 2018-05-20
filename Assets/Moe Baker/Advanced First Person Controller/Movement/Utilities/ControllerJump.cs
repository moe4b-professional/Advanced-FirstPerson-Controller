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

namespace AFPC
{
    public abstract partial class ControllerJumpBase : FPController.Module
    {
        [SerializeField]
        protected bool _control = true;
        public virtual bool Control
        {
            get
            {
                return Movement.Control && _control && cooldownCoroutine == null;
            }
            set
            {
                _control = value;
            }
        }

        [SerializeField]
        protected float power = 5f;
        public float Power
        {
            get
            {
                return power;
            }
            set
            {
                if(value < 0f)
                {
                    Debug.LogWarning("Cannot set " + typeof(ControllerJump) + "'s " + " power to the negative value of " + power);
                    return;
                }

                power = value;
            }
        }

        public ControllerMovement Movement { get { return Controller.Movement; } }

        public ControllerGroundCheck GroundCheck { get { return Movement.GroundCheck; } }

        public ControllerState State { get { return Movement.State; } }
        public ControllerState.IData TargetState { get { return State.Traverser.Target; } }

        public virtual bool CanDo
        {
            get
            {
                return TargetState == State.Walk || TargetState == State.Sprint;
            }
        }

        public virtual void Process()
        {
            if (InputModule.Jump && Control)
            {
                if (CanDo)
                    Do();
                else if (TargetState == State.Crouch || TargetState == State.Prone)
                {
                    State.Traverser.GoTo(State.Walk);
                }
                else if(TargetState == Movement.Procedure.Ground.Slide.Data)
                {
                    if (State.Transition.Sprint.Control)
                        State.Traverser.GoTo(State.Sprint);
                    else
                        State.Traverser.GoTo(State.Walk);
                }
            }
        }

        public event Action OnDo;
        public virtual void Do()
        {
            ProcessVelocity();

            rigidbody.AddForce(Vector3.up * power, ForceMode.VelocityChange);

            cooldownCoroutine = StartCoroutine(CooldownProcedure());

            if (OnDo != null)
                OnDo();
        }
        protected virtual void ProcessVelocity()
        {
            var velocity = rigidbody.velocity;

            if (GroundCheck.HasResault && GroundCheck.Resault.Rigidbody != null && GroundCheck.Resault.Rigidbody.velocity.y > 0f)
                velocity.y = GroundCheck.Resault.Rigidbody.velocity.y;
            else
                velocity.y = 0f;


            rigidbody.velocity = velocity;
        }

        Coroutine cooldownCoroutine;
        protected virtual IEnumerator CooldownProcedure()
        {
            yield return new WaitForSeconds(0.2f);

            cooldownCoroutine = null;
        }
    }

	public partial class ControllerJump : ControllerJumpBase
    {

	}
}