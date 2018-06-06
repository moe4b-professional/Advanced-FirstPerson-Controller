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
	public abstract partial class ControllerCharacterLookBase : FPController.Module
	{
        public ControllerLook Look { get { return Controller.Look; } }

        public ControllerLookTarget LookTarget { get { return Look.LookTarget; } }

        float targetLookScale;
        public virtual float Sensitivity
        {
            get
            {
                return Look.Sensitivity.Horizontal * targetLookScale;
            }
        }


        public virtual void Process()
        {
            ProcessLookTarget();

            ProcessInput();
        }

        protected virtual void ProcessLookTarget()
        {
            if (LookTarget.Position.HasValue)
            {
                var target = GetRotationTo(LookTarget.Position.Value);

                RotateTowards(target, LookTarget.Speed);

                targetLookScale = GetSensitivtyScale(target, LookTarget.Range);
            }
            else
                targetLookScale = 1f;
        }

        protected virtual void ProcessInput()
        {
            Controller.transform.localRotation *=
                Quaternion.Euler(0f, InputModule.Look.x * Look.Control.AbsoluteScale * Sensitivity, 0f);
        }


        public virtual float GetSensitivtyScale(Quaternion targetRotation, float range)
        {
            return MoeTools.Math.InvertScale(Quaternion.Angle(targetRotation, transform.rotation) / range);
        }
        public virtual Quaternion GetRotationTo(Vector3 position)
        {
            var direction = (position - Controller.transform.position).normalized;
            direction.y = 0f;

            return Quaternion.LookRotation(direction);
        }
        public virtual void RotateTowards(Quaternion target, float speed)
        {
            Controller.transform.rotation = 
                Quaternion.RotateTowards(Controller.transform.rotation, target, speed * Time.deltaTime);
        }
	}

    public partial class ControllerCharacterLook : ControllerCharacterLookBase
    {

    }
}