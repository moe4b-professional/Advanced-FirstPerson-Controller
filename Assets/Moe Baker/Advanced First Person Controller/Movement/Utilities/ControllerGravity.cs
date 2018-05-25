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
	public abstract partial class ControllerGravityBase : FPController.Module
	{
        [SerializeField]
        protected float multiplier = 1f;
        public float Multiplier
        {
            get
            {
                return multiplier;
            }
            set
            {
                multiplier = value;
            }
        }

        public override void Init(FPController link)
        {
            base.Init(link);

            rigidbody.useGravity = false;
        }

        public virtual void Apply()
        {
            Apply(multiplier);
        }
        public virtual void Apply(float multiplier)
        {
            rigidbody.AddForce(Physics.gravity * multiplier, ForceMode.Acceleration);
        }
	}

    public partial class ControllerGravity : ControllerGravityBase
    {

    }
}