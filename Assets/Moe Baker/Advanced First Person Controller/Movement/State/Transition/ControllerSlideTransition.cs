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
    public abstract partial class ControllerSlideTransitionBase : ControllerStateTransition.Module
    {
        public override bool Control
        {
            get
            {
                return base.Control && 
                    Mathf.Approximately(SprintTimer, minSprintTime) && 
                    Mathf.Approximately(Movement.Speed.Magnitude, State.Sprint.Speed);
            }
        }

        [SerializeField]
        protected float minSprintTime = 0.8f;
        public float MinSprintTime { get { return minSprintTime; } }

        public float SprintTimer { get; protected set; }

        public ControllerMovement Movement { get { return Controller.Movement; } }

        public override ControllerState.IData Data { get { return Movement.Procedure.Ground.Slide.Data; } }

        public override bool Process()
        {
            ProcessSprintTimer();

            if (InputModule.Crouch && Control && TargetState == State.Sprint && Movement.GroundCheck.Grounded)
            {
                Movement.Procedure.Ground.Slide.Begin();
                return true;
            }

            return false;
        }
        
        protected virtual void ProcessSprintTimer()
        {
            SprintTimer = Mathf.MoveTowards(SprintTimer, TargetState == State.Sprint ? minSprintTime : 0f, Time.deltaTime);
        }
    }

    public partial class ControllerSlideTransition : ControllerSlideTransitionBase
    {

    }
}