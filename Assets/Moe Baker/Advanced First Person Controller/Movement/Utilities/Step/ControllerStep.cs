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
	public class ControllerStepBase : FPController.Module
	{
		[SerializeField]
        protected ControllerStepLengthSet lengthSet;
        public ControllerStepLengthSet LengthSet { get { return lengthSet; } }
        public float CurrentStepLength { get { return lengthSet.Get(Controller); } }

        [SerializeField]
        protected float resetSpeed = 4f;
        public float ResetSpeed { get { return resetSpeed; } }

        public float Distance { get; protected set; }
        public float Rate { get; protected set; }

        public virtual void Process(bool calculate)
        {
            var velocity = Vector3.Scale(rigidbody.velocity, Vector3.forward + Vector3.right);

            if (velocity.magnitude > 0f && calculate)
            {
                Distance += velocity.magnitude * Time.deltaTime;

                if (Distance >= CurrentStepLength)
                    Completed();

                Rate = Distance / CurrentStepLength;
            }
            else
            {
                Distance = Mathf.MoveTowards(Distance, 0f, resetSpeed * Time.deltaTime);

                Rate = Mathf.MoveTowards(Rate, 0f, resetSpeed * Time.deltaTime);
            }
        }

        public event Action OnComplete;
        protected virtual void Completed()
        {
            if (Distance < CurrentStepLength) return;

            Distance -= CurrentStepLength;

            if (OnComplete != null)
                OnComplete();
        }
    }

    public class ControllerStep : ControllerStepBase
    {

    }
}