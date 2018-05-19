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
    public abstract partial class ControllerCastBase : FPController.Module
    {
        ControllerCast This { get { return this as ControllerCast; } }

        [SerializeField]
        protected LayerMask mask = Physics.DefaultRaycastLayers;
        public LayerMask Mask { get { return mask; } }

        [SerializeField]
        protected QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal;
        public QueryTriggerInteraction TriggerInteraction { get { return triggerInteraction; } }

        [SerializeField]
        [Range(0f, 1f)]
        protected float radiusScale = 0.5f;
        public float RadiusScale { get { return radiusScale; } }
        public virtual float Radius { get { return collider.radius * radiusScale; } }

        [SerializeField]
        protected float range = 0.1f;
        public float Range { get { return range; } }

        [SerializeField]
        protected float offset = 0.2f;
        public float Offset { get { return offset; } }

        public abstract Vector3 Start { get; }
        public virtual Vector3 RayStart
        {
            get
            {
                return Start + (-Direction * (offset + Radius));
            }
        }

        public abstract Vector3 Direction { get; }

        public ResaultData Resault { get; protected set; }
        [Serializable]
        public class ResaultData
        {
            public RaycastHit hit { get; protected set; }

            public float Distance { get; protected set; }
            public Rigidbody Rigidbody { get; protected set; }

            public ResaultData(ControllerCast module, RaycastHit hit)
            {
                this.hit = hit;

                Rigidbody = hit.collider.attachedRigidbody == null ? null : hit.collider.attachedRigidbody;

                Distance = hit.distance - module.Offset;
            }
        }
        public virtual bool HasResault { get { return Resault != null; } }

        RaycastHit tempHit;
        public virtual void Do()
        {
            if (Do(out tempHit))
                Resault = new ResaultData(This, tempHit);
            else
                Resault = null;
        }

        public virtual bool Do(out RaycastHit hit)
        {
            if (Physics.SphereCast(RayStart, Radius, Direction, out hit, range + offset, mask, triggerInteraction))
                return true;
            else
                return false;
        }
    }

    public abstract partial class ControllerCast : ControllerCastBase
    {
		
	}
}