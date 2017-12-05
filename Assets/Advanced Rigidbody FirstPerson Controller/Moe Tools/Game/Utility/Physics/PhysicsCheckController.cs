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

namespace Moe.Tools
{
    [Serializable]
    public abstract class BasePhysicsCheckController<TColliders, TRigidbodies>
        where TColliders : ICollection<Collider>, IEnumerable<Collider>, new()
        where TRigidbodies : ICollection<Rigidbody>, IEnumerable<Rigidbody>, new()
    {
        [SerializeField]
        TColliders colliders;
        public TColliders Colliders { get { return colliders; } }

        [SerializeField]
        TRigidbodies rigidbodies;
        public TRigidbodies Rigidbodies { get { return rigidbodies; } }

        public void Add(Collider collider)
        {
            colliders.Add(collider);
        }
        public void Add(Rigidbody rigidbody)
        {
            rigidbodies.Add(rigidbody);
        }

        public void Remove(Collider collider)
        {
            colliders.Remove(collider);
        }
        public void Remove(Rigidbody rigidbody)
        {
            rigidbodies.Remove(rigidbody);
        }

        public bool Contains(Collider collider)
        {
            return colliders.Contains(collider);
        }
        public bool Contains(Rigidbody rigidbody)
        {
            return rigidbodies.Contains(rigidbody);
        }

        public BasePhysicsCheckController()
        {
            colliders = new TColliders();
            rigidbodies = new TRigidbodies();
        }
    }

    [Serializable]
    public class PhysicsCheckController : BasePhysicsCheckController<List<Collider>, List<Rigidbody>>
    {

    }

    public class PhysicsHashCheckController : BasePhysicsCheckController<HashSet<Collider>, HashSet<Rigidbody>>
    {

    }
}