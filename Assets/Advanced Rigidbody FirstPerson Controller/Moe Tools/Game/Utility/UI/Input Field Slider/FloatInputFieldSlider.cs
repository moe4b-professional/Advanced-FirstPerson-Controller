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
    public class FloatInputFieldSlider : InputFieldSlider<float>
    {
        [SerializeField]
        int decimalsRounding = 2;

        protected override InputField.ContentType ContentType { get { return InputField.ContentType.DecimalNumber; } }

        protected override void EditValue()
        {
            base.EditValue();

            value = (float)Math.Round(value, decimalsRounding);
        }

        public override float ClampValue(float data)
        {
            return Mathf.Clamp(data, MinValue, MaxValue);
        }

        public override float GetData(float value)
        {
            return value;
        }

        public override float GetFloat(float data)
        {
            return data;
        }

        protected override float GetData(string text)
        {
            float value;

            if (float.TryParse(text, out value))
                return value;
            else
                return MinValue;
        }
    }
}