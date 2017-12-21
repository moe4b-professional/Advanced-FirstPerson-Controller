#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Moe.Tools
{
    [DisallowMultipleComponent]
    public class ShowDirection : MonoBehaviour
    {
        [SerializeField]
        Space space = Space.Self;

        [SerializeField]
        Direction direction;

        [SerializeField]
        Vector3 offset = Vector3.zero;
        [SerializeField]
        Vector3 angle = new Vector3(0, 0, 0);
        public Quaternion quaternionAngle { get { return Quaternion.Euler(Vector3.Scale(Vector3.one, new Vector3(angle.x, angle.y, angle.z))); } }

        [SerializeField]
        float length = 1.5f;

        [SerializeField]
        Color color = Color.green;

        Vector3 startPos;
        Vector3 endPos;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = color;

            Vector3

            startPos = transform.position + transform.TransformDirection(offset);
            endPos = transform.position + transform.TransformDirection(offset) + quaternionAngle * GetDirection(space, direction, transform) * length;

            Gizmos.DrawLine(startPos, endPos);
        }

        Vector3 GetDirection(Space space, Direction direction, Transform transform)
        {
            switch (space)
            {
                case Space.World:
                    switch (direction)
                    {
                        case Direction.Forward:
                            return Vector3.forward;
                        case Direction.Right:
                            return Vector3.right;
                        case Direction.Up:
                            return Vector3.up;
                        case Direction.Backwards:
                            return -Vector3.forward;
                        case Direction.Left:
                            return -Vector3.right;
                        case Direction.Down:
                            return -Vector3.up;
                    }
                    break;

                case Space.Self:
                    switch (direction)
                    {
                        case Direction.Forward:
                            return transform.forward;
                        case Direction.Right:
                            return transform.right;
                        case Direction.Up:
                            return transform.up;
                        case Direction.Backwards:
                            return -transform.forward;
                        case Direction.Left:
                            return -transform.right;
                        case Direction.Down:
                            return -transform.up;
                    }
                    break;
            }

            return Vector3.zero;
        }

        public enum Direction
        {
            Forward, Backwards, Right, Left, Up, Down
        }
    }
}
#endif