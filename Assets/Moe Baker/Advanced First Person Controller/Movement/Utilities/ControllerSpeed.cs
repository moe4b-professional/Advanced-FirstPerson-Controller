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
    public abstract partial class ControllerSpeedBase : FPController.Module
    {
        protected float _maxValue;
        public float MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if(value < 0f)
                {
                    return;
                }

                _maxValue = value;

                Value = Value;
            }
        }
        public virtual void UpdateMaxValue()
        {
            MaxValue = Controller.Movement.State.Traverser.Transition.Speed;
        }

        protected Vector2 _value;
        public Vector2 Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value.magnitude > MaxValue)
                    value = value.normalized * MaxValue;

                _value = value;

                Magnitude = _value.magnitude;
                Scale = Magnitude / MaxValue;
            }
        }

        public float Magnitude { get; protected set; }
        public float Scale { get; protected set; }


        public bool IsStale { get { return Mathf.Approximately(Magnitude, 0f); } }


        public override void Init(FPController link)
        {
            base.Init(link);

            UpdateMaxValue();

            Value = Vector2.zero;
        }


        public virtual void Calculate(float scale, ControllerSpeed.IModifiers modifiers)
        {
            Calculate(scale, modifiers.Acceleration, modifiers.DeAcceleration, modifiers.Snap);
        }
        public virtual void Calculate(float scale, float acceleration, float deAcceleration, bool snap)
        {
            Value = new Vector2()
            {
                x = ProcessAxis(Value.x, InputModule.Move.x * scale, acceleration, deAcceleration, snap),
                y = ProcessAxis(Value.y, InputModule.Move.y * scale, acceleration, deAcceleration, snap)
            };
        }

        public virtual float ProcessAxis(float value, float input, float acceleration, float deAcceleration, bool snap)
        {
            if (Mathf.Approximately(input, 0f))
                return ProcessAxisDeAcceleration(value, deAcceleration);
            else
                return ProcessAxisAcceleration(value, input, acceleration, snap);
        }

        public virtual void CalculateAcceleration(float scale, float speed, bool snap)
        {
            Value = new Vector2()
            {
                x = ProcessAxisAcceleration(Value.x, InputModule.Move.x * scale, speed, snap),
                y = ProcessAxisAcceleration(Value.y, InputModule.Move.y * scale, speed, snap)
            };
        }
        public virtual float ProcessAxisAcceleration(float value, float input, float speed, bool snap)
        {
            if (snap)
                value = ProcessSnap(value, input);

            return Mathf.MoveTowards(value, MaxValue * Mathf.Abs(input) * Mathf.Sign(input), speed * Time.deltaTime);
        }
        public virtual float ProcessSnap(float value, float input)
        {
            if ((value > 0f && input < 0f) || (value < 0f && input > 0f))
                return 0f;
            else
                return value;
        }

        public virtual void CalculateDeAcceleration(float speed)
        {
            Value = new Vector2()
            {
                x = ProcessAxisDeAcceleration(Value.x, speed),
                y = ProcessAxisDeAcceleration(Value.y, speed)
            };
        }
        public virtual float ProcessAxisDeAcceleration(float value, float speed)
        {
            return Mathf.MoveTowards(value, 0f, speed * Time.deltaTime);
        }

        public interface IModifiersBase
        {
            float Acceleration { get; }
            float DeAcceleration { get; }

            bool Snap { get; }
        }
        [Serializable]
        public class ModifiersBase : IModifiersBase
        {
            [SerializeField]
            protected float acceleration;
            public float Acceleration
            {
                get
                {
                    return acceleration;
                }
                set
                {
                    if (value < 0f)
                    {
                        Debug.LogWarning("Trying to set " + GetType().Name + "'s Acceleration To a negative value of " + value.ToString() + " is invalid");
                        return;
                    }

                    acceleration = value;
                }
            }

            [SerializeField]
            protected float deAcceleration;
            public float DeAcceleration
            {
                get
                {
                    return deAcceleration;
                }
                set
                {
                    if (value < 0f)
                    {
                        Debug.LogWarning("Trying to set " + GetType().Name + "'s DeAcceleration To a negative value of " + value.ToString() + " is invalid");
                        return;
                    }

                    deAcceleration = value;
                }
            }

            public bool Snap;
            bool IModifiersBase.Snap { get { return Snap; } }

            public ModifiersBase(float delta) : this(delta, delta, true)
            {

            }
            public ModifiersBase(float delta, bool snap) : this(delta, delta, snap)
            {

            }
            public ModifiersBase(float acceleration, float deAcceleration, bool snap)
            {
                this.acceleration = acceleration;
                this.deAcceleration = deAcceleration;
                this.Snap = snap;
            }
        }
    }

	public partial class ControllerSpeed : ControllerSpeedBase
    {
        public interface IModifiers : IModifiersBase
        {
            
        }
        [Serializable]
        public class Modifiers : ModifiersBase, IModifiers
        {
            public Modifiers(float delta) : base(delta, delta, true)
            {

            }
            public Modifiers(float delta, bool snap) : base(delta, delta, snap)
            {

            }
            public Modifiers(float acceleration, float deAcceleration, bool snap) : base(acceleration, deAcceleration, snap)
            {
                
            }
        }
    }
}