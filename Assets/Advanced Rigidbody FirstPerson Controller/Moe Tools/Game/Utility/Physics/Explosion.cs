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
    public class Explosion : MonoBehaviour
    {
        [SerializeField]
        ForceData force = new ForceData(10000f);
        public ForceData Force
        {
            get
            {
                return force;
            }
            set
            {
                force = value;
            }
        }
        [Serializable]
        public struct ForceData
        {
            [SerializeField]
            float value;
            public float Value { get { return value; } }

            [SerializeField]
            ForceMode mode;
            public ForceMode Mode { get { return mode; } }

            [SerializeField]
            float upwardsModifier;
            public float UpwardsModifier { get { return upwardsModifier; } }

            public ForceData(float value) : this(value, ForceMode.Force, 0f)
            {

            }
            public ForceData(float value, ForceMode mode, float upwardsModifier)
            {
                this.value = value;
                this.mode = mode;
                this.upwardsModifier = upwardsModifier;
            }
        }

        [Space]
        [SerializeField]
        float radius = 2f;

        [SerializeField]
        LayerMask mask = Physics.AllLayers;
        public LayerMask Mask { get { return mask; } }

        [SerializeField]
        QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        public QueryTriggerInteraction TriggerInteraction { get { return triggerInteraction; } }

        [SerializeField]
        Vector3 offset = Vector3.zero;
        public Vector3 Offset { get { return offset; } }
        public Vector3 WorldOffset
        {
            get
            {
                return transform.localToWorldMatrix * offset;
            }
        }

        [SerializeField]
        PhysicsCheckController ignores;
        public PhysicsCheckController Ignores { get { return ignores; } }

        [SerializeField]
        AudioSource aud;
        public AudioSource Aud { get { return aud; } }

        [SerializeField]
        SoundSet _SFX;
        public SoundSet SFX { get { return _SFX; } }

        [SerializeField]
        ParticleSystem particle;

        public void Init()
        {

        }

        public delegate void AddForceDelegate(Rigidbody rigidbody, float distance);
        public event AddForceDelegate OnAddForce;
        public virtual void Explode()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + WorldOffset, radius, mask, triggerInteraction);

            Stack<Rigidbody> rigidbodies = ExtractHits(colliders);

            foreach (var rb in rigidbodies)
            {
                ProcessRigidbody(rb);

                if (OnAddForce != null)
                    OnAddForce(rb, Vector3.Distance(transform.position, rb.position));
            }

            aud.PlayOneShot(_SFX.RandomClip);
            particle.Play();
        }

        protected virtual Stack<Rigidbody> ExtractHits(Collider[] colliders)
        {
            Stack<Rigidbody> rigidbodies = new Stack<Rigidbody>();

            Rigidbody rigidbody;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (Ignores.Contains(colliders[i])) continue;

                rigidbody = colliders[i].attachedRigidbody;

                if (rigidbody && !rigidbodies.Contains(rigidbody) && !Ignores.Contains(rigidbody))
                    rigidbodies.Push(colliders[i].attachedRigidbody);
            }

            return rigidbodies;
        }

        protected virtual void ProcessRigidbody(Rigidbody rb)
        {
            rb.AddExplosionForce(force.Value, transform.position + WorldOffset, radius, force.UpwardsModifier, force.Mode);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + WorldOffset, radius);
        }
    }
}