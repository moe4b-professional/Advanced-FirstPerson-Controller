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
    public abstract partial class ControllerGroundCheckBase : ControllerCast
    {
        public virtual bool Grounded { get { return Resault != null; } }

        public override Vector3 Start { get { return transform.position; } }
        public override Vector3 Direction { get { return Vector3.down; } }

        public override void Init(FPController link)
        {
            base.Init(link);

            Do();
        }

        bool firstDoFlag = false;
        public override void Do()
        {
            var prevGrounded = Grounded;

            base.Do();

            if (firstDoFlag)
                ProcessChange(prevGrounded);
            else
                firstDoFlag = true;
        }

        protected virtual void ProcessChange(bool prevGrounded)
        {
            if (Grounded)
            {
                if (!prevGrounded)
                    LandingAction();
            }
            else
            {
                if (prevGrounded)
                    LeaveAction();
            }
        }

        public event Action OnLanding;
        protected virtual void LandingAction()
        {
            if (OnLanding != null)
                OnLanding();
        }

        public event Action OnLeave;
        protected virtual void LeaveAction()
        {
            if (OnLeave != null)
                OnLeave();
        }
    }

    public partial class ControllerGroundCheck : ControllerGroundCheckBase
    {

    }
}