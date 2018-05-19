using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    public abstract class CheckZone : MonoBehaviour
    {
        [SerializeField]
        protected CollidersCheckData colliders;
        public CollidersCheckData Colliders { get { return colliders; } }
        public bool HasColliders { get { return colliders.HasData; } }
        [Serializable]
        public class CollidersCheckData : CheckData<Collider, ColliderEvent>
        {

        }
        [Serializable]
        public class ColliderEvent : UnityEvent<Collider>
        {

        }

        [SerializeField]
        protected RigidbodysCheckData rigidbodies;
        public RigidbodysCheckData Rigidbodies { get { return rigidbodies; } }
        public bool HasRigidbodies { get { return rigidbodies.HasData; } }
        [Serializable]
        public class RigidbodysCheckData : CheckData<Rigidbody, RigidbodyEvent>
        {

        }
        [Serializable]
        public class RigidbodyEvent : UnityEvent<Rigidbody>
        {

        }

        public abstract bool CheckCollider(Collider collider);

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (CheckCollider(collider))
            {
                colliders.Add(collider);

                if (collider.attachedRigidbody && !rigidbodies.Contains(collider.attachedRigidbody))
                    rigidbodies.Add(collider.attachedRigidbody);
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            if (CheckCollider(collider))
            {
                if (colliders.Contains(collider))
                    colliders.Remove(collider);
                else
                    Debug.LogWarning("Collider " + collider.name + " Exited The Trigger Check Zone But Was Not Registered As An Entrant");

                if (collider.attachedRigidbody && rigidbodies.Contains(collider.attachedRigidbody))
                    rigidbodies.Remove(collider.attachedRigidbody);
            }
        }

        [Serializable]
        public abstract class CheckData<TObject, TEvent>
            where TObject : Object
            where TEvent : UnityEvent<TObject>
        {
            [SerializeField]
            protected List<TObject> list;
            public List<TObject> List { get { return list; } }
            public bool HasData { get { return list.Count != 0; } }

            [SerializeField]
            protected TEvent onAdd;
            public TEvent OnAdd { get { return onAdd; } }

            [SerializeField]
            protected TEvent onRemove;
            public TEvent OnRemove { get { return onRemove; } }

            public virtual void Add(TObject obj)
            {
                if (Contains(obj))
                    Debug.LogError("Trying To Add Duplicate Object " + obj.name);
                else
                {
                    list.Add(obj);

                    onAdd.Invoke(obj);
                }
            }

            public virtual void Remove(TObject obj)
            {
                if (Contains(obj))
                {
                    list.Remove(obj);

                    onRemove.Invoke(obj);
                }
                else
                    Debug.LogError("Trying To Remove Non Registered Object " + obj.name);
            }

            public virtual bool Contains(TObject obj)
            {
                return list.Contains(obj);
            }
        }
    }
}