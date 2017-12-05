using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moe.Tools
{
    public class BoundsCheckArea : MonoBehaviour
    {
        [SerializeField]
        Vector3 size = Vector3.one;
        public Vector3 Size { get { return size; } }

        [SerializeField]
        LayerMask mask = Physics.AllLayers;
        public LayerMask Mask { get { return mask; } }

        [SerializeField]
        PhysicsCheckController ignores = new PhysicsCheckController();
        public PhysicsCheckController Ignores { get { return ignores; } }

        [SerializeField]
        List<Collider> colliders;
        public List<Collider> Colliders { get { return colliders; } }

        [SerializeField]
        List<Rigidbody> rigidBodies;
        public List<Rigidbody> RigidBodies { get { return rigidBodies; } }

        public virtual void CheckArea()
        {
            colliders = Physics.OverlapBox(transform.position, size / 2f, transform.rotation, mask).ToList();
            colliders.RemoveAll(delegate (Collider collider) { return ignores.Contains(collider); });

            rigidBodies.Clear();
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].attachedRigidbody != null && !rigidBodies.Contains(colliders[i].attachedRigidbody))
                    rigidBodies.Add(colliders[i].attachedRigidbody);
            }
            rigidBodies.RemoveAll(delegate (Rigidbody rigidbody) { return ignores.Contains(rigidbody); });
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = new Color(1, 0, 0, 0.5F);

            Gizmos.DrawCube(Vector3.zero, size);
        }
    }
}