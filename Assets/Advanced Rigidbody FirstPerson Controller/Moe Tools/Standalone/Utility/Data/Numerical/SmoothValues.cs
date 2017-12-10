using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Moe.Tools
{
    [Serializable]
    public class SmoothValue
    {
        [SerializeField]
        protected float value;
        public virtual float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        [SerializeField]
        protected float delta;
        public virtual float Delta
        {
            get
            {
                return delta;
            }
            set
            {
                delta = value;
            }
        }

        [SerializeField]
        protected bool angle;
        public bool Angle
        {
            get
            {
                return angle;
            }
        }

        public float MoveTowards(float target)
        {
            return MoveTowards(target, delta);
        }
        public virtual float MoveTowards(float target, float delta)
        {
            if (angle)
                return MoveTowardsAngle(target, delta);

            return MoveTowardsValue(target, delta);
        }
        public virtual float MoveTowardsValue(float target, float delta)
        {
            value = Mathf.MoveTowards(value, target, delta * Time.deltaTime);
            return value;
        }
        public virtual float MoveTowardsAngle(float target, float delta)
        {
            value = Mathf.MoveTowardsAngle(value, target, delta * Time.deltaTime);
            return value;
        }

        public float LerpTowards(float target)
        {
            return LerpTowards(target, delta);
        }
        public virtual float LerpTowards(float target, float delta)
        {
            if (angle)
                return LerpTowardsAngle(target, delta);

            return LerpTowardsValue(target, delta);
        }
        public virtual float LerpTowardsValue(float target, float delta)
        {
            value = Mathf.Lerp(value, target, delta * Time.deltaTime);
            return value;
        }
        public virtual float LerpTowardsAngle(float target, float delta)
        {
            value = Mathf.Lerp(value, target, delta * Time.deltaTime);
            return value;
        }

        public SmoothValue(float value, float delta) : this(value, delta, false)
        {

        }
        public SmoothValue(float value, float delta, bool angle)
        {
            this.value = value;
            this.delta = delta;

            this.angle = angle;
        }
    }

    [Serializable]
    public class TargetSmoothValue : SmoothValue
    {
        [SerializeField]
        float target;
        public float Target { get { return target; } set { target = value; } }

        public virtual void MoveTowardsTarget()
        {
            MoveTowards(target);
        }

        public TargetSmoothValue(float target, float delta) : base(0, delta)
        {
            this.target = target;
        }
        public TargetSmoothValue(float target, float delta, bool angle) : base(0, delta, angle)
        {
            this.target = target;
        }
    }

    [Serializable]
    public abstract class RangedSmoothValueBase : SmoothValue
    {
        public abstract float Min
        {
            get;
        }
        public abstract float Max
        {
            get;
        }

        public override float Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                this.value = Mathf.Clamp(value, Min, Max);
            }
        }

        public virtual void MoveTowardsMin()
        {
            MoveTowards(Min);
        }
        public virtual void MoveTowardsMax()
        {
            MoveTowards(Max);
        }

        public virtual void SetValueToMin()
        {
            value = Min;
        }
        public virtual void SetValueToMax()
        {
            value = Max;
        }

        public virtual float Lerp(float scale)
        {
            if (angle)
                return LerpAngle(scale);

            return LerpValue(scale);
        }
        public virtual float LerpValue(float scale)
        {
            value = Mathf.Lerp(Min, Max, scale);
            return value;
        }
        public virtual float LerpAngle(float scale)
        {
            value = Mathf.LerpAngle(Min, Max, scale);
            return value;
        }

        public RangedSmoothValueBase(float delta) : base(0, delta)
        {

        }
        public RangedSmoothValueBase(float delta, bool angle) : base(0, delta, angle)
        {

        }
    }

    public abstract class RangedSmoothValue<TRange> : RangedSmoothValueBase where TRange : IFloatRange
    {
        [SerializeField]
        protected TRange range;
        public override float Min
        {
            get
            {
                return range.Min;
            }
        }
        public override float Max
        {
            get
            {
                return range.Max;
            }
        }

        public RangedSmoothValue(float delta) : base(delta)
        {

        }
        public RangedSmoothValue(float delta, bool angle) : base(delta, angle)
        {

        }
    }

    [Serializable]
    public class RangedSmoothValue : RangedSmoothValue<FloatValueRange>
    {
        public void SetMin(float min)
        {
            range.Min = min;
        }
        public void SetMax(float max)
        {
            range.Max = max;
        }

        public RangedSmoothValue(float min, float max, float delta) : base(delta)
        {
            range = new FloatValueRange(min, max);
        }
        public RangedSmoothValue(float min, float max, float delta, bool angle) : base(delta, angle)
        {
            range = new FloatValueRange(min, max);
        }
    }

    [Serializable]
    public class MaxSmoothValue : RangedSmoothValueBase
    {
        [SerializeField]
        float max;
        public override float Max
        {
            get
            {
                return max;
            }
        }
        public override float Min
        {
            get
            {
                return 0f;
            }
        }

        public void SetMax(float max)
        {
            this.max = max;
        }

        public MaxSmoothValue(float max, float delta) : base(delta, false)
        {
            this.max = max;
        }
    }

    [Serializable]
    public class ScaleSmoothValue : RangedSmoothValueBase
    {
        public override float Min
        {
            get
            {
                return 0f;
            }
        }
        public override float Max
        {
            get
            {
                return 1f;
            }
        }

        public ScaleSmoothValue(float delta) : base(delta, false)
        {

        }
    }

    [Serializable]
    public abstract class BaseAxisSmoothValue : RangedSmoothValueBase
    {
        [SerializeField]
        protected float gravity;
        public float Gravity
        {
            get
            {
                return gravity;
            }
            set
            {
                gravity = value;
            }
        }

        [SerializeField]
        bool snap = true;
        public bool Snap { get { return snap; } }

        public virtual void Update(float scale)
        {
            scale = Mathf.Clamp(scale, Min, Max);

            if (scale == 0f)
                MoveTowards(0, gravity);
            else
            {
                if (snap && ((scale > 0f && value < 0f) || (scale < 0f && value > 0f)))
                {
                    value = 0f;
                }

                MoveTowards((scale > 0f ? Max : Min) * Mathf.Abs(scale), delta);
            }
        }

        public BaseAxisSmoothValue(float delta, float gravity, bool snap) : base(delta, false)
        {
            this.gravity = gravity;
            this.snap = snap;
        }
        public BaseAxisSmoothValue(float delta, bool snap) : this(delta, delta, snap)
        {

        }
        public BaseAxisSmoothValue(float delta) : this(delta, delta, true)
        {

        }
    }

    [Serializable]
    public class AxisSmoothValue : BaseAxisSmoothValue
    {
        public override float Min
        {
            get
            {
                return -1;
            }
        }
        public override float Max
        {
            get
            {
                return 1;
            }
        }

        public AxisSmoothValue(float delta, float gravity, bool snap) : base(delta, gravity, snap)
        {
            
        }
        public AxisSmoothValue(float delta, bool snap) : base(delta, snap)
        {

        }
        public AxisSmoothValue(float delta) : base(delta)
        {

        }
    }
}