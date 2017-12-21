using System;
using System.IO;
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
    public class StateMachineCallbackRewind : UStateMachine
    {
        //State
        public class StateEventData
        {
            public event Action<Animator, AnimatorStateInfo, int> Complex;
            public virtual void InvokeComplex(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                if (Complex != null)
                    Complex(animator, stateInfo, layerIndex);
            }

            public event Action Simple;
            public virtual void InvokeSimple()
            {
                if (Simple != null)
                    Simple();
            }
        }

        protected StateEventData stateEnter = new StateEventData();
        public StateEventData StateEnter { get { return stateEnter; } }
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            stateEnter.InvokeSimple();
            stateEnter.InvokeComplex(animator, stateInfo, layerIndex);
        }

        protected StateEventData stateUpdate = new StateEventData();
        public StateEventData StateUpdate { get { return stateUpdate; } }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            stateUpdate.InvokeSimple();
            stateUpdate.InvokeComplex(animator, stateInfo, layerIndex);
        }

        protected StateEventData stateExit = new StateEventData();
        public StateEventData StateExit { get { return stateExit; } }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            stateExit.InvokeSimple();
            stateExit.InvokeComplex(animator, stateInfo, layerIndex);
        }

        //State Machine
        public class StateMachineEventData
        {
            public event Action<Animator, int> Complex;
            public virtual void InvokeComplex(Animator animator, int stateMachinePathHash)
            {
                if (Complex != null)
                    Complex(animator, stateMachinePathHash);
            }

            public event Action Simple;
            public virtual void InvokeSimple()
            {
                if (Simple != null)
                    Simple();
            }
        }

        protected StateMachineEventData stateMachineEnter = new StateMachineEventData();
        public StateMachineEventData StateMachineEnter { get { return stateMachineEnter; } }
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineEnter(animator, stateMachinePathHash);

            stateMachineEnter.InvokeSimple();
            stateMachineEnter.InvokeComplex(animator, stateMachinePathHash);
        }

        protected StateMachineEventData stateMachineExit;
        public StateMachineEventData StateMachineExit { get { return stateMachineExit; } }
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineExit(animator, stateMachinePathHash);

            stateMachineExit.InvokeSimple();
            stateMachineExit.InvokeComplex(animator, stateMachinePathHash);
        }
    }
}