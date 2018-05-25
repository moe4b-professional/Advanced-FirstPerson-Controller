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
    public abstract class ControllerInputModule : InputModule
	{
        public const string MenuPath = ControllerInputModulator.MenuPath + "Modules/";

        public Vector2 Move { get; protected set; }

        public bool Jump { get; protected set; }

        public bool Sprint { get; protected set; }

        public bool Crouch { get; protected set; }
        public bool Prone { get; protected set; }

        public Vector2 Look { get; protected set; }
        public int Lean { get; protected set; }

        public override void Clear()
        {
            base.Clear();

            Move = Vector2Int.zero;
            Look = Vector2.zero;
        }
    }

    [Serializable]
    public class AcceleratedCombinedAxis
    {
        [SerializeField]
        protected CombinedAxisData axis;
        public CombinedAxisData Axis { get { return axis; } }

        [SerializeField]
        protected AxisAcceleration acceleration;
        public AxisAcceleration Acceleration { get { return acceleration; } }

        public Vector2 Value { get; protected set; }

        public virtual void Process()
        {
            var input = axis.Value;

            acceleration.Process(input);

            Value = acceleration.Scale(input);
        }

        public AcceleratedCombinedAxis(string axis, float maxAcceleration)
        {
            this.axis = new CombinedAxisData(axis);
            this.acceleration = new AxisAcceleration(maxAcceleration);
        }
        public AcceleratedCombinedAxis(string xAxis, string yAxis, float maxAcceleration)
        {
            this.axis = new CombinedAxisData(xAxis, yAxis);
            this.acceleration = new AxisAcceleration(maxAcceleration);
        }
    }

    [Serializable]
    public class AxisAcceleration
    {
        [SerializeField]
        protected float max = 2f;
        public float Max
        {
            get
            {
                return max;
            }
            set
            {
                if (value < 0f)
                {
                    Debug.LogWarning("Cannot Set " + GetType().Name + "'s Max to negative value of " + max);
                    return;
                }

                max = value;
            }
        }

        [SerializeField]
        protected SpeedData speed  = new SpeedData(2f);
        public SpeedData Speed
        {
            get
            {
                return speed;
            }
        }
        [Serializable]
        public class SpeedData
        {
            [SerializeField]
            protected float value;
            public float Value
            {
                get
                {
                    return value;
                }
                set
                {
                    if(value < 0f)
                    {
                        return;
                    }

                    this.value = value;
                }
            }

            [SerializeField]
            protected AnimationCurve curve;
            public AnimationCurve Curve { get { return curve; } }

            public float Get(float input)
            {
                input = Mathf.Abs(input);

                input = Mathf.Clamp(input, -1, 1f);

                return value * curve.Evaluate(input);
            }

            public SpeedData(float value)
            {
                this.value = value;
                this.curve = new AnimationCurve();
            }
        }

        [SerializeField]
        protected Vector2 value;
        public Vector2 Value { get { return value; } }

        Vector2 lastInput;

        public virtual Vector2 Scale(Vector2 input)
        {
            return Vector2.Scale(input, value);
        }

        public virtual void Process(Vector2 input)
        {
            value.x = ProcessAxis(value.x, input.x, ref lastInput.x);
            value.y = ProcessAxis(value.y, input.y, ref lastInput.y);
        }

        public virtual float ProcessAxis(float value, float input, ref float lastInput)
        {
            if (input == 0f || Mathf.Sign(input) != Mathf.Sign(lastInput))
                value = 0f;

            lastInput = input;

            return Mathf.MoveTowards(value, max * Mathf.Abs(input), speed.Get(input) * Time.deltaTime);
        }

        public AxisAcceleration(float max)
        {
            this.max = max;
            this.value = Vector2.zero;
        }
    }
}